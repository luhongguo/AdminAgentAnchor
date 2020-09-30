using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elight.WebUI.Controllers;
using Elight.WebUI.Filters;
using Elight.Logic.Sys;
using Elight.Entity.Sys;
using Elight.Utility.ResponseModels;
using Elight.Utility.Extension;
using Elight.Utility.Format;

namespace Elight.WebUI.Areas.System.Controllers
{
    [LoginChecked]
    public class RoleController : BaseController
    {
        private SysRoleLogic roleLogic;

        public RoleController()
        {
            roleLogic = new SysRoleLogic();
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
            var pageData = roleLogic.GetList(pageIndex, pageSize, keyWord, ref totalCount);
            var result = new LayPadding<SysRole>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount// pageData.Count
            };
            return Content(result.ToJson());
        }

        [HttpGet, AuthorizeChecked]
        public ActionResult form()
        {
            return View();
        }

        [HttpPost, AuthorizeChecked, ValidateAntiForgeryToken]
        public ActionResult Form(SysRole model)
        {
            if (model.Id.IsNullOrEmpty())
            {
                int row = roleLogic.Insert(model);
                return row > 0 ? Success() : Error();
            }
            else
            {
                int row = roleLogic.Update(model);
                return row > 0 ? Success() : Error();
            }
        }

        [HttpGet, AuthorizeChecked]
        public ActionResult Detail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetForm(string primaryKey)
        {
            SysRole entity = roleLogic.Get(primaryKey);
            entity.IsEnabled = entity.IsEnabled == "1" ? "true" : "false";
            return Content(entity.ToJson());
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        [HttpPost, AuthorizeChecked]
        public ActionResult Delete(string primaryKey)
        {
            int row = roleLogic.Delete(primaryKey.ToStrArray());
            return row > 0 ? Success() : Error();
        }

        [HttpPost]
        public ActionResult GetListTreeSelect()
        {
            List<SysRole> listRole = roleLogic.GetList();
            var listTree = new List<TreeSelect>();
            foreach (var item in listRole)
            {
                TreeSelect model = new TreeSelect();
                model.id = item.Id;
                model.text = item.Name;
                listTree.Add(model);
            }
            return Content(listTree.ToJson());
        }
    }


}
