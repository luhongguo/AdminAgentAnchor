﻿using Elight.Entity.Sys;
using Elight.Logic.Base;
using Elight.Utility.Log;
using Elight.Utility.Model;
using Elight.Utility.Operator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using SqlSugar;
using System.Linq;
using Elight.Entity;

namespace Elight.Logic.Sys
{
    public class SysAgentWithdrawalRecordLogic : BaseLogic
    {
        /// <summary>
        /// 获取经纪人提现记录
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public List<SysAgentWithdrawalRecordEntity> GetAgentWithdrawalRecordPage(PageParm parm, ref int totalCount)
        {
            var result = new List<SysAgentWithdrawalRecordEntity>();
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
                    return db.Queryable<SysAgentWithdrawalRecordEntity, SysUser, SysAgentBankEntity>((it, st, at) => new object[] { JoinType.Left, it.AgentID == st.Id, JoinType.Left, it.AgentBankID == at.id })
                          .Where((it, st, at) => it.createTime >= Convert.ToDateTime(dic["startTime"]) && it.createTime < Convert.ToDateTime(dic["endTime"]))
                        .WhereIF(dic.ContainsKey("Name") && !string.IsNullOrEmpty(dic["Name"].ToString()), (it, st) => st.Account.Contains(dic["Name"].ToString()))
                         .WhereIF(dic.ContainsKey("Status") && Convert.ToInt32(dic["Status"]) != -1, (it, st) => it.Status == Convert.ToInt32(dic["Status"]))
                        .Select((it, st, at) => new SysAgentWithdrawalRecordEntity
                        {
                            id = it.id,
                            AgentName = st.Account,
                            WithdrawalAmount = it.WithdrawalAmount,
                            CategoryCode = at.CategoryCode,
                            bankano = at.bankano,
                            bankaccount = at.bankaccount,
                            address = at.address,
                            Remark = it.Remark,
                            payType = at.payType,
                            Type = it.Type,
                            Feedback = it.Feedback,
                            Status = it.Status,
                            createTime = it.createTime,
                            ModifiedTime = it.ModifiedTime,
                            ModifiedBy = it.ModifiedBy
                        })
                        .OrderBy(" it.Status desc")
                        .ToPageList(parm.page, parm.limit, ref totalCount);
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "获取经纪人提现记录", ex.Message, ex.StackTrace);
            }
            return result;
        }
        /// <summary>
        /// 新增提现记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(SysAgentWithdrawalRecordEntity model)
        {
            try
            {
                using (var db = GetInstance())
                {
                    model.Status = 3;
                    model.createTime = DateTime.Now;
                    model.ModifiedTime = DateTime.Now;
                    model.Remark = model.Remark;
                    model.WithdrawalAmount = Math.Truncate(model.WithdrawalAmount * 100) / 100;
                    return db.Insertable(model).ExecuteReturnIdentity();
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "新增提现记录", ex.Message, ex.StackTrace);
            }
            return 0;
        }
        /// <summary>
        /// 根据主键得到提现记录信息
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public SysAgentWithdrawalRecordEntity Get(long primaryKey)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysAgentWithdrawalRecordEntity>()
                    .Where((A) => A.id == primaryKey)
                    .Select((A) => new SysAgentWithdrawalRecordEntity
                    {
                        id = A.id,
                        AgentID = A.AgentID,
                        Status = A.Status,
                        Feedback = A.Feedback,
                        WithdrawalAmount = A.WithdrawalAmount
                    })
                    .First();
            }
        }
        /// <summary>
        /// 处理提现成功
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(SysAgentWithdrawalRecordEntity model, SysUser agentModel)
        {
            int result = 0;
            using (var db = GetInstance())
            {
                try
                {
                    db.Ado.BeginTran();//开启事务
                    result = db.Updateable<SysAgentWithdrawalRecordEntity>().SetColumns(it => new SysAgentWithdrawalRecordEntity
                    {
                        Status = model.Status,
                        Feedback = model.Feedback,
                        WithdrawalAmount = model.WithdrawalAmount,
                        ModifiedBy = OperatorProvider.Instance.Current.Account,
                        ModifiedTime = DateTime.Now
                    }).Where(it => it.id == model.id).ExecuteCommand();
                    //更新用户余额
                    db.Updateable<SysUser>().SetColumns(it => new SysUser { Balance = agentModel.Balance - model.WithdrawalAmount*10 }).Where(it => it.Id == agentModel.Id).ExecuteCommand();
                    db.Ado.CommitTran();
                }
                catch (Exception ex)
                {
                    db.Ado.RollbackTran();
                    new LogLogic().Write(Level.Error, "处理提现金额", ex.Message, ex.StackTrace);
                }
                return result;
            }
        }
        /// <summary>
        /// 处理提现驳回
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Reject(SysAgentWithdrawalRecordEntity model)
        {
            var result = 0;
            using (var db = GetInstance())
            {
                try
                {
                    result = db.Updateable<SysAgentWithdrawalRecordEntity>().SetColumns(it => new SysAgentWithdrawalRecordEntity
                    {
                        Status = model.Status,
                        Feedback = model.Feedback,
                        WithdrawalAmount = model.WithdrawalAmount,
                        ModifiedBy = OperatorProvider.Instance.Current.Account,
                        ModifiedTime = DateTime.Now
                    }).Where(it => it.id == model.id).ExecuteCommand();
                }
                catch (Exception ex)
                {
                    new LogLogic().Write(Level.Error, "处理提现金额", ex.Message, ex.StackTrace);
                }
                return result;
            }
        }
    }
}
