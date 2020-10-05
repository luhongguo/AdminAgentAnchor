using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elight.Entity.Sys
{

    [SugarTable("QPAgentAnchorDB.dbo.Sys_User")]
    public partial class SysUser : ModelContext
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        /// <summary>
        /// 商户ID
        /// </summary>
        public int ShopID { get; set; }

        [SugarColumn(ColumnName = "Account")]
        public string Account { get; set; }

        [SugarColumn(ColumnName = "RealName")]
        public string RealName { get; set; }

        [SugarColumn(ColumnName = "CompanyCode")]
        public string CompanyCode { get; set; }

        [SugarColumn(ColumnName = "Avatar")]
        public string Avatar { get; set; }


        [SugarColumn(ColumnName = "Gender")]
        public string Gender { get; set; }

        [SugarColumn(ColumnName = "Birthday")]
        public DateTime? Birthday { get; set; }

        [SugarColumn(ColumnName = "MobilePhone")]
        public string MobilePhone { get; set; }


        [SugarColumn(ColumnName = "Email")]
        public string Email { get; set; }

        [SugarColumn(ColumnName = "Signature")]
        public string Signature { get; set; }


        [SugarColumn(ColumnName = "Address")]
        public string Address { get; set; }


        [SugarColumn(ColumnName = "CompanyId")]
        public string CompanyId { get; set; }

        [SugarColumn(ColumnName = "IsEnabled")]
        public string IsEnabled { get; set; }

        [SugarColumn(ColumnName = "SortCode")]
        public int? SortCode { get; set; }

        [SugarColumn(ColumnName = "DepartmentId")]
        public string DepartmentId { get; set; }

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

        [SugarColumn(IsIgnore = true)]
        public string StrBirthday { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<string> RoleId { set; get; }
        [SugarColumn(IsIgnore = true)]
        public string ShopName { set; get; }
    }
}
