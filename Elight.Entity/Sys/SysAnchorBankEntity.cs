﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SqlSugar;
namespace Elight.Entity.Sys
{
    /// <summary>
    /// 主播银行卡
    /// </summary>
    [SugarTable("QPAgentAnchorDB.dbo.AnchorBank")]
    public class SysAnchorBankEntity
    {
        /// <summary>
        /// id
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }
        /// <summary>
        /// 商户ID
        /// </summary>
        public int ShopID { get; set; }
        /// <summary>
        /// 主播ID
        /// </summary>
        public int AnchorID { get; set; }
        /// <summary>
        /// 银行卡名称
        /// </summary>
        public string CategoryCode { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string bankano { get; set; }
        /// <summary>
        /// 开户人
        /// </summary>
        public string bankaccount { get; set; }
        /// <summary>
        /// 开户地址
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(DateTimeToJson))]
        public DateTime createtime { get; set; }
        /// <summary>
        /// 支付类型：1：银行卡，2：支付宝，3：微信
        /// </summary>
        public int payType { get; set; }
        /// <summary>
        /// 代理名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string AgentName { get; set; }
        /// <summary>
        /// 主播昵称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string NickName { get; set; }
        /// <summary>
        /// 支付图片
        /// </summary>
        public string ImgUrl { get; set; }
    }
}
