using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace Elight.Entity.Sys
{
    /// <summary>
    /// 直播记录表
    /// </summary>
    [SugarTable("AnchorDB.dbo.anchor_live_record")]
    public partial class SysAnchorLiveRecordEntity
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(ColumnName = "seqid", IsPrimaryKey = true)]
        public string seqid { get; set; }
        /// <summary>
        /// 主播id
        /// </summary>
        public int aid { get; set; }
        /// <summary>
        /// 上播时间
        /// </summary>
        public DateTime ontime { get; set; }
        /// <summary>
        /// 下播时间
        /// </summary>
        public DateTime uptime { get; set; }
        /// <summary>
        /// ip
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// 流地址
        /// </summary>
        public string flvurl { get; set; }
        /// <summary>
        /// 直播时长
        /// </summary>
        public decimal livetime { get; set; }
        /// <summary>
        /// 礼物数量
        /// </summary>
        public int giftnum { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal amount { get; set; }
        /// <summary>
        /// 房间信息
        /// </summary>
        public string roomvalue { get; set; }
        /// <summary>
        /// 是否有效：0否，1是
        /// </summary>
        public int status { get; set; }
    }
}
