using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Elight.Entity.Sys
{
    [SugarTable("QPAgentAnchorDB.dbo.Sys_User_Anchor")]
    public class SysUserAnchor : ModelContext
    {
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public string id { get; set; }
        public string UserID { get; set; }
        public int AnchorID { get; set; }
        public int AnchorName { get; set; }
    }
}
