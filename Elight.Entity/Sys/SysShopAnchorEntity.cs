using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elight.Entity.Sys
{
    [SugarTable("QPAgentAnchorDB.dbo.Sys_ShopAnchor")]
    public class SysShopAnchorEntity
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }
        /// <summary>
        /// 商户ID
        /// </summary>
        public int ShopID { get; set; }
        /// <summary>
        /// 主播ID
        /// </summary>
        public int AnchorID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(DateTimeToJson))]
        public DateTime? CreateTime { get; set; }
    }
}
