using Elight.Entity.Sys;
using Elight.Logic.Base;
using Elight.Utility.Log;
using Elight.Utility.Model;
using Elight.Utility.Operator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using SqlSugar;
using System.Linq;

namespace Elight.Logic.Sys
{
    public class SysAgentBankLogic : BaseLogic
    {
        /// <summary>
        /// 银行卡分页
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public List<SysAgentBankEntity> GetAgentBankPage(PageParm parm, ref int totalCount)
        {
            var result = new List<SysAgentBankEntity>();
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
                    return db.Queryable<SysAgentBankEntity, SysUser>((it, st) => new object[] { JoinType.Left, it.AgentID == st.Id })
                                    .WhereIF(dic.ContainsKey("Name") && !string.IsNullOrEmpty(dic["Name"].ToString()), (it, st) => st.Account.Contains(dic["Name"].ToString()))
                                    .Select((it, st) => new SysAgentBankEntity
                                    {
                                        id = it.id,
                                        AgentName = st.Account,
                                        address = it.address,
                                        bankaccount = it.bankaccount,
                                        bankano = it.bankano,
                                        CategoryCode = it.CategoryCode,
                                        createtime = it.createtime,
                                        payType = it.payType
                                    })
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
        public SysAgentBankEntity Get(int primaryKey)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysAgentBankEntity, SysUser>((A, B) => new Object[] { JoinType.Left, A.AgentID == B.Id })
                    .Where((A) => A.id == primaryKey)
                    .Select((A, B) => new SysAgentBankEntity
                    {
                        id = A.id,
                        AgentName = B.Account,
                        address = A.address,
                        bankaccount = A.bankaccount,
                        bankano = A.bankano,
                        CategoryCode = A.CategoryCode,
                        payType = A.payType
                    })
                    .First();
            }
        }
        /// <summary>
        /// 新增经纪人银行卡
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(SysAgentBankEntity model)
        {
            try
            {
                using (var db = GetInstance())
                {
                    model.createtime = DateTime.Now;
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
        /// 更新经纪人银行卡
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(SysAgentBankEntity model)
        {
            try
            {
                using (var db = GetInstance())
                {
                    return db.Updateable(model).UpdateColumns(it => new
                    {
                        model.bankano,
                        model.CategoryCode,
                        model.payType,
                        model.bankaccount,
                        model.address
                    }).ExecuteCommand();
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "更新经纪人银行卡", ex.Message, ex.StackTrace);
            }
            return 0;
        }
        /// <summary>
        /// 批量删除银行卡信息
        /// </summary>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        public int Delete(List<long> idList)
        {
            using (var db = GetInstance())
            {
                return db.Deleteable<SysAgentBankEntity>().In(idList).ExecuteCommand();
            }
        }
        /// <summary>
        /// 经纪人银行卡下拉框
        /// </summary>
        /// <param name="id">经纪人id</param>
        /// <returns></returns>
        public string GetUserBankSelect(string id)
        {
            var result = "";
            try
            {
                using (var db = GetInstance())
                {
                    result = db.Queryable<SysAgentBankEntity>()
                        .Where(it => it.AgentID == id)
                                .Select((it) => new
                                {
                                    BankName = it.CategoryCode + "--" + it.bankano,
                                    it.id,
                                }).ToJson();
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "经纪人银行卡下拉框", ex.Message, ex.StackTrace);
            }
            return result;
        }
    }
}
