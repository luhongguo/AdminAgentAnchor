using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elight.Utility.Model
{
    public class UserRebateModel
    {
        public string Id { get; set; }
        /// <summary>
        /// 经纪人礼物返点
        /// </summary>
        public decimal TipRebate { get; set; }
        /// <summary>
        /// 经纪人工时返点
        /// </summary>
        public decimal HourRebate { get; set; }
    }
}
