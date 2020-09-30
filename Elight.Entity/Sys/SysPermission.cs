using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elight.Entity.Sys
{

    [SugarTable("Sys_Permission")]
    public partial class SysPermission
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        /// <summary>
        /// 商户ID
        /// </summary>
        public int ShopID { get; set; }

        [SugarColumn(ColumnName = "ParentId")]
        public string ParentId { get; set; }

        [SugarColumn(ColumnName = "Layer")]
        public int? Layer { get; set; }

        [SugarColumn(ColumnName = "EnCode")]
        public string EnCode { get; set; }

        [SugarColumn(ColumnName = "Name")]
        public string Name { get; set; }


        [SugarColumn(ColumnName = "JsEvent")]
        public string JsEvent { get; set; }

        [SugarColumn(ColumnName = "Icon")]
        public string Icon { get; set; }

        [SugarColumn(ColumnName = "Url")]
        public string Url { get; set; }


        [SugarColumn(ColumnName = "Remark")]
        public string Remark { get; set; }

        [SugarColumn(ColumnName = "Type")]
        public int? Type { get; set; }

        [SugarColumn(ColumnName = "SortCode")]
        public int? SortCode { get; set; }
        /// <summary>
        /// 是否公开 0否  1是
        /// </summary>
        [SugarColumn(ColumnName = "IsPublic")]
        public string IsPublic { get; set; }
        /// <summary>
        /// 是否可用0否  1是
        /// </summary>

        [SugarColumn(ColumnName = "IsEnable")]
        public string IsEnable { get; set; }
        /// <summary>
        /// 是否编辑0否  1是
        /// </summary>

        [SugarColumn(ColumnName = "IsEdit")]
        public string IsEdit { get; set; }


        [SugarColumn(ColumnName = "DeleteMark")]
        public string DeleteMark { get; set; }

        [SugarColumn(ColumnName = "CreateUser")]
        public string CreateUser { get; set; }

        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime? CreateTime { get; set; }

        [SugarColumn(ColumnName = "ModifyUser")]
        public string ModifyUser { get; set; }


        [SugarColumn(ColumnName = "ModifyTime")]
        public DateTime? ModifyTime { get; set; }

    }
}
