using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elight.Entity.Enum
{
    /// <summary>
    /// 权限类型。
    /// </summary>
    public class ModuleType
    {
        /// <summary>
        /// 菜单。
        /// </summary>
        public const int Menu = 0;
        /// <summary>
        /// 按钮。
        /// </summary>
        public const int Button = 1;
        /// <summary>
        /// 请求。
        /// </summary>
        public const int Ajax = 2;
    }
    /// <summary>
    /// 直播状态枚取
    /// </summary>
    public enum AnchorStatus
    {
        /// <summary>
        /// 正常=0
        /// </summary>
        正常 = 0,
        /// <summary>
        /// 直播=1
        /// </summary>
        直播 = 1,
        /// <summary>
        /// 离线=2
        /// </summary>
        离线 = 2,
        /// <summary>
        /// 禁用=3
        /// </summary>
        禁用 = 3
    }
}
