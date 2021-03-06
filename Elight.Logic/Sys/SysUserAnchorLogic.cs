﻿using Elight.Entity.Enum;
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
    public class SysUserAnchorLogic : BaseLogic
    {
        /// <summary>
        /// 超管查看主播信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<SysAnchor> UserSelectAnchorList(PageParm parm, ref int totalCount)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(parm.where))
            {
                dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(parm.where);
            }
            var result = new List<SysAnchor>();
            try
            {
                //lmstatus  连麦状态 live 直播 offline 离线  normal正常 kickline踢线  disabled禁用
                //statu 	正常unlock 禁用 lock 审核中 audit
                using (var db = GetInstance())
                {
                    result = db.Queryable<SysAnchor, SysAnchorInfoEntity, SysShopAnchorEntity, SysShopEntity>((it, st, ot, dt) => new object[] {
                        JoinType.Left, it.id == st.aid,
                        JoinType.Left, it.id == ot.AnchorID,
                        JoinType.Left, ot.ShopID==dt.ID
                    })
                                .WhereIF(dic.ContainsKey("Name") && !string.IsNullOrEmpty(dic["Name"].ToString()), (it) => it.anchorName.Contains(dic["Name"].ToString()) || it.nickName.Contains(dic["Name"].ToString()))
                                .WhereIF(dic.ContainsKey("startTime") && !string.IsNullOrEmpty(dic["startTime"].ToString()) && dic.ContainsKey("endTime") && !string.IsNullOrEmpty(dic["endTime"].ToString()), (it) => it.createTime >= Convert.ToDateTime(dic["startTime"]) && it.createTime <= Convert.ToDateTime(dic["endTime"]))
                                .WhereIF(dic.ContainsKey("Status") && Convert.ToInt32(dic["Status"]) != -1, (it, st) => st.status == (AnchorStatusEnum)Convert.ToInt32(dic["Status"]))
                                .WhereIF(dic.ContainsKey("isColletCode") && !string.IsNullOrEmpty(dic["isColletCode"].ToString()), (it, st) => it.isColletCode == dic["isColletCode"].ToString())
                                .WhereIF(dic.ContainsKey("ShopID") && Convert.ToInt32(dic["ShopID"]) != -1, (it, st, ot) => ot.ShopID == Convert.ToInt32(dic["ShopID"]))
                                .Select((it, st, ot, dt) => new SysAnchor
                                {
                                    id = it.id,
                                    anchorName = it.anchorName,
                                    nickName = it.nickName,
                                    headUrl = SqlFunc.IIF(it.headUrl.Contains("http"), it.headUrl, Image_CDN + it.headUrl),
                                    balance = st.agentGold,
                                    follow = st.follow,
                                    birthday = it.birthday,
                                    status = st.status,
                                    createTime = it.createTime,
                                    isColletCode = it.isColletCode,
                                    shopName = dt.Name,
                                    sort = it.sort
                                })
                                .OrderBy(" st.agentGold desc")
                                .ToPageList(parm.page, parm.limit, ref totalCount);
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "超管查看主播信息", ex.Message, ex.StackTrace);
            }
            return result;
        }
        /// <summary>
        /// 超管获得经纪人拥有的主播 列表分页 （已经取消了这个经纪人授权主播功能，添加了商户授权主播）
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<SysAnchor> GetUserAnchorList(int pageIndex, int pageSize, string keyWord, ref int totalCount)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(keyWord))
            {
                dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(keyWord);
            }
            var result = new List<SysAnchor>();
            try
            {
                //lmstatus  连麦状态 live 直播 offline 离线 disabled禁用 normal正常 kickline踢线
                //statu 	正常unlock 禁用 lock 审核中 audit
                using (var db = GetInstance())
                {
                    //result = db.Queryable<SysUserAnchor, SysAnchor>((st, it) => new object[] { JoinType.Left, st.AnchorID == it.id })
                    //             .WhereIF(dic.ContainsKey("anchorUserName") && !string.IsNullOrEmpty(dic["anchorUserName"].ToString()), (st, it) => it.username.Contains(dic["anchorUserName"].ToString()) || it.nickname.Contains(dic["anchorUserName"].ToString()))
                    //             .WhereIF(dic.ContainsKey("userID") && !string.IsNullOrEmpty(dic["userID"].ToString()), (st, it) => st.UserID == dic["userID"].ToString())
                    //             .WhereIF(dic.ContainsKey("isCollet") && Convert.ToInt32(dic["isCollet"]) != -1, (st, it) => it.isCollet == Convert.ToInt32(dic["isCollet"]))
                    //             .WhereIF(dic.ContainsKey("isColletCode") && dic["isColletCode"].ToString() != "-1", (st, it) => it.isColletCode == dic["isColletCode"].ToString())
                    //             .Select((st, it) => new SysAnchor
                    //             {
                    //                 id = it.id,
                    //                 //username = it.username,
                    //                 //nickname = it.nickname,
                    //                 //photo = it.photo,
                    //                 //balance = it.balance,
                    //                 //atteCount = it.atteCount,
                    //                 //ishot = it.ishot,
                    //                 //isrecommend = it.isrecommend,
                    //                 //regtime = it.regtime,
                    //                 //viplevel = it.viplevel,
                    //                 //birthday = it.birthday,
                    //             }).ToPageList(pageIndex, pageSize, ref totalCount);
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "获得经纪人主播列表分页", ex.Message, ex.StackTrace);
            }
            return result;
        }
        /// <summary>
        /// 经纪人不拥有的主播 数据分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord">查询条件</param>
        /// <param name="totalCount">数据总数</param>
        /// <returns></returns>
        public List<SysAnchor> GetUserNoOwnedAnchorList(int pageIndex, int pageSize, string keyWord, ref int totalCount)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(keyWord))
            {
                dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(keyWord);
            }
            var result = new List<SysAnchor>();
            try
            {
                //lmstatus  连麦状态 live 直播 offline 离线 disabled禁用 normal正常 kickline踢线
                //statu 	正常unlock 禁用 lock 审核中 audit
                using (var db = GetSqlSugarDB(DbConnType.QPVideoAnchorDB))
                {
                    //result = db.Queryable<SysAnchor>()
                    //             .WhereIF(dic.ContainsKey("anchorUserName") && !string.IsNullOrEmpty(dic["anchorUserName"].ToString()), it => it.username.Contains(dic["anchorUserName"].ToString()) || it.nickname.Contains(dic["anchorUserName"].ToString()))
                    //             .WhereIF(dic.ContainsKey("isCollet") && Convert.ToInt32(dic["isCollet"]) != -1, (it) => it.isCollet == Convert.ToInt32(dic["isCollet"]))
                    //             .WhereIF(dic.ContainsKey("isColletCode") && dic["isColletCode"].ToString() != "-1", (it) => it.isColletCode == dic["isColletCode"].ToString())
                    //             .Where(it => SqlFunc.Subqueryable<SysUserAnchor>().Where(st => st.UserID == dic["userID"].ToString()).Where(st => st.AnchorID == it.id).NotAny())
                    //             .Select(it => new SysAnchor
                    //             {
                    //                 id = it.id,
                    //                 username = it.username,
                    //                 nickname = it.nickname,
                    //                 photo = it.photo,
                    //                 balance = it.balance,
                    //                 atteCount = it.atteCount,
                    //                 ishot = it.ishot,
                    //                 isrecommend = it.isrecommend,
                    //                 regtime = it.regtime,
                    //                 viplevel = it.viplevel,
                    //                 birthday = it.birthday,
                    //             }).ToPageList(pageIndex, pageSize, ref totalCount);
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "经纪人不拥有的主播 数据分页", ex.Message, ex.StackTrace);
            }
            return result;
        }
        /// <summary>
        /// 添加主播给经纪人
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="idList">主播ID集合</param>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        public bool AddUserAnchor(string[] idList, string userID)
        {
            var result = true;
            try
            {
                using (var db = GetInstance())
                {
                    List<SysUserAnchor> list = new List<SysUserAnchor>();
                    foreach (var item in idList)
                    {
                        list.Add(new SysUserAnchor { id = Guid.NewGuid().ToString().Replace("-", ""), UserID = userID, AnchorID = Convert.ToInt32(item) });
                    }
                    var count = db.Insertable(list.ToArray()).ExecuteCommand();
                    result = count == idList.Count() ? true : false;
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "添加主播给经纪人", ex.Message, ex.StackTrace);
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 删除经纪人主播
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="idList">主播ID集合</param>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        public bool Delete(string[] idList, string userID)
        {
            var result = true;
            try
            {
                using (var db = GetInstance())
                {
                    var count = db.Deleteable<SysUserAnchor>().Where(it => it.UserID == userID).In(it => it.AnchorID, idList).ExecuteCommand();
                    result = count == idList.Count() ? true : false;
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "删除经纪人主播", ex.Message, ex.StackTrace);
                result = false;
            }
            return result;
        }


        /// <summary>
        /// 主播财务报表分页信息
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public List<IncomeTemplateModel> GetAnchorReportPage(PageParm parm, ref int totalCount, ref IncomeTemplateModel sumModel)
        {
            var res = new List<IncomeTemplateModel>();
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
                using (var db = GetSqlSugarDB(DbConnType.QPAnchorRecordDB))
                {
                    var query = db.Queryable<SysIncomeEntity, SysAnchor, SysAnchorInfoEntity, SysShopAnchorEntity, SysAnchorRebateEntity>((it, st, at, ot, rt) =>
                     new object[] {
                        JoinType.Left, it.AnchorID == st.id,
                        JoinType.Left, st.id == at.aid,
                        JoinType.Left, st.id == ot.AnchorID,
                        JoinType.Left, st.id==rt.AnchorID
                     })
                          .Where((it, st) => it.opdate >= Convert.ToDateTime(dic["startTime"]) && it.opdate < Convert.ToDateTime(dic["endTime"]))
                          .WhereIF(dic.ContainsKey("Name") && !string.IsNullOrEmpty(dic["Name"].ToString()), (it, st) => st.anchorName.Contains(dic["Name"].ToString()) || st.nickName.Contains(dic["Name"].ToString()))
                          .WhereIF(dic.ContainsKey("isColletCode") && !string.IsNullOrEmpty(dic["isColletCode"].ToString()), (it, st, at) => st.isColletCode == dic["isColletCode"].ToString())
                          .WhereIF(dic.ContainsKey("ShopID") && Convert.ToInt32(dic["ShopID"]) != -1, (it, st, at, ot) => ot.ShopID == Convert.ToInt32(dic["ShopID"]))
                            ;
                    sumModel = query.Clone().Select((it, st, at) => new IncomeTemplateModel
                    {
                        tip_income = SqlFunc.AggregateSum(it.tip_income),
                        agent_income = SqlFunc.AggregateSum(it.agent_income),
                        Platform_income = SqlFunc.AggregateSum(it.Platform_income),
                        hour_income = SqlFunc.AggregateSum(it.hour_income),
                        agentHour_income = SqlFunc.AggregateSum(it.agentHour_income),
                        livetime = SqlFunc.AggregateSum(it.livetime),
                    }).First();
                    if (sumModel == null)
                    {
                        sumModel = new IncomeTemplateModel();
                    }
                    res = query.GroupBy((it, st, at, ot, rt) => new { it.AnchorID, st.anchorName, st.nickName, at.agentGold, rt.IsWorkHours, rt.LiveTime, rt.Salary, rt.TipRebate, rt.HourRebate,rt.GiftAmount })
                          .Select((it, st, at, ot, rt) => new IncomeTemplateModel
                          {
                              AnchorID = it.AnchorID,
                              AnchorName = st.anchorName,
                              NickName = st.nickName,
                              Balance = at.agentGold,
                              tip_income = SqlFunc.AggregateSum(it.tip_income),
                              agent_income = SqlFunc.AggregateSum(it.agent_income),
                              Platform_income = SqlFunc.AggregateSum(it.Platform_income),
                              hour_income = SqlFunc.AggregateSum(it.hour_income),
                              agentHour_income = SqlFunc.AggregateSum(it.agentHour_income),
                              livetime = SqlFunc.AggregateSum(it.livetime),
                              IsWorkHours = rt.IsWorkHours,
                              MinimumLiveTime = rt.LiveTime,
                              Salary = rt.Salary,
                              TipRebate = rt.TipRebate,
                              HourRebate = rt.HourRebate,
                              GiftAmount=rt.GiftAmount
                          })
                          .OrderBy(" sum(it.tip_income) desc")
                          .ToPageList(parm.page, parm.limit, ref totalCount);

                    #region 老版本写法
                    //                    var tableList = GetSqlSugarDB(DbConnType.QPAnchorRecordDB).DbMaintenance.GetTableInfoList();//获取数据库所有表名
                    //                    var query = db.Queryable<SysAnchor>()
                    //                            .WhereIF(dic.ContainsKey("Name") && !string.IsNullOrEmpty(dic["Name"].ToString()), (st) => st.username.Contains(dic["Name"].ToString()) || st.nickname.Contains(dic["Name"].ToString()))
                    //                            .WhereIF(dic.ContainsKey("isCollet") && Convert.ToInt32(dic["isCollet"]) != -1, (it) => it.isCollet == Convert.ToInt32(dic["isCollet"]))
                    //                            .WhereIF(dic.ContainsKey("isColletCode") && dic["isColletCode"].ToString() != "-1", (it) => it.isColletCode == dic["isColletCode"].ToString())
                    //                            .Select((st) => new IncomeTemplateModel
                    //                            {
                    //                                AnchorID = st.id,
                    //                                AnchorName = st.username,
                    //                                NickName = st.nickname,
                    //                                Balance = st.balance,
                    //                                isCollet = st.isCollet,
                    //                            }).WithCache(120);//缓存120秒

                    //                    res = query.Clone()
                    //                        .Mapper((it, cache) =>
                    //                              {
                    //                                  if (tableList.Any(st => st.Name.Equals($@"income_{it.AnchorName}")))//判断表是否存在
                    //                                  {
                    //                                      //isnull(sum(tip_income),0) tip_income,   
                    //                                      var table = db.SqlQueryable<IncomeTemplateModel>($@"select  isnull(sum(hour_income),0) hour_income,isnull(sum(agent_income),0) agent_income,isnull(sum(test_income),0) test_income  
                    //from QPAnchorRecordDB.dbo.income_{it.AnchorName} where opdate>='{dic["startTime"].ToString()}' and opdate<'{dic["endTime"].ToString()}'")
                    //                                                .First();
                    //                                      it.hour_income = table.hour_income;
                    //                                      it.agent_income = table.agent_income;
                    //                                      it.test_income = table.test_income;
                    //                                  }
                    //                                  if (tableList.Any(st => st.Name.Equals($@"tip_{it.AnchorName}")))//判断表是否存在
                    //                                  {
                    //                                      //礼物金额
                    //                                      var totalamount = db.SqlQueryable<TipTemplateModel>($@"select sum(totalamount) as totalamount from QPAnchorRecordDB.dbo.tip_{it.AnchorName}
                    //where sendtime>='{dic["startTime"].ToString()}' and sendtime<'{dic["endTime"].ToString()}'").First().totalamount;
                    //                                      it.tip_income = totalamount;
                    //                                  }
                    //                              })
                    //                        .ToList().OrderByDescending(it => it.tip_income).Skip((parm.page - 1) * parm.limit).Take(parm.limit).ToList();
                    //                    //.ToPageList(parm.page,m, ref totalCount);

                    //                    totalCount = query.Mapper((it, cache) =>//求和
                    //                    {
                    //                        if (tableList.Any(st => st.Name.Equals($@"income_{it.AnchorName}")))//判断表是否存在
                    //                        {
                    //                            //isnull(sum(tip_income),0) tip_income,   
                    //                            var table = db.SqlQueryable<IncomeTemplateModel>($@"select  isnull(sum(hour_income),0) hour_income,isnull(sum(agent_income),0) agent_income,isnull(sum(test_income),0) test_income  
                    //from QPAnchorRecordDB.dbo.income_{it.AnchorName} where opdate>='{dic["startTime"].ToString()}' and opdate<'{dic["endTime"].ToString()}'")
                    //                                      .First();
                    //                            hour_income += table.hour_income;
                    //                            agent_income += table.agent_income;
                    //                            test_income += table.test_income;
                    //                        }
                    //                        if (tableList.Any(st => st.Name.Equals($@"tip_{it.AnchorName}")))//判断表是否存在
                    //                        {
                    //                            //礼物金额
                    //                            var totalamount = db.SqlQueryable<TipTemplateModel>($@"select sum(totalamount) as totalamount from QPAnchorRecordDB.dbo.tip_{it.AnchorName}
                    //where sendtime>='{dic["startTime"].ToString()}' and sendtime<'{dic["endTime"].ToString()}'").First().totalamount;
                    //                            tip_income += totalamount;
                    //                        }
                    //                        Balance += it.Balance;
                    //                    }).ToList().Count();

                    //                    sumModel = new IncomeTemplateModel
                    //                    {
                    //                        hour_income = hour_income,
                    //                        agent_income = agent_income,
                    //                        tip_income = tip_income,
                    //                        test_income = test_income,
                    //                        Balance = Balance
                    //                    };
                    #endregion
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "主播财务报表分页信息", ex.Message, ex.StackTrace);
            }
            return res;
        }
        /// <summary>
        /// 主播打赏礼物 分页信息
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public List<TipEntity> GetFlowDetailsPage(PageParm parm, ref int totalCount, ref decimal sumTotalAmount)
        {
            var res = new List<TipEntity>();
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
                using (var db = GetSqlSugarDB(DbConnType.QPAnchorRecordDB))
                {
                    var query = db.Queryable<TipEntity, SysAnchor, SysShopAnchorEntity>((it, st, ot) => new object[] { JoinType.Left, it.AnchorID == st.id, JoinType.Left, st.id == ot.AnchorID })
                          .Where(it => it.sendtime >= Convert.ToDateTime(dic["startTime"]) && it.sendtime < Convert.ToDateTime(dic["endTime"]))
                          .WhereIF(dic.ContainsKey("userName") && !string.IsNullOrEmpty(dic["userName"].ToString()), (it, st) => st.anchorName.Contains(dic["userName"].ToString()) || st.nickName.Contains(dic["userName"].ToString()))
                          .WhereIF(dic.ContainsKey("RewardName") && !string.IsNullOrEmpty(dic["RewardName"].ToString()), (it, st) => it.username.Contains(dic["RewardName"].ToString()) || it.userNickname.Contains(dic["RewardName"].ToString()))
                          .WhereIF(dic.ContainsKey("Type") && Convert.ToInt32(dic["Type"].ToString()) != -1, (it, st) => it.Type == Convert.ToInt32(dic["Type"].ToString()))
                           .WhereIF(dic.ContainsKey("ShopID") && Convert.ToInt32(dic["ShopID"]) != -1, (it, st, ot) => ot.ShopID == Convert.ToInt32(dic["ShopID"]))
                          ;
                    sumTotalAmount = query.Clone().WithCache(60).Sum(it => it.totalamount);
                    res = query
                          .Select((it, st) => new TipEntity
                          {
                              orderno = it.orderno,
                              giftname = it.giftname,
                              price = it.price,
                              quantity = it.quantity,
                              Type = it.Type,
                              totalamount = it.totalamount,
                              username = it.username,
                              sendtime = it.sendtime,
                              AnchorName = st.anchorName,
                              AnchorNickName = st.nickName,
                              userNickname = it.userNickname
                          })
                          .WithCache(60)
                         .OrderBy(" it.sendtime desc")
                         .ToPageList(parm.page, parm.limit, ref totalCount);
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "主播打赏礼物 分页信息", ex.Message, ex.StackTrace);
            }
            return res;
        }
        /// <summary>
        /// 主播工时 分页信息
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public List<HourModel> GetHourDetailsPage(PageParm parm, ref int totalCount, ref decimal sumAmount, ref decimal sumDuration)
        {
            var result = new List<HourModel>();
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
                using (var db = GetSqlSugarDB(DbConnType.QPVideoAnchorDB))
                {
                    var query = db.Queryable<SysAnchorLiveRecordEntity, SysAnchor, SysShopAnchorEntity>((it, st, ot) => new object[] {
                        JoinType.Left, it.aid == st.id,
                        JoinType.Left, st.id == ot.AnchorID
                    })
                                   .Where(it => it.ontime >= Convert.ToDateTime(dic["startTime"]) && it.ontime < Convert.ToDateTime(dic["endTime"]))
                                   .WhereIF(dic.ContainsKey("seqid") && !string.IsNullOrEmpty(dic["seqid"].ToString()), it => it.seqid == dic["seqid"].ToString())
                                   .WhereIF(dic.ContainsKey("isLive") && Convert.ToInt32(dic["isLive"]) == 1, it => SqlFunc.IsNullOrEmpty(it.uptime))
                                   .WhereIF(dic.ContainsKey("isLive") && Convert.ToInt32(dic["isLive"]) == 0, it => !SqlFunc.IsNullOrEmpty(it.uptime))
                                   .WhereIF(dic.ContainsKey("Name") && !string.IsNullOrEmpty(dic["Name"].ToString()), (it, st) => st.anchorName.Contains(dic["Name"].ToString()) || st.nickName.Contains(dic["Name"].ToString()))
                                   .WhereIF(dic.ContainsKey("ShopID") && Convert.ToInt32(dic["ShopID"]) != -1, (it, st, ot) => ot.ShopID == Convert.ToInt32(dic["ShopID"]))
                                   ;//缓存30秒
                    var sumReuslt = query.Clone().Select((it, st) => new { amount = SqlFunc.AggregateSum(it.amount), duration = SqlFunc.AggregateSum(it.livetime) }).WithCache(30).First();
                    sumAmount = sumReuslt.amount;
                    sumDuration = sumReuslt.duration;
                    return query
                          .Select((it, st, ot) => new HourModel
                          {
                              seqid = it.seqid,
                              AnchorName = st.anchorName,
                              NickName = st.nickName,
                              begintime = it.ontime,
                              endtime = it.uptime,
                              duration = it.livetime,
                              islive = SqlFunc.IIF(SqlFunc.IsNullOrEmpty(it.uptime), 1, 0),
                              flvurl = it.flvurl,
                              status = it.status
                          })
                          .WithCache(30)
                          .OrderBy(" it.ontime desc")
                          .ToPageList(parm.page, parm.limit, ref totalCount);
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "主播工时 分页信息", ex.Message, ex.StackTrace);
            }
            return result;
        }
        /// <summary>
        /// 统计主播的 工时收益
        /// </summary>
        public bool StatisticsAnchorWorkHourIncome(DateTime startTime, DateTime endTime)
        {
            using (var db = GetSqlSugarDB(DbConnType.QPAgentAnchorDB))
            {
                try
                {
                    db.Ado.BeginTran();
                    var list = db.Queryable<SysAnchorRebateEntity, SysAnchorLiveRecordEntity>((it, st) => new object[] { JoinType.Left, it.AnchorID == st.aid })
                          .Where((it, st) => it.IsWorkHours == 1 && st.ontime >= startTime && st.ontime < endTime && st.status == 1)
                          .GroupBy((it, st) => new { it.AnchorID, it.LiveTime, it.Salary, it.HourRebate, it.parentID,it.GiftAmount })
                          .Having((it, st) => SqlFunc.AggregateSum(st.livetime) >= it.LiveTime * 60 &&
                           SqlFunc.Subqueryable<TipEntity>().Where(gt => gt.sendtime >= startTime && gt.sendtime < SqlFunc.AggregateMax(st.uptime).AddMinutes(3)).Where(gt => gt.AnchorID == it.AnchorID).Sum(gt => gt.totalamount) >= it.GiftAmount)
                          .Select((it, st) => new SysTipIncomeDetailEntity
                          {
                              ShopID = 0,
                              UserID = it.parentID,
                              AnchorID = it.AnchorID,
                              StartDate = startTime,
                              UserRebate = it.HourRebate,
                              CreateTime = startTime,
                              TipType = TipTypeEnum.其他,
                              IncomeType = IncomeTypeEnum.工时,
                              AnchorIncome = it.Salary * (100 - it.HourRebate) / 100,
                              UserIncome = it.Salary * it.HourRebate / 100
                          })
                          .ToList();
                    if (list.Count == 0)
                    {
                        return true;
                    }
                    //批量插入工时收益明细
                    db.Insertable(list).ExecuteCommand();
                    List<int> anchorIDList = list.Select(it => it.AnchorID).ToList();
                    //更新工时记录 无效字段
                    db.Updateable<SysAnchorLiveRecordEntity>().SetColumns(it => new SysAnchorLiveRecordEntity { status = 0 })
                      .Where(it => anchorIDList.Contains(it.aid) && it.ontime >= startTime && it.ontime < endTime)
                      .ExecuteCommand();
                    // 处理总收益报表
                    var incomeList = db.Queryable<SysIncomeEntity>().Where(it => it.opdate == startTime.Date).ToList();
                    var updateIncomeList = new List<SysIncomeEntity>();//更新集合
                    var addIncomeList = new List<SysIncomeEntity>();//新增集合
                    list.ForEach(it =>
                    {
                        //判读对应日期部分是否有该主播数据 有就更新
                        var updateModel = incomeList.Where(st => st.AnchorID == it.AnchorID).FirstOrDefault();
                        if (updateModel != null)//存在
                        {
                            updateModel.hour_income = it.AnchorIncome;
                            updateModel.agentHour_income = it.UserIncome;
                            updateIncomeList.Add(updateModel);
                        }
                        else
                        {
                            addIncomeList.Add(new SysIncomeEntity
                            {
                                AnchorID = it.AnchorID,
                                opdate = startTime.Date,
                                hour_income = it.AnchorIncome,
                                agentHour_income = it.UserIncome
                            });
                        }
                    });
                    if (addIncomeList.Count > 0)
                    {
                        db.Insertable(addIncomeList).ExecuteCommand();
                    }
                    if (updateIncomeList.Count > 0)
                    {
                        db.Updateable(updateIncomeList).UpdateColumns(it => new { it.hour_income, it.agentHour_income }).ExecuteCommand();
                    }
                    //更新代理余额
                    var agentBalance = list.GroupBy(s => new { s.UserID }).Select(group => new SysUser
                    {
                        Id = group.Key.UserID,
                        Balance = group.Sum(p => p.UserIncome),
                    }).ToList();
                    agentBalance.ForEach(it =>
                    {
                        db.Updateable<SysUser>().SetColumns(gt => new SysUser { Balance = gt.Balance + it.Balance }).Where(gt => gt.Id == it.Id).ExecuteCommand();
                    });
                    //更新主播余额
                    var anchorBalance = list.GroupBy(s => new { s.AnchorID }).Select(group => new SysAnchorInfoEntity
                    {
                        aid = group.Key.AnchorID,
                        agentGold = group.Sum(p => p.AnchorIncome),
                    }).ToList();
                    anchorBalance.ForEach(it =>
                    {
                        db.Updateable<SysAnchorInfoEntity>().SetColumns(gt => new SysAnchorInfoEntity { agentGold = gt.agentGold + it.agentGold })
                        .Where(gt => gt.aid == it.aid).ExecuteCommand();
                    });
                    db.Ado.CommitTran();
                    return true;
                }
                catch (Exception ex)
                {
                    db.Ado.RollbackTran();
                    LogHelper.WriteLogTips("按天统计工时收益异常：开始时间：" + startTime + ",结束时间：" + endTime + "。错误信息：" + ex.Message + "------" + ex.StackTrace);
                    return false;
                }
            }
        }
        /// <summary>
        /// 重新计算时长
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UpdateWorkDuration(string id)
        {
            try
            {
                using (var db = GetSqlSugarDB(DbConnType.QPVideoAnchorDB))
                {
                    var model = db.Queryable<SysAnchorLiveRecordEntity>().Where(it => it.seqid == id).First();
                    return db.Updateable<SysAnchorLiveRecordEntity>().SetColumns(it => new SysAnchorLiveRecordEntity { livetime = (decimal)model.uptime.Subtract(model.ontime).TotalMinutes }).Where(it => it.seqid == id).ExecuteCommand() > 0;
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "重新计算时长", ex.Message, ex.StackTrace);
            }
            return false;
        }
        /// <summary>
        /// 编辑工时记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UpdateWorkHours(SysAnchorLiveRecordEntity model)
        {
            var result = 0;
            using (var db = GetSqlSugarDB(DbConnType.QPVideoAnchorDB))
            {
                try
                {
                    db.Ado.BeginTran();
                    var opdate = model.ontime.Date;
                    var aid = db.Queryable<SysAnchorLiveRecordEntity>().Where(it => it.seqid == model.seqid).Select(it => it.aid).First();
                    decimal liveTime = (decimal)model.uptime.Subtract(model.ontime).TotalMinutes;
                    result = db.Updateable<SysAnchorLiveRecordEntity>().SetColumns(it => new SysAnchorLiveRecordEntity { livetime = liveTime, ontime = model.ontime, uptime = model.uptime }).Where(it => it.seqid == model.seqid).ExecuteCommand();
                    //判断报表数据是否存在
                    var incomeModel = db.Queryable<SysIncomeEntity>().Where(it => it.AnchorID == aid && it.opdate == opdate).First();
                    //获取修改时间当天的 直播时长和
                    decimal sumLiveTime = db.Queryable<SysAnchorLiveRecordEntity>()
                                            .Where(it => it.aid == aid && it.ontime >= opdate && it.ontime < opdate.AddDays(1))
                                            .Select(it => SqlFunc.AggregateSum(it.livetime)).First();
                    if (incomeModel == null)
                    {
                        db.Insertable(new SysIncomeEntity
                        {
                            AnchorID = aid,
                            opdate = opdate,
                            livetime = sumLiveTime,
                        }).ExecuteCommand();
                    }
                    else
                    {
                        incomeModel.livetime = sumLiveTime;
                        db.Updateable(incomeModel).UpdateColumns(it => new { it.livetime }).ExecuteCommand();
                    }
                    db.Ado.CommitTran();
                }
                catch (Exception ex)
                {
                    db.Ado.RollbackTran();
                    new LogLogic().Write(Level.Error, "编辑工时记录", ex.Message, ex.StackTrace);
                }
            }
            return result > 0;
        }

        /// <summary>
        /// 主播名称下拉框
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public string AnchorUserNameSelect()
        {
            var result = "";
            try
            {
                using (var db = GetInstance())
                {
                    result = db.Queryable<SysAnchor>()
                                .Select((it) => new
                                {
                                    nickName = string.IsNullOrEmpty(it.nickName) ? it.anchorName : it.nickName,
                                    userName = it.anchorName,
                                }).ToJson();
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "主播名称下拉框", ex.Message, ex.StackTrace);
            }
            return result;
        }
        /// <summary>
        /// 主播名称下拉框 ID和昵称
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public string AnchorUserIDSelect()
        {
            var result = "";
            try
            {
                using (var db = GetInstance())
                {
                    result = db.Queryable<SysAnchor>()
                                .Select((it) => new
                                {
                                    nickName = string.IsNullOrEmpty(it.nickName) ? it.anchorName : it.nickName,
                                    it.id,
                                }).ToJson();
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "主播名称下拉框", ex.Message, ex.StackTrace);
            }
            return result;
        }
        /// <summary>
        /// 删除用户的主播
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public int Delete(string[] primaryKeys)
        {
            using (var db = GetInstance())
            {
                try
                {
                    db.Ado.BeginTran();
                    foreach (string primaryKey in primaryKeys)
                    {
                        db.Deleteable<SysUserAnchor>().Where(it => it.UserID == primaryKey).ExecuteCommand();
                    }
                    db.Ado.CommitTran();
                    return 1;
                }
                catch (Exception ex)
                {
                    db.Ado.RollbackTran();
                    return 0;
                }
            }
        }
        /// <summary>
        /// 获取公司代码集合
        /// </summary>
        /// <returns></returns>
        public string selectCompanyCodeList()
        {
            using (var db = GetInstance())
            {
                return db.UnionAll(db.Queryable<CompanyEntity>()
                        .Select((it) => new SelectModel
                        {
                            Name = it.code,
                            ID = it.id,
                        }),
                        db.Queryable<SubCompanyCodeEntity>()
                        .Select((st) => new SelectModel
                        {
                            Name = st.code,
                            ID = st.id,
                        })).ToJson();
            }
        }
        /// <summary>
        /// 验证主播是否存在
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public SysAnchor CheckAnchorName(string account)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysAnchor>().Where((A) => A.anchorName == account).Select(A => new SysAnchor
                {
                    id = A.id,
                }).First();
            }
        }
        /// <summary>
        /// 获取主播余额
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public SysAnchorInfoEntity GetAnchorBalance(int id)
        {
            using (var db = GetSqlSugarDB(DbConnType.QPVideoAnchorDB))
            {
                return db.Queryable<SysAnchorInfoEntity>().Where((A) => A.aid == id).Select(A => new SysAnchorInfoEntity
                {
                    aid = A.aid,
                    agentGold = A.agentGold
                }).First();
            }
        }
    }
}
