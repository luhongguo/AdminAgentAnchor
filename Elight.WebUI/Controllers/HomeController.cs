using Elight.Logic.Sys;
using Elight.WebUI.Filters;
using Elight.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elight.Entity.Enum;
using Elight.Utility.Operator;
using Elight.Utility.Files;
using Elight.Utility.ResponseModels;
using Elight.Utility.Format;
using Elight.Entity.Sys;
using Elight.Utility;

namespace QingDai.WebUI.Controllers
{
    //[LoginChecked]
    public class HomeController : BaseController
    {
        private SysUserLogic userLogic;
        private SysItemsDetailLogic itemDetailLogic;
        private SysUserLogOnLogic userLogOnLogic;
        private SysPermissionLogic permissionLogic;
        public HomeController()
        {
            userLogic = new SysUserLogic();
            itemDetailLogic = new SysItemsDetailLogic();
            userLogOnLogic = new SysUserLogOnLogic();
            permissionLogic = new SysPermissionLogic();
        }

        /// <summary>
        /// 后台首页视图。
        /// </summary>
        /// <returns></returns>
        [Route("Home/Index")]
        [HttpGet, LoginChecked]
        public ActionResult Index()
        {
            if (OperatorProvider.Instance.Current != null)
            {
                ViewBag.SoftwareName = ConstUtils.SoftwareName;

                ViewBag.Account = OperatorProvider.Instance.Current.Account;
                ViewBag.Avatar = OperatorProvider.Instance.Current.Avatar;
                return View();
            }
            else
            {
                return Redirect("/Account/Login");
            }
        }
        /// <summary>
        /// 默认显示视图。
        /// </summary>
        /// <returns></returns>
        [Route("Home/Default")]
        [HttpGet, LoginChecked]
        public ActionResult Default()
        {
            return View();
        }

        /// <summary>
        /// 获取左侧菜单。
        /// </summary>
        /// <returns></returns>
        [Route("Home/GetLeftMenu")]
        [HttpPost, LoginChecked]
        public ActionResult GetLeftMenu()
        {
            string userId = OperatorProvider.Instance.Current.UserId;

            List<LayNavbar> listNavbar = new List<LayNavbar>();
            var listModules = permissionLogic.GetList(userId);
            foreach (var item in listModules.Where(c => c.Type == ModuleType.Menu && c.Layer == 0).ToList())
            {
                LayNavbar navbarEntity = new LayNavbar();
                var listChildNav = listModules.Where(c => c.Type == ModuleType.Menu && c.Layer == 1 && c.ParentId == item.Id)
                    .Select(c => new LayChildNavbar() { href = c.Url, icon = c.Icon, title = c.Name }).ToList();
                navbarEntity.icon = item.Icon;
                navbarEntity.spread = false;
                navbarEntity.title = item.Name;
                navbarEntity.children = listChildNav;
                listNavbar.Add(navbarEntity);
            }
            return Content(listNavbar.ToJson());
        }

        /// <summary>
        /// 获取登录用户权限。
        /// </summary>
        /// <returns></returns>
        [Route("Home/GetPermission")]
        [HttpPost, LoginChecked]
        public ActionResult GetPermission()
        {
            var userId = OperatorProvider.Instance.Current.UserId;
            List<SysPermission> modules = permissionLogic.GetList(userId);
            return Content(modules.ToJson());
        }
    }
}
