using Elight.Entity.Model;
using Elight.Entity.Sys;
using Elight.Logic.Base;
using Elight.Utility.Log;
using Elight.Utility.Model;
using Elight.Utility.Operator;
using Newtonsoft.Json;
using SqlSugar;
using SyntacticSugar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elight.Logic.Sys
{
    public class SysRebateLogic : BaseLogic
    {
        /// <summary>
        /// 主播返点集合
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public List<SysRebateEntity> GetHourDetailsPage(PageParm parm, ref int totalCount)
        {
            var result = new List<SysRebateEntity>();
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
                using (var db = GetSqlSugarDB(DbConnType.QPAgentAnchorDB))
                {
                    if (dic.ContainsKey("userName") && dic["userName"].ToString() == "-1")//选中全部
                    {
                        return db.Queryable<SysRebateEntity, SysAnchor>((it, gt) => new object[] { JoinType.Left, it.anchorID == gt.id })
                                   .WhereIF(dic.ContainsKey("Name") && !string.IsNullOrEmpty(dic["Name"].ToString()), (it, gt) => gt.username.Contains(dic["Name"].ToString()) || gt.nickname.Contains(dic["Name"].ToString()))
                                   .Select(it => it)
                                   .ToPageList(parm.page, parm.limit, ref totalCount);
                    }
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "主播返点集合 分页信息", ex.Message, ex.StackTrace);
            }
            return result;
        }
    }
}
