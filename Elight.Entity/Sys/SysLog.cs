using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elight.Entity.Sys
{
    /// <summary>
    /// 日志实例
    /// </summary>
    [SugarTable("Sys_Log")]
    public partial class SysLog
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [JsonConverter(typeof(DateTimeToJson))]
        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime? CreateTime { get; set; }

        [SugarColumn(ColumnName = "LogLevel")]
        public string LogLevel { get; set; }

        [SugarColumn(ColumnName = "Operation")]
        public string Operation { get; set; }

        [SugarColumn(ColumnName = "Message")]
        public string Message { get; set; }

        [SugarColumn(ColumnName = "Account")]
        public string Account { get; set; }

        [SugarColumn(ColumnName = "RealName")]
        public string RealName { get; set; }

        [SugarColumn(ColumnName = "IP")]
        public string IP { get; set; }

        [SugarColumn(ColumnName = "IPAddress")]
        public string IPAddress { get; set; }

        [SugarColumn(ColumnName = "Browser")]
        public string Browser { get; set; }

        [SugarColumn(ColumnName = "StackTrace")]
        public string StackTrace { get; set; }

    }
}
