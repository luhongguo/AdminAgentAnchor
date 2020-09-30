using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using Elight.WebUI.Controllers;
using Elight.WebUI.Filters;
using Elight.Logic.Sys;
using Elight.Entity.Sys;
using Elight.Utility.ResponseModels;
using Elight.Utility.Format;
using Elight.Utility.Extension;
using System.ServiceModel;
using Elight.Utility.Operator;

namespace Elight.WebUI.Areas.System.Controllers
{
    [LoginChecked]
    public class PermissionController : BaseController
    {
        private SysPermissionLogic permissionLogic;

        public PermissionController()
        {
            permissionLogic = new SysPermissionLogic();
        }

        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord)
        {
            int totalCount = 0;
            var pageData = permissionLogic.GetList(pageIndex, pageSize, keyWord, ref totalCount);
            var result = new LayPadding<SysPermission>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count,
            };
            return Content(result.ToJson());
        }

        [HttpGet, AuthorizeChecked]
        public ActionResult Form()
        {
            return View();
        }

        [HttpPost, AuthorizeChecked, ValidateAntiForgeryToken]
        public ActionResult Form(SysPermission model)
        {
            if (model.Id.IsNullOrEmpty())
            {
                int row = permissionLogic.Insert(model);
                return row > 0 ? Success() : Error();
            }
            else
            {
                int row = permissionLogic.Update(model);
                return row > 0 ? Success() : Error();
            }
        }

        [HttpGet, AuthorizeChecked]
        public ActionResult Detail()
        {
            return View();
        }
        /// <summary>
        /// 删除权限 
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        [HttpPost, AuthorizeChecked]
        public ActionResult Delete(string primaryKey)
        {
            long count = permissionLogic.GetChildCount(primaryKey);
            if (count == 0)
            {
                int row = permissionLogic.Delete(primaryKey.ToStrArray());
                return row > 0 ? Success() : Error();
            }
            return Error(string.Format("操作失败，请先删除该项的{0}个子级权限。", count));
        }

        [HttpPost]
        public ActionResult GetForm(string primaryKey)
        {
            SysPermission entity = permissionLogic.Get(primaryKey);
            //entity.IsEdit = entity.IsEdit == "1" ? "true" : "false";
            entity.IsEnable = entity.IsEnable == "1" ? "true" : "false";
            //entity.IsPublic = entity.IsPublic == "1" ? "true" : "false";
            return Content(entity.ToJson());
        }

        [HttpPost]
        public ActionResult GetParent()
        {
            var data = permissionLogic.GetShopPowersList(OperatorProvider.Instance.Current.ShopID);
            var treeList = new List<TreeSelect>() { new TreeSelect { id = "1", text = "最高级菜单", parentId = "0" } };
            foreach (SysPermission item in data)
            {
                TreeSelect model = new TreeSelect();
                model.id = item.Id;
                model.text = item.Name;
                model.parentId = item.ParentId;
                treeList.Add(model);
            }
            return Content(treeList.ToTreeSelectJson());
        }

        [HttpGet]
        public ActionResult Icon()
        {
            return View();
        }

    }

}
