using Elight.Entity.Model;
using Elight.Entity.Sys;
using Elight.Logic.Base;
using Elight.Utility.Log;
using Elight.Utility.Model;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elight.Logic.Sys
{
    public class SysTipIncomeDetailLogic : BaseLogic
    {
        /// <summary>
        /// 礼物返点信息 分页信息
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public List<TipIncomeDetailModel> GetTipIncomeDetailPage(PageParm parm, ref int totalCount, ref TipIncomeDetailModel sumModel)
        {
            var res = new List<TipIncomeDetailModel>();
            try
            {
                if (parm == null)
                {
                    parm = new PageParm();
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(parm.where))
                {
                    dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(parm.where);

                }
                var intStartTime = int.Parse(Convert.ToDateTime(dic["startTime"]).ToString("yyyyMMdd"));
                var intEndTime = int.Parse(Convert.ToDateTime(dic["endTime"]).ToString("yyyyMMdd"));
                using (var db = GetSqlSugarDB(DbConnType.QPAnchorRecordDB))
                {
                    var query = db.Queryable<SysTipIncomeDetailEntity, SysUser, SysAnchor, TipEntity>((at, bt, ct, dt) => new object[] {
                                JoinType.Left,at.UserID==bt.Id,
                                JoinType.Left,at.AnchorID==ct.id,
                                JoinType.Left,at.orderno==dt.orderno
                      })
                          .Where((at, bt, ct, dt) => at.StartDate >= intStartTime && at.StartDate < intEndTime)
                          .WhereIF(dic.ContainsKey("AgentName") && !string.IsNullOrEmpty(dic["AgentName"].ToString()), (at, bt, ct, dt) => bt.Account.Contains(dic["AgentName"].ToString()))
                          .WhereIF(dic.ContainsKey("AnchorName") && !string.IsNullOrEmpty(dic["AnchorName"].ToString()), (at, bt, ct, dt) => ct.username.Contains(dic["AnchorName"].ToString()) || ct.nickname.Contains(dic["AnchorName"].ToString()));
                    sumModel = query.Clone().Select((at, bt, ct, dt) => new TipIncomeDetailModel
                    {
                        AnchorIncome = SqlFunc.AggregateSum(at.AnchorIncome),
                        UserIncome = SqlFunc.AggregateSum(at.UserIncome),
                        PlatformIncome = SqlFunc.AggregateSum(at.PlatformIncome),
                        totalamount = SqlFunc.AggregateSum(dt.totalamount)
                    }).WithCache(60).First();
                    res = query
                         .Select((at, bt, ct, dt) => new TipIncomeDetailModel
                         {
                             UserName = bt.Account,
                             AnchorName = ct.username,
                             UserIncome = at.UserIncome,
                             AnchorIncome = at.AnchorIncome,
                             PlatformIncome = at.PlatformIncome,
                             UserRebate = at.UserRebate,
                             PlatformRebate = at.PlatformRebate,
                             orderno = dt.orderno,
                             giftname = dt.giftname,
                             price = dt.price,
                             quantity = dt.quantity,
                             totalamount = dt.totalamount,
                             sendtime = dt.sendtime
                         }).WithCache(60)
                        .ToPageList(parm.page, parm.limit, ref totalCount);
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "礼物返点信息 分页信息", ex.Message, ex.StackTrace);
            }
            return res;
        }
    }
}
