using Elight.Entity.Sys;
using Elight.Logic.Sys;
using Elight.Utility.Extension;
using Elight.Utility.Format;
using Elight.Utility.Model;
using Elight.WebUI.Controllers;
using Elight.WebUI.Filters;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Elight.WebUI.Areas.System.Controllers
{
    [LoginChecked]
    public class ShopController : BaseController
    {
        private readonly SysShopLogic sysShopLogic;
        public ShopController()
        {
            sysShopLogic = new SysShopLogic();
        }
        // GET: System/Shop
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取商户分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord">查询条件</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetShopPage(PageParm parm)
        {
            int totalCount = 0;
            var res = sysShopLogic.GetShopPage(parm, ref totalCount);
            return pageSuccess(res, totalCount);
        }
        /// <summary>
        /// 编辑页面
        /// </summary>
        /// <returns></returns>
        [HttpGet, AuthorizeChecked]
        public ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// 新增或修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, AuthorizeChecked, ValidateAntiForgeryToken]
        public ActionResult Form(SysShopEntity model)
        {
            if (model.ID == null)
            {
                int row = sysShopLogic.Insert(model);
                return row > 0 ? Success() : Error();
            }
            else
            {
                int row = sysShopLogic.Update(model);
                return row > 0 ? Success() : Error();
            }
        }
        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetForm(int primaryKey)
        {
            SysShopEntity entity = sysShopLogic.Get(primaryKey);
            return Content(entity.ToJson());
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        [HttpPost, AuthorizeChecked]
        public ActionResult Delete(string primaryKey)
        {
            string[] idArr = primaryKey.ToStrArray();
            List<int> idlist = new List<int>();
            idArr.ForEach(it =>
            {
                idlist.Add(Convert.ToInt32(it));
            });
            int row = sysShopLogic.Delete(idlist);
            return row > 0 ? Success() : Error();
        }
    }
}