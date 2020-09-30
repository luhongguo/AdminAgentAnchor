using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elight.WebUI.Controllers;
using Elight.WebUI.Filters;
using Elight.Logic.Sys;
using Elight.Utility.ResponseModels;
using Elight.Utility.Format;
using Elight.Utility.Extension;
using Elight.Utility.Operator;

namespace Elight.WebUI.Areas.System.Controllers
{
    [LoginChecked]
    public class RoleAuthorizeController : BaseController
    {
        private SysRoleAuthorizeLogic roleAuthorizeLogic;
        private SysPermissionLogic permissionLogic;

        public RoleAuthorizeController()
        {
            roleAuthorizeLogic = new SysRoleAuthorizeLogic();
            permissionLogic =new SysPermissionLogic();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string roleId)
        {
            var listPerIds = roleAuthorizeLogic.GetList(roleId).Select(c => c.ModuleId).ToList();
            var listAllPers = permissionLogic.GetShopPowersList(OperatorProvider.Instance.Current.ShopID);
            List<ZTreeNode> result = new List<ZTreeNode>();
            foreach (var item in listAllPers)
            {
                ZTreeNode model = new ZTreeNode();
                model.@checked = listPerIds.Contains(item.Id) ? model.@checked = true : model.@checked = false;
                model.id = item.Id;
                model.pId = item.ParentId;
                model.name = item.Name;
                model.open = true;
                result.Add(model);
            }
            return Content(result.ToJson());
        }

        [HttpPost]
        public ActionResult Form(string roleId, string perIds)
        {
            roleAuthorizeLogic.Authorize(roleId, perIds.ToStrArray());
            return Success("授权成功");
        }
        /// <summary>
        /// 商户授权
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ShopAuthorizeIndex()
        {
            return View();
        }
        /// <summary>
        /// 获取商户权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ShopPowersIndex(string shopID)
        {
            //商户权限 用编码标识是否存在
            var listPerIds = permissionLogic.GetShopPowersList(Convert.ToInt32(shopID)).Select(it=>it.EnCode);
            //超管权限
            var listAllPers = permissionLogic.GetShopPowersList(OperatorProvider.Instance.Current.ShopID);
            List<ZTreeNode> result = new List<ZTreeNode>();
            foreach (var item in listAllPers)
            {
                ZTreeNode model = new ZTreeNode();
                model.@checked = listPerIds.Contains(item.EnCode) ? model.@checked = true : model.@checked = false;
                model.id = item.Id;
                model.pId = item.ParentId;
                model.name = item.Name;
                model.open = true;
                result.Add(model);
            }
            return Content(result.ToJson());
        }
        /// <summary>
        /// 保存商户的权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="perIds"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ShopPowersForm(string shopID, string perIds)
        {
            roleAuthorizeLogic.ShopAuthorize(Convert.ToInt32(shopID), perIds.ToStrArray());
            return Success("授权成功");
        }
    }
}
