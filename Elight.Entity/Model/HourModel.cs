using Elight.Entity.Enum;
using Elight.Entity.Sys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elight.Entity.Model
{
    public class HourModel
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string AnchorName { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public int id { get; set; }
        ///// <summary>
        ///// 单号
        ///// </summary>
        //public string orderno { get; set; }
        /// <summary>
        /// 开播时间
        /// </summary>
        [JsonConverter(typeof(DateTimeToJson))]
        public DateTime begintime { get; set; }
        /// <summary>
        /// 停播时间
        /// </summary>
        [JsonConverter(typeof(DateTimeToJson))]
        public DateTime? endtime { get; set; }
        ///// <summary>
        ///// 结算时间
        ///// </summary>
        //[JsonConverter(typeof(DateTimeToJson))]
        //public DateTime operationtime { get; set; }
        ///// <summary>
        ///// 是否有效 0否 1是
        ///// </summary>
        //public int iseffective { get; set; }
        /// <summary>
        /// 待处理pending 结算中settlement  已处理settled
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 是否直播 0否 1是
        /// </summary>
        public int islive { get; set; }
        ///// <summary>
        ///// 描述
        ///// </summary>
        //public string description { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal amount { get; set; }
        ///// <summary>
        ///// 公司代码
        ///// </summary>
        //public string companycode { get; set; }
        /// <summary>
        /// 持续时长
        /// </summary>
        public decimal duration { get; set; }
        /// <summary>
        /// 来源 来源  0：腾讯，1：阿里
        /// </summary>
        public int source { get; set; }
        /// <summary>
        /// 推流地址
        /// </summary>
        public string flvurl { get; set; }
    }
}
