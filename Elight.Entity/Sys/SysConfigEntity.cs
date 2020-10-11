using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace Elight.Entity.Sys
{
    /// <summary>
    /// 主播基本信息
    /// </summary>
    [SugarTable("QPAgentAnchorDB.dbo.config")]
    public class SysConfigEntity
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// key
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string values { get; set; }
    }
}
