using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elight.Entity.Model
{
    /// <summary>
    /// 添加花币实体
    /// </summary>
    public class AddTipDetailModel
    {
        /// <summary>
        /// 主播名称
        /// </summary>
        public string anchorName { get; set; }
        /// <summary>
        /// 花币金额
        /// </summary>
        public decimal money { get; set; }
    }
}
