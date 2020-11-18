using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace Elight.Entity.Sys
{
    /// <summary>
    /// 收益报表
    /// </summary>
    [SugarTable("AnchorRecordDB.dbo.Income")]
    public class SysIncomeEntity
    {
        /// <summary>
        /// id
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }
        /// <summary>
        /// 商户id
        /// </summary>
        public int ShopID { get; set; }
        /// <summary>
        /// 主播id
        /// </summary>
        public int AnchorID { get; set; }
        /// <summary>
        /// 统计时间
        /// </summary>
        public DateTime opdate { get; set; }
        /// <summary>
        /// /工时收益
        /// </summary>
        public decimal hour_income { get; set; }
        /// <summary>
        /// 代理礼物收益
        /// </summary>
        public decimal agent_income { get; set; }
        /// <summary>
        /// 主播礼物收益
        /// </summary>
        public decimal tip_income { get; set; }
        /// <summary>
        /// 带玩收益
        /// </summary>
        public decimal test_income { get; set; }
        /// <summary>
        /// 平台收益
        /// </summary>
        public decimal Platform_income { get; set; }
        /// <summary>
        /// 代理工时收益
        /// </summary>
        public decimal agentHour_income { get; set; }
    }
}
