using Elight.Entity.Sys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elight.Entity.Model
{
    /// <summary>
    /// 返回礼物收益详情实体
    /// </summary>
    public class TipIncomeDetailModel
    {
        /// <summary>
        /// 代理名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 主播名称
        /// </summary>
        public string AnchorName { get; set; }
        /// <summary>
        /// 主播昵称
        /// </summary>
        public string AnchorNickName { get; set; }
        /// <summary>
        /// 代理收益
        /// </summary>
        public decimal? UserIncome { get; set; }
        /// <summary>
        /// 主播收益
        /// </summary>
        public decimal? AnchorIncome { get; set; }
        /// <summary>
        /// 平台收益
        /// </summary>
        public decimal? PlatformIncome { get; set; }
        /// <summary>
        /// 代理返点
        /// </summary>
        public decimal UserRebate { get; set; }
        /// <summary>
        /// 平台返点
        /// </summary>
        public decimal PlatformRebate { get; set; }
        /// <summary>
        /// 统计时间
        /// </summary>
        public int StartDate { get; set; }
        /// <summary>
        /// 订单
        /// </summary>
        public string orderno { get; set; }
        /// <summary>
        /// 礼物名称
        /// </summary>
        public string giftname { get; set; }
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
        public decimal? totalamount { get; set; }

        /// <summary>
        /// 打赏时间
        /// </summary>
        [JsonConverter(typeof(DateTimeToJson))]
        public DateTime? sendtime { get; set; }
        ///// <summary>
        ///// 打赏人
        ///// </summary>
        //public string Rewarder { get; set; }
        /// <summary>
        /// 类型1打赏礼物 2;房间扣费 ,3:计时扣费
        /// </summary>
        public int Type { get; set; }
    }
}
