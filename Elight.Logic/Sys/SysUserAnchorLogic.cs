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
        /// 超管获得经纪人拥有的主播 列表分页
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
                    result = db.Queryable<SysUserAnchor, SysAnchor>((st, it) => new object[] { JoinType.Left, st.AnchorID == it.id })
                                 .WhereIF(dic.ContainsKey("anchorUserName") && !string.IsNullOrEmpty(dic["anchorUserName"].ToString()), (st, it) => it.username.Contains(dic["anchorUserName"].ToString()) || it.nickname.Contains(dic["anchorUserName"].ToString()))
                                 .WhereIF(dic.ContainsKey("userID") && !string.IsNullOrEmpty(dic["userID"].ToString()), (st, it) => st.UserID == dic["userID"].ToString())
                                 .WhereIF(dic.ContainsKey("isCollet") && Convert.ToInt32(dic["isCollet"]) != -1, (st, it) => it.isCollet == Convert.ToInt32(dic["isCollet"]))
                                 .WhereIF(dic.ContainsKey("isColletCode") && dic["isColletCode"].ToString() != "-1", (st, it) => it.isColletCode == dic["isColletCode"].ToString())
                                 .Select((st, it) => new SysAnchor
                                 {
                                     id = it.id,
                                     username = it.username,
                                     nickname = it.nickname,
                                     photo = it.photo,
                                     balance = it.balance,
                                     atteCount = it.atteCount,
                                     ishot = it.ishot,
                                     isrecommend = it.isrecommend,
                                     regtime = it.regtime,
                                     viplevel = it.viplevel,
                                     birthday = it.birthday,
                                 }).ToPageList(pageIndex, pageSize, ref totalCount);
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
                    result = db.Queryable<SysAnchor>()
                                 .WhereIF(dic.ContainsKey("anchorUserName") && !string.IsNullOrEmpty(dic["anchorUserName"].ToString()), it => it.username.Contains(dic["anchorUserName"].ToString()) || it.nickname.Contains(dic["anchorUserName"].ToString()))
                                 .WhereIF(dic.ContainsKey("isCollet") && Convert.ToInt32(dic["isCollet"]) != -1, (it) => it.isCollet == Convert.ToInt32(dic["isCollet"]))
                                 .WhereIF(dic.ContainsKey("isColletCode") && dic["isColletCode"].ToString() != "-1", (it) => it.isColletCode == dic["isColletCode"].ToString())
                                 .Where(it => SqlFunc.Subqueryable<SysUserAnchor>().Where(st => st.UserID == dic["userID"].ToString()).Where(st => st.AnchorID == it.id).NotAny())
                                 .Select(it => new SysAnchor
                                 {
                                     id = it.id,
                                     username = it.username,
                                     nickname = it.nickname,
                                     photo = it.photo,
                                     balance = it.balance,
                                     atteCount = it.atteCount,
                                     ishot = it.ishot,
                                     isrecommend = it.isrecommend,
                                     regtime = it.regtime,
                                     viplevel = it.viplevel,
                                     birthday = it.birthday,
                                 }).ToPageList(pageIndex, pageSize, ref totalCount);
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
                new LogLogic().Write(Level.Error, "添加主播给经纪人", ex.Message, ex.StackTrace);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 超管查看自己的主播信息
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
                    result = db.Queryable<SysAnchor>()
                                .WhereIF(dic.ContainsKey("Name") && !string.IsNullOrEmpty(dic["Name"].ToString()), (it) => it.username.Contains(dic["Name"].ToString()) || it.nickname.Contains(dic["Name"].ToString()))
                                .WhereIF(dic.ContainsKey("startTime") && !string.IsNullOrEmpty(dic["startTime"].ToString()) && !string.IsNullOrEmpty(dic["endTime"].ToString()), (it) => it.regtime >= Convert.ToDateTime(dic["startTime"]) && it.regtime <= Convert.ToDateTime(dic["endTime"]))
                                .WhereIF(dic.ContainsKey("isCollet") && Convert.ToInt32(dic["isCollet"]) != -1, (it) => it.isCollet == Convert.ToInt32(dic["isCollet"]))
                                .WhereIF(dic.ContainsKey("isColletCode") && dic["isColletCode"].ToString() != "-1", (it) => it.isColletCode == dic["isColletCode"].ToString())
                                .Select((it) => new SysAnchor
                                {
                                    id = it.id,
                                    username = it.username,
                                    nickname = it.nickname,
                                    photo = it.photo,
                                    balance = it.balance,
                                    atteCount = it.atteCount,
                                    ishot = it.ishot,
                                    isrecommend = it.isrecommend,
                                    regtime = it.regtime,
                                    viplevel = it.viplevel,
                                    birthday = it.birthday,
                                    lmstatus = it.lmstatus,
                                    isCollet = it.isCollet
                                }).ToPageList(parm.page, parm.limit, ref totalCount);
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "经纪人查看主播信息", ex.Message, ex.StackTrace);
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
                using (var db = GetInstance())
                {
                    decimal hour_income = 0;
                    decimal agent_income = 0;
                    decimal tip_income = 0;
                    decimal test_income = 0;
                    decimal Balance = 0;
                    var tableList = GetSqlSugarDB(DbConnType.QPAnchorRecordDB).DbMaintenance.GetTableInfoList();//获取数据库所有表名
                    var query = db.Queryable<SysAnchor>()
                            .WhereIF(dic.ContainsKey("Name") && !string.IsNullOrEmpty(dic["Name"].ToString()), (st) => st.username.Contains(dic["Name"].ToString()) || st.nickname.Contains(dic["Name"].ToString()))
                            .WhereIF(dic.ContainsKey("isCollet") && Convert.ToInt32(dic["isCollet"]) != -1, (it) => it.isCollet == Convert.ToInt32(dic["isCollet"]))
                            .WhereIF(dic.ContainsKey("isColletCode") && dic["isColletCode"].ToString() != "-1", (it) => it.isColletCode == dic["isColletCode"].ToString())
                            .Select((st) => new IncomeTemplateModel
                            {
                                AnchorID = st.id,
                                AnchorName = st.username,
                                NickName = st.nickname,
                                Balance = st.balance,
                                isCollet = st.isCollet,
                            }).WithCache(120);//缓存120秒

                    res = query.Clone()
                        .Mapper((it, cache) =>
                              {
                                  if (tableList.Any(st => st.Name.Equals($@"income_{it.AnchorName}")))//判断表是否存在
                                      {
                                          //isnull(sum(tip_income),0) tip_income,   
                                          var table = db.SqlQueryable<IncomeTemplateModel>($@"select  isnull(sum(hour_income),0) hour_income,isnull(sum(agent_income),0) agent_income,isnull(sum(test_income),0) test_income  
from QPAnchorRecordDB.dbo.income_{it.AnchorName} where opdate>='{dic["startTime"].ToString()}' and opdate<'{dic["endTime"].ToString()}'")
                                                    .First();
                                      it.hour_income = table.hour_income;
                                      it.agent_income = table.agent_income;
                                      it.test_income = table.test_income;
                                  }
                                  if (tableList.Any(st => st.Name.Equals($@"tip_{it.AnchorName}")))//判断表是否存在
                                      {
                                          //礼物金额
                                          var totalamount = db.SqlQueryable<TipTemplateModel>($@"select sum(totalamount) as totalamount from QPAnchorRecordDB.dbo.tip_{it.AnchorName}
where sendtime>='{dic["startTime"].ToString()}' and sendtime<'{dic["endTime"].ToString()}'").First().totalamount;
                                      it.tip_income = totalamount;
                                  }
                              })
                        .ToList().OrderByDescending(it => it.tip_income).Skip((parm.page - 1) * parm.limit).Take(parm.limit).ToList();
                    //.ToPageList(parm.page,m, ref totalCount);

                    totalCount = query.Mapper((it, cache) =>//求和
                    {
                        if (tableList.Any(st => st.Name.Equals($@"income_{it.AnchorName}")))//判断表是否存在
                            {
                                //isnull(sum(tip_income),0) tip_income,   
                                var table = db.SqlQueryable<IncomeTemplateModel>($@"select  isnull(sum(hour_income),0) hour_income,isnull(sum(agent_income),0) agent_income,isnull(sum(test_income),0) test_income  
from QPAnchorRecordDB.dbo.income_{it.AnchorName} where opdate>='{dic["startTime"].ToString()}' and opdate<'{dic["endTime"].ToString()}'")
                                          .First();
                            hour_income += table.hour_income;
                            agent_income += table.agent_income;
                            test_income += table.test_income;
                        }
                        if (tableList.Any(st => st.Name.Equals($@"tip_{it.AnchorName}")))//判断表是否存在
                            {
                                //礼物金额
                                var totalamount = db.SqlQueryable<TipTemplateModel>($@"select sum(totalamount) as totalamount from QPAnchorRecordDB.dbo.tip_{it.AnchorName}
where sendtime>='{dic["startTime"].ToString()}' and sendtime<'{dic["endTime"].ToString()}'").First().totalamount;
                            tip_income += totalamount;
                        }
                        Balance += it.Balance;
                    }).ToList().Count();

                    sumModel = new IncomeTemplateModel
                    {
                        hour_income = hour_income,
                        agent_income = agent_income,
                        tip_income = tip_income,
                        test_income = test_income,
                        Balance = Balance
                    };
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
        public List<TipTemplateModel> GetFlowDetailsPage(PageParm parm, ref int totalCount, ref decimal sumTotalAmount)
        {
            var res = new List<TipTemplateModel>();
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
                    var query = db.SqlQueryable<TipTemplateModel>($@"select orderno,giftname,price,quantity,ratio, totalamount,username,sendtime  from tip_" + dic["userName"].ToString()
                               + $@" where sendtime>='{dic["startTime"].ToString()}' and sendtime<'{dic["endTime"].ToString()}'");
                    sumTotalAmount = query.Clone().Sum(it => it.totalamount);
                    res = query.ToPageList(parm.page, parm.limit, ref totalCount);
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
        public List<HourModel> GetHourDetailsPage(PageParm parm, ref int totalCount, ref decimal sumAmount, ref int sumDuration)
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
                    if (dic.ContainsKey("userName") && dic["userName"].ToString() == "-1")//选中全部
                    {
                        var query = db.Queryable<LiveCallbackHourEntity, SysAnchor>((it, st) => new object[] { JoinType.Left, it.stream_id == st.id })
                                   .Where(it => it.begintime >= Convert.ToDateTime(dic["startTime"]) && it.begintime < Convert.ToDateTime(dic["endTime"]))
                                   .WhereIF(dic.ContainsKey("isLive") && Convert.ToInt32(dic["isLive"]) != -1, it => it.islive == Convert.ToInt32(dic["isLive"]));
                        var sumReuslt = query.Clone().Select((it, st) => new { amount = SqlFunc.AggregateSum(it.amount), duration = SqlFunc.AggregateSum(it.duration) }).First();
                        sumAmount = sumReuslt.amount;
                        sumDuration = sumReuslt.duration;
                        return query
                              .Select((it, st) => new HourModel
                              {
                                  AnchorName = st.username,
                                  NickName = st.nickname,
                                  begintime = it.begintime,
                                  endtime = it.endtime,
                                  duration = it.duration,
                                  source = it.source,
                                  status = it.status,
                                  islive = it.islive
                              }).WithCache(30)//缓存30秒
                              .OrderBy(it => it.begintime, OrderByType.Desc)
                              .ToPageList(parm.page, parm.limit, ref totalCount);
                    }
                    else//查询单个用户
                    {
                        var query = db.Queryable<LiveCallbackHourEntity, SysAnchor>((it, st) => new object[] { JoinType.Left, it.stream_id == st.id })
                                 .Where(it => it.begintime >= Convert.ToDateTime(dic["startTime"]) && it.begintime < Convert.ToDateTime(dic["endTime"]))
                                 .Where(it => it.stream_id == Convert.ToInt32(dic["userName"]))
                                 .WhereIF(dic.ContainsKey("isLive") && Convert.ToInt32(dic["isLive"]) != -1, it => it.islive == Convert.ToInt32(dic["isLive"]));
                        var sumReuslt = query.Clone().Select((it, st) => new { amount = SqlFunc.AggregateSum(it.amount), duration = SqlFunc.AggregateSum(it.duration) }).First();
                        sumAmount = sumReuslt.amount;
                        sumDuration = sumReuslt.duration;
                        return query
                             .Select((it, st) => new HourModel
                             {
                                 AnchorName = st.username,
                                 NickName = st.nickname,
                                 begintime = it.begintime,
                                 endtime = it.endtime,
                                 duration = it.duration,
                                 source = it.source,
                                 status = it.status,
                                 amount = it.amount,
                                 islive = it.islive
                             }).WithCache(30)//缓存30秒
                            .OrderBy(it => it.begintime, OrderByType.Desc)
                            .ToPageList(parm.page, parm.limit, ref totalCount);
                    }
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "主播工时 分页信息", ex.Message, ex.StackTrace);
            }
            return result;
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
                                    nickName = string.IsNullOrEmpty(it.nickname) ? it.username : it.nickname,
                                    userName = it.username,
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
                                    nickName = string.IsNullOrEmpty(it.nickname) ? it.username : it.nickname,
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
    }
}
