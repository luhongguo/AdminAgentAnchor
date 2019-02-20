﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elight.Entity.Sys
{
    [SugarTable("Sys_Role")]
    public partial class SysRole : ModelContext
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public string Id { get; set; }

        [SugarColumn(ColumnName = "OrganizeId")]
        public string OrganizeId { get; set; }

        [SugarColumn(ColumnName = "EnCode")]
        public string EnCode { get; set; }

        [SugarColumn(ColumnName = "Type")]
        public int? Type { get; set; }

        [SugarColumn(ColumnName = "Name")]
        public string Name { get; set; }


        [SugarColumn(ColumnName = "AllowEdit")]
        public string AllowEdit { get; set; }

        [SugarColumn(ColumnName = "DeleteMark")]
        public string DeleteMark { get; set; }

        [SugarColumn(ColumnName = "IsEnabled")]
        public string IsEnabled { get; set; }

        [SugarColumn(ColumnName = "Remark")]
        public string Remark { get; set; }

        [SugarColumn(ColumnName = "SortCode")]
        public int? SortCode { get; set; }

        [SugarColumn(ColumnName = "CreateUser")]
        public string CreateUser { get; set; }


        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime? CreateTime { get; set; }


        [SugarColumn(ColumnName = "ModifyUser")]
        public string ModifyUser { get; set; }


        [SugarColumn(ColumnName = "ModifyTime")]
        public DateTime? ModifyTime { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string DeptName { get; set; }// { get { return this.OrganizeSingle.FullName; } }

        //[SugarColumn(IsIgnore = true)]
        //public SysOrganize OrganizeSingle
        //{
        //    get
        //    {
        //        try
        //        {
        //            return base.CreateMapping<SysOrganize>().Single(it => it.Id == this.OrganizeId);
        //        }
        //        catch
        //        {
        //            return new SysOrganize();
        //        }

        //    }
        //}
    }
}
