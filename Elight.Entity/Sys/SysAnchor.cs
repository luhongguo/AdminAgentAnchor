using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Elight.Entity.Enum;

namespace Elight.Entity.Sys
{
    /// <summary>
    /// 主播基本信息
    /// </summary>
    [SugarTable("AnchorDB.dbo.anchor_basic")]
    public partial class SysAnchor : ModelContext
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主播名称
        /// </summary>
        public string anchorName { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string nickName { get; set; }
        /// <summary>
        /// 头像地址
        /// </summary>
        public string headUrl { get; set; }
        /// <summary>
        /// 封面图片
        /// </summary>
        public string avatarUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string bgUrl { get; set; }
        /// <summary>
        /// 性别0女
        /// </summary>
        public int sex { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(DateTimeToJson))]
        public DateTime createTime { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        [JsonConverter(typeof(DateTimeToJson))]
        public DateTime birthday { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public string height { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        public string weight { get; set; }
        /// <summary>
        /// 三围
        /// </summary>
        public string sanWei { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        public string country { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int sort { get; set; }
        /// <summary>
        /// 采集代码
        /// </summary>
        public string isColletCode { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public decimal balance { get; set; }
        /// <summary>
        /// 关注人数
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int follow { get; set; }
        /// <summary>
        /// 直播状态
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public AnchorStatus status { get; set; }
    }
}
