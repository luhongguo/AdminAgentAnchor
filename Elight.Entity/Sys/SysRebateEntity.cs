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
    /// 返点实例
    /// </summary>
    [SugarTable("QPAgentAnchorDB.dbo.Sys_Rebate")]
    public class SysRebateEntity
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// 主播ID
        /// </summary>
        public int ShopID { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 礼物返点
        /// </summary>
        public decimal TipRebate { get; set; }
        /// <summary>
        /// 工时返点
        /// </summary>
        public decimal HourRebate { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifiedBy { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(DateTimeToJson))]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [JsonConverter(typeof(DateTimeToJson))]
        public DateTime? ModifiedTime { get; set; }
        /// <summary>
        /// 用户账号
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string UserAccount { get; set; }
    }
}
