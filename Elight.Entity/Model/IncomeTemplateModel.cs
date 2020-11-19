﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elight.Entity.Model
{
    /// <summary>
    /// 主播总收益
    /// </summary>
    public class IncomeTemplateModel
    {
        /// <summary>
        /// 主播ID
        /// </summary>
        public int AnchorID { get; set; }
        /// <summary>
        /// 主播名称
        /// </summary>
        public string AnchorName { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public decimal Balance { get; set; }
        /// <summary>
        /// 主播礼物收益
        /// </summary>
        public decimal tip_income { get; set; }
        /// <summary>
        /// 代理礼物收益
        /// </summary>
        public decimal agent_income { get; set; }
        /// <summary>
        /// 平台礼物收益
        /// </summary>
        public decimal Platform_income { get; set; }
        /// <summary>
        /// 工时收益
        /// </summary>
        public decimal hour_income { get; set; }
        /// <summary>
        /// 代理工时收益
        /// </summary>
        public decimal agentHour_income { get; set; }
        /// <summary>
        /// 带玩收益
        /// </summary>
        public decimal test_income { get; set; }
        /// <summary>
        /// 直播时长
        /// </summary>
        public decimal livetime { get; set; }
    }
}
