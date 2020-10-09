using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elight.Entity.Sys;
using Newtonsoft.Json;
using SqlSugar;
namespace Elight.Entity
{
    /// <summary>
    /// 经纪人提现
    /// </summary>
    [SugarTable("QPAgentAnchorDB.dbo.AgentWithdrawalRecord")]
    public class SysAgentWithdrawalRecordEntity
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }
        /// <summary>
        /// 经纪人id
        /// </summary>
        public string AgentID { get; set; }
        /// <summary>
        /// 提现金额
        /// </summary>
        public decimal WithdrawalAmount { get; set; }
        /// <summary>
        /// 银行卡id
        /// </summary>
        public long AgentBankID { get; set; }
        /// <summary>
        /// 提现备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 提现类型:1提现，2：手动扣款
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 提现状态：1成功，2：驳回，3：待处理
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 反馈信息
        /// </summary>
        public string Feedback { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(DateTimeToJson))]
        public DateTime createTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [JsonConverter(typeof(DateTimeToJson))]
        public DateTime ModifiedTime { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifiedBy { get; set; }
        /// <summary>
        /// 代理名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string AgentName { get; set; }
        /// <summary>
        /// 银行卡名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string CategoryCode { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string bankano { get; set; }
        /// <summary>
        /// 开户人
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string bankaccount { get; set; }
        /// <summary>
        /// 开户地址
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string address { get; set; }
        /// <summary>
        /// 支付类型：1：银行卡，2：支付宝，3：微信
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int payType { get; set; }
    }
}
