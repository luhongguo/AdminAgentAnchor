using Elight.Logic.Base;
using Elight.Logic.Sys;
using Elight.Utility.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using Elight.Entity.Sys;
using Anchor.StackExchange.Redis.Helper;
using Elight.Entity.Model;
using Elight.Utility;
using Newtonsoft.Json.Linq;
using Elight.Entity;

namespace TimedTasksService
{
    public partial class TipService
    {
        /// <summary>
        /// 统计主播的 礼物收益
        /// </summary>
        public static void StatisticsAgentTipIncome(DateTime time)
        {
            try
            {
                DateTime dt = time.AddDays(-1);
                int StartDate = int.Parse(dt.ToString("yyyyMMdd"));//处理时间
                var flag = false;//标识是否已经处理返点收益  主播返点 返的是经纪人的收益，经纪人返点返的是平台的收益
                using (var db = sugarClient.GetSqlSugarDB(sugarClient.DbConnType.QPAnchorRecordDB))
                {
                    var model = db.Queryable<SysTipIncomeDetailEntity>().Where(it => it.StartDate == StartDate).First();
                    if (model == null)
                    {
                        flag = true;//未处理
                    }
                    if (flag)
                    {
                        List<TipEntity> updateTipList = new List<TipEntity>();//采集礼物的id集合
                        List<SysTipIncomeDetailEntity> list = db.Queryable<TipEntity, SysAnchorRebateEntity, SysRebateEntity>((at, st, ct) =>
                         new object[] {
                            JoinType.Left, at.AnchorID==st.AnchorID,
                            JoinType.Left,st.parentID==ct.UserID
                         })
                               .Where((at, st, ct) => at.sendtime >= Convert.ToDateTime(dt.ToString("yyyy-MM-dd")) && at.sendtime < Convert.ToDateTime(time.ToString("yyyy-MM-dd")))//前1天数据
                               .Where((at, st, ct) => at.status == 1)
                               .Select((at, st, ct) => new SysTipIncomeDetailEntity
                               {
                                   id = at.id,
                                   ShopID = 0,
                                   UserID = ct.UserID,
                                   AnchorID = at.AnchorID,
                                   orderno = at.orderno,
                                   StartDate = StartDate,
                                   totalamount = at.totalamount,
                                   UserRebate = st.TipRebate,
                                   PlatformRebate = ct.TipRebate,
                               })
                               .Mapper((it) =>
                               {
                                   updateTipList.Add(new TipEntity { id = it.id, status = 0 });
                                   it.PlatformIncome = it.totalamount * it.PlatformRebate / 100;//平台收益
                                   it.UserIncome = (it.totalamount - it.PlatformIncome) * it.UserRebate / 100;//经纪人收益=总金额减去平台收益 * 主播返点
                                   it.AnchorIncome = (it.totalamount - it.PlatformIncome) * (100 - it.UserRebate) / 100;//主播收益=总金额减去平台收益 * （100-主播返点）
                               })
                               .ToList();
                        db.Updateable(updateTipList).ExecuteCommand();//id是礼物表id 批量更新状态
                        db.Insertable(list).ExecuteCommand();

                    }
                }
                Console.WriteLine("按天统计代理的礼物收益：" + time);
            }
            catch (Exception ex)
            {
                //统一记录日志
                Console.WriteLine("按天统计代理的礼物收益异常：" + ex.Message + "------" + ex.StackTrace);
                new LogLogic().Write(new SysLog
                {
                    Operation = "按天统计代理的礼物收益",
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    CreateTime = DateTime.Now
                });
            }
        }
        /// <summary>
        /// 打赏礼物采集
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="pageSize">采集条数</param>
        public static void StatisticsCollectTipGifts(DateTime time,int pageSize)
        {
            try
            {
                ////设置成5分钟后重启（为了防止下面的逻辑执行速度不快时，延长的时间，一旦执行完成，将会把当前的执行周期改为正常的值）
                //(sender as System.Timers.Timer).Interval = 5 * 60 * 1000;

                //实例化Redis
                RedisHelper redis = new RedisHelper(6);//只记录最大的时间

                int totalCount = 0;
                string key = "last_tips_max_datetime";

                DateTime startTime = time.AddHours(-12);//第一次执行获取前面12个小时的数据
                if (redis.KeyExists(key))
                {
                    startTime = redis.StringGet<DateTime>(key).AddSeconds(-2);//读取上次采集数据的最大时间 往前加2秒重新采集（怕漏单）
                }
                DateTime endTime = DateTime.Now;//当前时间即可
                //List<Model.dt_anchor> dt_Anchors = new BLL.dt_anchor().GetModelList("");
                //获取所有的礼物数据
                List<GiftEntity> gifts = new List<GiftEntity>();
                if (redis.KeyExists("anchor_gift_"))
                {
                    gifts = redis.StringGet<List<GiftEntity>>("anchor_gift_");
                }
                else
                {
                    using (var db = sugarClient.GetSqlSugarDB(sugarClient.DbConnType.QPVideoAnchorDB))
                    {
                        gifts = db.Queryable<GiftEntity>().ToList();
                        redis.StringSet("anchor_gift_", gifts, TimeSpan.FromMinutes(5));
                    }
                }
                PagingCollect(gifts, pageSize, startTime, endTime, out totalCount, out endTime);

                //将最后获取到的最大时间写入
                redis.StringSet<DateTime>(key, endTime);
            }
            catch (Exception ex)
            {
                new LogLogic().Write(new SysLog
                {
                    Operation = "打赏礼物采集",
                    Message = ex.Message,
                    StackTrace = ex.StackTrace.Length > 500 ? ex.StackTrace.Substring(0, 500) : ex.StackTrace,
                    CreateTime = DateTime.Now
                });
            }
        }
        public static void PagingCollect(List<GiftEntity> gifts,int pageSize, DateTime startTime, DateTime endTime, out int totalCount, out DateTime maxDateTime)
        {
            try
            {
                //实例化Redis
                RedisHelper redisTips = new RedisHelper(6);//只记录打赏的订单号
                string api_url = System.Configuration.ConfigurationManager.AppSettings["tips_data_api"];
                //using (var db = sugarClient.GetSqlSugarDB(sugarClient.DbConnType.QPVideoAnchorDB))  测试
                //{
                //    var addList = db.SqlQueryable<TipEntity>($@"select 75 as AnchorID,orderno,giftname,price,quantity,ratio, totalamount,username,sendtime  from tip_zb001").ToList();
                //    //找出数据库中已经存在的订单号
                //    var checkOrdernoList = db.Queryable<TipEntity>().In(it => it.orderno, addList.Select(it=>it.orderno)).Select(it => it.orderno).ToList();
                //    if (checkOrdernoList != null && checkOrdernoList.Count > 0)
                //    {
                //        //新增集合中移除已经存在的订单号
                //        addList.RemoveAll(it => checkOrdernoList.Contains(it.orderno));
                //    }
                //    db.Insertable(addList).ExecuteCommand();
                //}
                //时间段内的n条数据，
                string result = Utils.HttpGet(api_url + string.Format("?pageIndex={0}&pageSize={1}&startTime={2}&endTime={3}",
                     1, pageSize, startTime, endTime), "Bearer QW5jaG9yX01vtaunclo66M2ak1odXFSSW54YUQ=");
                //将json转换为JObject  
                JObject jObj = JObject.Parse(result);
                var s = jObj["et"]["data"].Children().ToList();
                if (Convert.ToInt32(jObj["code"]) == 20000 && jObj["et"]["data"].Children().Count() > 0)
                {
                    var q = (from p in jObj["et"]["data"].Children()
                             select new
                             {
                                 orderId = p["orderId"].ToString(),
                                 account = p["account"].ToString(),
                                 company = p["company"].ToString(),
                                 aid = int.Parse(p["anchorId"].ToString()),//主播ID
                                 amount = decimal.Parse(p["amount"].ToString()),
                                 num = int.Parse(p["number"].ToString()),//礼物数量
                                 Type = int.Parse(p["actionType"].ToString()),//1是打赏，2是房间,3计时
                                 code = p["code"].ToString(),//礼物编码
                                 orderCreateTime = Utils.StrToDateTime(p["creatDate"].ToString())
                             }).ToList();

                    //最大时间
                    maxDateTime = q.Max(p => p.orderCreateTime);
                    string giftName = string.Empty;
                    List<TipEntity> addList = new List<TipEntity>();
                    List<string> ordernoList = new List<string>();//订单号集合
                    foreach (var item in q)
                    {
                        var g = from p in gifts
                                where p.code == item.code
                                select new
                                {
                                    p.name,
                                    p.price
                                };

                        if (g != null && g.Count() > 0)
                            giftName = g.FirstOrDefault().name;
                        else
                            giftName = item.code;
                        TipEntity model = new TipEntity();
                        model.AnchorID = item.aid;
                        model.isconfirm = 1;
                        model.issettle = 0;
                        model.status = 1;
                        model.username = item.account;
                        model.companycode = item.company;
                        model.companystyle = item.company;
                        model.description = giftName;
                        model.gift = item.code;
                        model.giftname = giftName;
                        model.istest = 0;
                        model.orderno = item.orderId;
                        model.price = item.amount;
                        model.quantity = item.num;
                        model.sendtime = item.orderCreateTime;
                        model.confirmtime = item.orderCreateTime;
                        model.totalamount = (decimal)(item.num * item.amount);// * (model.ratio / (decimal)100.0); 暂时没有 比率
                        model.Type = item.Type;
                        //redis中不存在单号 就添加到新增集合中
                        if (!redisTips.KeyExists(model.orderno))
                        {
                            addList.Add(model);
                            ordernoList.Add(model.orderno);
                        }
                    }
                    //写入前，要判定是否存在
                    using (var db = sugarClient.GetSqlSugarDB(sugarClient.DbConnType.QPVideoAnchorDB))
                    {
                        //找出数据库中已经存在的订单号
                        var checkOrdernoList = db.Queryable<TipEntity>().In(it => it.orderno, ordernoList).Select(it => it.orderno).ToList();
                        if (checkOrdernoList != null && checkOrdernoList.Count > 0)
                        {
                            //新增集合中移除已经存在的订单号
                            addList.RemoveAll(it => checkOrdernoList.Contains(it.orderno));

                        }
                        int count = db.Insertable(addList).ExecuteCommand();
                    }
                    ordernoList.ForEach(it =>
                    {
                        redisTips.StringSet<string>(it, it, TimeSpan.FromHours(48));//保存2天的数据
                    });
                    totalCount = int.Parse(jObj["et"]["count"].ToString());
                    return;
                }
                totalCount = 0;
                maxDateTime = DateTime.Now;
                return;
            }
            catch (Exception ex)
            {
                new LogLogic().Write(new SysLog
                {
                    Operation = "打赏礼物采集",
                    Message = ex.Message,
                    StackTrace = ex.StackTrace.Length > 500 ? ex.StackTrace.Substring(0, 500) : ex.StackTrace,
                    CreateTime = DateTime.Now
                });
                totalCount = 0;
                maxDateTime = DateTime.Now;
                return;
            }
        }
    }
}
