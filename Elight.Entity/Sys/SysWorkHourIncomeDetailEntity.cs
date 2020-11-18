using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace Elight.Entity.Sys
{
    [SugarTable("AnchorRecordDB.dbo.WorkHourIncomeDetail")]
    public class SysWorkHourIncomeDetailEntity
    {
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }
        /// <summary>
        /// 主播ID
        /// </summary>
        public int anchorID { get; set; }
        /// <summary>
        /// 直播时长
        /// </summary>
        public decimal livetime { get; set; }
        /// <summary>
        /// 薪资
        /// </summary>
        public decimal Salary { get; set; }
        /// <summary>
        /// 统计开始时间
        /// </summary>
        public DateTime startTime { get; set; }
        /// <summary>
        /// 统计结束时间
        /// </summary>
        public DateTime endTime { get; set; }
        /// <summary>
        /// 代理工时收益
        /// </summary>
        public decimal agentHour_income { get; set; }
        /// <summary>
        /// 工时返点
        /// </summary>
        public decimal HourRebate { get; set; }
        /// <summary>
        /// 代理id
        /// </summary>
        public string UserID { get; set; }
    }
}
