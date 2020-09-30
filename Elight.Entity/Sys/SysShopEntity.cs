using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SqlSugar;
namespace Elight.Entity.Sys
{
    [SugarTable("QPAgentAnchorDB.dbo.Sys_Shop")]
    public class SysShopEntity
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
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
    }
}
