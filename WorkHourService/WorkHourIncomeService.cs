using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elight.Entity.Enum;
using Elight.Entity.Sys;
using Elight.Utility.Log;
using SqlSugar;

namespace WorkHourService
{
    public class WorkHourIncomeService
    {
        /// <summary>
        /// 统计主播的 工时收益
        /// </summary>
        public static void StatisticsAnchorWorkHourIncome(DateTime startTime, DateTime endTime)
        {
            using (var db = sugarClient.GetSqlSugarDB(sugarClient.DbConnType.QPAnchorRecordDB))
            {
                try
                {
                    db.Ado.BeginTran();
                    var list = db.Queryable<SysAnchorRebateEntity, SysAnchorLiveRecordEntity>((it, st) => new object[] { JoinType.Left, it.AnchorID == st.aid })
                          .Where((it, st) => it.IsWorkHours == 1 && st.ontime >= startTime && st.ontime < endTime && st.status == 1)
                          .GroupBy((it, st) => new { it.AnchorID, it.LiveTime, it.Salary, it.HourRebate, it.parentID, it.GiftAmount })
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
                          }).ToList();
                    if (list.Count == 0)
                    {
                        Console.WriteLine("统计主播的工时收益：执行时间：" + DateTime.Now + ",统计开始时间--" + startTime + ",统计结束时间：--" + endTime + "，统计数据+" + list.Count);
                        return;
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
                    Console.WriteLine("统计主播的工时收益：执行时间：" + DateTime.Now + ",统计开始时间--" + startTime + ",统计结束时间：--" + endTime + "，统计数据+" + list.Count);
                }
                catch (Exception ex)
                {
                    db.Ado.RollbackTran();
                    //统一记录日志
                    Console.WriteLine("按天统计工时收益异常：执行时间：" + DateTime.Now + ",统计开始时间--" + startTime + ",统计结束时间：--" + endTime + "。错误信息：" + ex.Message + "------" + ex.StackTrace);
                    LogHelper.WriteLogTips("按天统计工时收益异常：统计开始时间--" + startTime + ",统计结束时间：--" + endTime + "。错误信息：" + ex.Message + "------" + ex.StackTrace);
                }
            }
        }
        /// <summary>
        /// 统计主播的工时
        /// </summary>
        public static void StatisticsAnchorLiveTime(DateTime endTime)
        {
            using (var db = sugarClient.GetSqlSugarDB(sugarClient.DbConnType.QPAnchorRecordDB))
            {
                var startTime = endTime.AddHours(-24);
                try
                {
                    db.Ado.BeginTran();
                    var list = db.Queryable<SysAnchorLiveRecordEntity>().Where(it => it.ontime >= startTime && it.ontime < endTime)
                                 .GroupBy(it => it.aid)
                                 .Select(it => new SysIncomeEntity
                                 {
                                     AnchorID = it.aid,
                                     livetime = SqlFunc.AggregateSum(it.livetime),
                                     opdate = startTime.Date,
                                 })
                                 .ToList();
                    if (list.Count == 0)
                    {
                        Console.WriteLine("统计主播的直播时长：执行时间：" + DateTime.Now + ",统计开始时间--" + startTime + ",统计结束时间：--" + endTime + "，统计数据+" + list.Count);
                        return;
                    }
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
                            updateModel.livetime = it.livetime;
                            updateIncomeList.Add(updateModel);
                        }
                        else
                        {
                            addIncomeList.Add(it);
                        }
                    });
                    if (addIncomeList.Count > 0)
                    {
                        db.Insertable(addIncomeList).ExecuteCommand();
                    }
                    if (updateIncomeList.Count > 0)
                    {
                        db.Updateable(updateIncomeList).UpdateColumns(it => new { it.livetime }).ExecuteCommand();
                    }
                    db.Ado.CommitTran();
                    Console.WriteLine("统计主播的直播时长：执行时间：" + DateTime.Now + ",统计开始时间--" + startTime + ",统计结束时间：--" + endTime + "，统计数据+" + list.Count);
                }
                catch (Exception ex)
                {
                    db.Ado.RollbackTran();
                    //统一记录日志
                    Console.WriteLine("统计主播的直播时长异常：执行时间：" + DateTime.Now + ",统计开始时间--" + startTime + ",统计结束时间：--" + endTime + "。错误信息：" + ex.Message + "------" + ex.StackTrace);
                    LogHelper.WriteLogTips("统计主播的直播时长异常：统计开始时间--" + startTime + ",统计结束时间：--" + endTime + "。错误信息：" + ex.Message + "------" + ex.StackTrace);
                }
            }
        }
    }
}
