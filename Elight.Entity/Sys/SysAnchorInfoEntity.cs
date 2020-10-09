using Elight.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace Elight.Entity.Sys
{
    /// <summary>
    /// 主播补充信息
    /// </summary>
    [SugarTable("AnchorDB.dbo.anchor_info")]
    public class SysAnchorInfoEntity
    {
        /// <summary>
        /// 主播id
        /// </summary>
        public int aid { get; set; }
        /// <summary>
        /// 直播线路id
        /// </summary>
        public int liveroad { get; set; }
        public string label { get; set; }
        /// <summary>
        /// 直播状态
        /// </summary>
        public AnchorStatus status { get; set; }
        /// <summary>
        /// 关注人数
        /// </summary>
        public int follow { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public decimal gold { get; set; }
        /// <summary>
        /// 在线人数
        /// </summary>
        public int online { get; set; }
        /// <summary>
        /// 开关  0 关  1开  默认关闭
        /// </summary>
        public int contactswtich { get; set; }
        /// <summary>
        /// 显示价格
        /// </summary>
        public decimal showprice { get; set; }
        /// <summary>
        /// WX
        /// </summary>
        public string wxchat { get; set; }
        /// <summary>
        /// QQ
        /// </summary>
        public string qqchat { get; set; }
        /// <summary>
        /// 土豆
        /// </summary>
        public string tdchat { get; set; }
        /// <summary>
        /// TG
        /// </summary>
        public string tgchat { get; set; }
        /// <summary>
        /// 代理平台的主播余额
        /// </summary>
        public decimal agentGold { get; set; }
    }
}
