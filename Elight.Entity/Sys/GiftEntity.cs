using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace Elight.Entity.Sys
{
    /// <summary>
    /// 礼物
    /// </summary>
    [SugarTable("AnchorDB.dbo.gift")]
    public class GiftEntity
    {
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }

        /// <summary>
        /// 礼物代码
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 礼物名称
        /// </summary>
        public string codeName { get; set; }

        /// <summary>
        /// 礼物金额
        /// </summary>
        public decimal? price { get; set; }

        /// <summary>
        /// 金额单位（USD  ，CNY）
        /// </summary>
        public string curLogogram { get; set; }

        /// <summary>
        /// 是否特效 unlock ：是 lock：不是
        /// </summary>
        public int isSpecial { get; set; }
        public int isProp { get; set; }

        /// <summary>
        /// 是否使用 unlock ：使用 lock：不使用
        /// </summary>
        public int status { get; set; }
    }
}
