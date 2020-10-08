using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace Elight.Entity.Sys
{
    /// <summary>
    /// 代理、主播礼物收益明细
    /// </summary>
    [SugarTable("AnchorRecordDB.dbo.TipIncomeDetail")]
    public class SysTipIncomeDetailEntity
    {
        /// <summary>
        /// id
        /// </summary>
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }
        /// <summary>
        /// 商户ID
        /// </summary>
        public int ShopID { get; set; }
        /// <summary>
        /// 代理ID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 主播id
        /// </summary>
        public int AnchorID { get; set; }
        /// <summary>
        /// 订单
        /// </summary>
        public string orderno { get; set; }
        /// <summary>
        /// 代理收益
        /// </summary>
        public decimal UserIncome { get; set; }
        /// <summary>
        /// 主播收益
        /// </summary>
        public decimal AnchorIncome { get; set; }
        /// <summary>
        /// 平台收益
        /// </summary>
        public decimal PlatformIncome { get; set; }
        /// <summary>
        /// 主播礼物返点
        /// </summary>
        public decimal UserRebate { get; set; }
        /// <summary>
        /// 经纪人礼物返点
        /// </summary>
        public decimal PlatformRebate { get; set; }
        /// <summary>
        /// 统计时间 （采集时间的日期部分）
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public decimal totalamount { get; set; }
        /// <summary>
        /// 是否有效（0否，1是）
        /// </summary>
        public byte status { get; set; }
    }
}
