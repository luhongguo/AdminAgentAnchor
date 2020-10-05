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
    /// 主播返点实例
    /// </summary>
    [SugarTable("QPAgentAnchorDB.dbo.Sys_AnchorRebate")]
    public class SysAnchorRebateEntity
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// 商户ID
        /// </summary>
        public int ShopID { get; set; }
        /// <summary>
        /// 主播ID
        /// </summary>
        public int AnchorID { get; set; }
        /// <summary>
        /// 经纪人ID
        /// </summary>
        public string parentID { get; set; }
        /// <summary>
        /// 主播礼物返点
        /// </summary>
        public decimal TipRebate { get; set; }
        /// <summary>
        /// 主播工时返点
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
        /// 主播账号
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string AnchorName { get; set; }
        /// <summary>
        /// 主播昵称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string AnchorNickName { get; set; }
        /// <summary>
        /// 上级账号
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string UserAccount { get; set; }
    }
}
