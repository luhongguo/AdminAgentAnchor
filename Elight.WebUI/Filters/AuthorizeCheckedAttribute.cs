using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Elight.Logic.Sys;
using Elight.Utility.Operator;

namespace Elight.WebUI.Filters
{
    /// <summary>
    /// 表示一个特性，该特性用于标识用户是否有访问权限。
    /// </summary>
    public class AuthorizeCheckedAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 是否忽略权限检查。
        /// </summary>
        public bool Ignore { get; set; }

        public AuthorizeCheckedAttribute(bool ignore = false)
        {
            this.Ignore = ignore;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Ignore)
            {
                return;
            };
            try
            {
                var current = OperatorProvider.Instance.Current;
                if (current == null)
                {
                    filterContext.HttpContext.Response.Write("<script>top.location.href = '/Account/Login'</script>");
                    //StringBuilder script = new StringBuilder();
                    //script.Append("<script>alert('对不起，Session已过期，请重新登录');</script>");
                    //filterContext.Result = new ContentResult() { Content = script.ToString() };
                }
                else
                {
                    SysPermissionLogic logic = new SysPermissionLogic();
                    var action = HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"].ToString();
                    bool hasPermission = logic.ActionValidate(current.UserId, action);
                    if (!hasPermission)
                    {
                        StringBuilder script = new StringBuilder();
                        script.Append("<script>alert('对不起，您没有权限访问当前页面。');</script>");
                        filterContext.Result = new ContentResult() { Content = script.ToString() };
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}