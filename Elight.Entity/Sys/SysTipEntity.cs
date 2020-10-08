using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SqlSugar;
namespace Elight.Entity.Sys
{
    /// <summary>
    /// 采集礼物实体
    /// </summary>
    [SugarTable("AnchorRecordDB.dbo.Tip")]
    public partial class TipEntity
    {
        /// <summary>
        /// id
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }
        /// <summary>
        /// 主播ID
        /// </summary>
        public int AnchorID { get; set; }
        /// <summary>
        /// 类型1礼物   2;房间扣费
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 礼物类型
        /// </summary>
        public string gift { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal? price { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int? quantity { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal totalamount { get; set; }

        /// <summary>
        /// 打赏时间
        /// </summary>
        [JsonConverter(typeof(DateTimeToJson))]
        public DateTime sendtime { get; set; }

        /// <summary>
        /// 状态 0无效 1有效   (返点算完后 状态改成无效)
        /// </summary>
        public int? status { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string orderno { get; set; }
        /// <summary>
        /// 公司代码
        /// </summary>
        public string companycode { get; set; }
        /// <summary>
        /// 打赏人
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 比率
        /// </summary>
        public int ratio { get; set; }
        /// <summary>
        /// 结算状态  0未结算 1已结算
        /// </summary>
        public int issettle { get; set; }
        /// <summary>
        /// 结算时间
        /// </summary>
        [JsonConverter(typeof(DateTimeToJson))]
        public DateTime? Settletime { get; set; }

        /// <summary>
        /// 是否为带玩账号 1是0否
        /// </summary>
        public int istest { get; set; }
        /// <summary>
        /// 公司后缀
        /// </summary>
        public string companystyle { get; set; }
        /// <summary>
        /// 确认扣除会员金额状态 0未确认 1已确认 2确认未成功
        /// </summary>
        public int isconfirm { get; set; }
        /// <summary>
        /// 确认未成功的原因描述
        /// </summary>
        public string errorreason { get; set; }
        /// <summary>
        /// 确认时间
        /// </summary>
        [JsonConverter(typeof(DateTimeToJson))]
        public DateTime? confirmtime { get; set; }
        /// <summary>
        /// 扣款单号，后台传值，第三方创建
        /// </summary>
        public string cashno { get; set; }
        /// <summary>
		/// 房间号
		/// </summary>
		public string roomid { get; set; }

        /// <summary>
        /// 登录凭证
        /// </summary>
        public string loginkey { get; set; }

        /// <summary>
        /// 礼物名称
        /// </summary>
        public string giftname { get; set; }
        /// <summary>
        /// 游戏ID
        /// </summary>
        public int gameType { get; set; }
        /// <summary>
        /// 货币类型
        /// </summary>
        public string curLogogram { get; set; }
        /// <summary>
        /// 主播名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string AnchorName { get; set; }
        /// <summary>
        /// 主播昵称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string AnchorNickName { get; set; }
    }
}
