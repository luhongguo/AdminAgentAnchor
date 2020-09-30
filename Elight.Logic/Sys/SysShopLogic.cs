using Elight.Entity.Sys;
using Elight.Logic.Base;
using Elight.Utility.Log;
using Elight.Utility.Model;
using Elight.Utility.Operator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elight.Logic.Sys
{
    public class SysShopLogic : BaseLogic
    {
        /// <summary>
        /// 商户分页
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public List<SysShopEntity> GetShopPage(PageParm parm, ref int totalCount)
        {
            var result = new List<SysShopEntity>();
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
                    return db.Queryable<SysShopEntity>()
                                    .WhereIF(dic.ContainsKey("Name") && !string.IsNullOrEmpty(dic["Name"].ToString()), (it) => it.Name.Contains(dic["Name"].ToString()))
                                    .Select(it => it)
                                    .ToPageList(parm.page, parm.limit, ref totalCount);
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "商户分页信息", ex.Message, ex.StackTrace);
            }
            return result;
        }
        /// <summary>
        /// 根据主键得到商户信息
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public SysShopEntity Get(int primaryKey)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysShopEntity>().Where((A) => A.ID == primaryKey).First();
            }
        }
        /// <summary>
        /// 新增商户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(SysShopEntity model)
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
                new LogLogic().Write(Level.Error, "新增商户", ex.Message, ex.StackTrace);
            }
            return 0;
        }

        /// <summary>
        /// 更新商户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(SysShopEntity model)
        {
            try
            {
                using (var db = GetInstance())
                {
                    model.ModifiedBy = OperatorProvider.Instance.Current.Account;
                    model.ModifiedTime = DateTime.Now;
                    return db.Updateable(model).UpdateColumns(it => new
                    {
                        it.Name,
                        it.ModifiedBy,
                        it.ModifiedTime
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
        /// 批量删除商户信息
        /// </summary>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        public int Delete(List<int> idList)
        {
            using (var db = GetInstance())
            {
                return db.Deleteable<SysShopEntity>().In(idList).ExecuteCommand();
            }
        }
    }
}
