using Elight.Utility.Format;
using Elight.Utility.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Elight.WebUI.Controllers
{
    /// <summary>
    /// 控制器基类。
    /// </summary>
    public class BaseController : Controller
    {
        #region 快捷方法
        protected ActionResult Success(string message = "恭喜您，操作成功。", object data = null)
        {
            return Content(new AjaxResult(ResultType.Success, message, data).ToJson());
        }
        protected ActionResult Error(string message = "对不起，操作失败。", object data = null)
        {
            return Content(new AjaxResult(ResultType.Error, message, data).ToJson());
        }
        protected ActionResult Warning(string message, object data = null)
        {
            return Content(new AjaxResult(ResultType.Warning, message, data).ToJson());
        }
        protected ActionResult Info(string message, object data = null)
        {
            return Content(new AjaxResult(ResultType.Info, message, data).ToJson());
        }
        #endregion
        /// <summary>
        /// 新版本分页返回成功信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected ActionResult pageSuccess(object data = null, int count = 0, string message = "success。")
        {
            return Content(new TableResult(0, message, data, count).ToJson());
        }
        protected ActionResult pageError( object data = null, int count = 0, string message = "error。")
        {
            return Content(new TableResult(1, message, data, count).ToJson());
        }
    }
}
