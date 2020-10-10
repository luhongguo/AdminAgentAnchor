using Elight.Entity.Model;
using Elight.Entity.Sys;
using Elight.Logic.Base;
using Elight.Utility.Log;
using Elight.Utility.Model;
using Elight.Utility.Operator;
using Elight.Utility.ResponseModels;
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
        public readonly int ShopID = OperatorProvider.Instance.Current.ShopID;
        /// <summary>
        /// 用户返点分页信息
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public List<SysRebateEntity> GetUserRebatePage(PageParm parm, ref int totalCount)
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
                    return db.Queryable<SysRebateEntity, SysUser>((gt, it) => new object[] { JoinType.Left, gt.UserID == it.Id })
                               .WhereIF(dic.ContainsKey("Name") && !string.IsNullOrEmpty(dic["Name"].ToString()), (gt, it) => it.Account.Contains(dic["Name"].ToString()) || it.RealName.Contains(dic["Name"].ToString()))
                               .Select((gt, it) => new SysRebateEntity
                               {
                                   id = gt.id,
                                   TipRebate = gt.TipRebate,
                                   HourRebate = gt.HourRebate,
                                   ModifiedBy = gt.ModifiedBy,
                                   ModifiedTime = gt.ModifiedTime,
                                   CreateTime = gt.CreateTime,
                                   UserAccount = it.Account
                               })
                               .ToPageList(parm.page, parm.limit, ref totalCount);
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "用户返点分页信息", ex.Message, ex.StackTrace);
            }
            return result;
        }
        /// <summary>
        /// 根据主键得到用户返点信息
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public SysRebateEntity Get(int primaryKey)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysRebateEntity, SysUser>((A, B) => new object[] { JoinType.Left, A.UserID == B.Id })
                    .Where((A, B) => A.id == primaryKey)
                    .Select((A, B) => new SysRebateEntity
                    {
                        id = A.id,
                        UserAccount = B.Account,
                        TipRebate = A.TipRebate,
                        HourRebate = A.HourRebate
                    }).First();
            }
        }
        /// <summary>
        /// 新增返点
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(SysRebateEntity model)
        {
            try
            {
                using (var db = GetInstance())
                {
                    model.CreateTime = DateTime.Now;
                    model.ModifiedBy = OperatorProvider.Instance.Current.Account;
                    model.ModifiedTime = model.CreateTime;
                    return db.Insertable(model).ExecuteReturnIdentity();
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "新增返点", ex.Message, ex.StackTrace);
            }
            return 0;
        }
        /// <summary>
        /// 更新返点
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(SysRebateEntity model)
        {
            try
            {
                using (var db = GetInstance())
                {
                    model.ModifiedBy = OperatorProvider.Instance.Current.Account;
                    model.ModifiedTime = DateTime.Now;
                    return db.Updateable(model).UpdateColumns(it => new
                    {
                        it.TipRebate,
                        it.HourRebate
                    }).ExecuteCommand();
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "修改商户", ex.Message, ex.StackTrace);
            }
            return 0;
        }
        /// <summary>
        /// 验证用户返点是否存在
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public SysRebateEntity GetRebateByAccount(string userID)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysRebateEntity>().Where(it => it.UserID == userID).First();
            }
        }
        /// <summary>
        /// 批量删除返点信息
        /// </summary>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        public int Delete(List<int> idList)
        {
            using (var db = GetInstance())
            {
                return db.Deleteable<SysRebateEntity>().In(idList).ExecuteCommand();
            }
        }
    }
}
