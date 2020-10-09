using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace Elight.Entity.Sys
{
    /// <summary>
    /// 银行卡类别
    /// </summary>
    [SugarTable("QPAgentAnchorDB.dbo.Sys_Category")]
    public class SysCategoryEntity
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        /// 调用别名(不一定唯一) 查询时作为条件
        /// </summary>
        public string AliasesCode { get; set; }
        /// <summary>
        /// 属性代码，唯一
        /// </summary>
        public string CategoryCode { get; set; }
        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 是否启用 1：启用 2：禁用
        /// </summary>
        public int IsEnable { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
