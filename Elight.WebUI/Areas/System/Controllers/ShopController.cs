using Elight.Entity.Sys;
using Elight.Logic.Sys;
using Elight.Utility.Extension;
using Elight.Utility.Format;
using Elight.Utility.Model;
using Elight.Utility.ResponseModels;
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
            if (model.ID == 0)
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
       

        /// <summary>
        /// 商户授权主播页面
        /// </summary>
        /// <returns></returns>
        [HttpGet, AuthorizeChecked]
        public ActionResult Distribution()
        {
            return View();
        }
        /// <summary>
        /// 商户名下主播
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord">查询条件</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetUserAnchorList(int pageIndex, int pageSize, PageParm pageParm)
        {
            int totalCount = 0;
            var pageData = sysShopLogic.GetShopAnchorList(pageIndex, pageSize, pageParm.where, ref totalCount);
            var result = new LayPadding<SysAnchor>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount// pageData.Count
            };
            return Content(result.ToJson());
        }
        /// <summary>
        /// 商户新增主播页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddUserAnchor()
        {
            return View();
        }
        /// <summary>
        /// 经纪人不拥有的主播
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord">查询条件</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetUserNoOwnedAnchorList(int pageIndex, int pageSize, PageParm pageParm)
        {
            int totalCount = 0;
            var pageData = sysShopLogic.GetShopNoOwnedAnchorList(pageIndex, pageSize, pageParm.where, ref totalCount);
            var result = new LayPadding<SysAnchor>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount// pageData.Count
            };
            return Content(result.ToJson());
        }
        /// <summary>
        /// 添加主播给商户
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="idList">主播ID集合</param>
        /// <param name="userID">商户ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult addShopAnchor(string idList, int userID)
        {
            var agentUser = new SysUserLogic().GetUserByShopID(userID);
            if (agentUser == null)
            {
                return Error("商户没有经纪人，请先添加经纪人!");
            }
            var result = sysShopLogic.AddShopAnchor(idList.ToStrArray(), userID, agentUser);
            if (result)
            {
                return Success();
            }
            return Error();
        }
        /// <summary>
        /// 删除商户的主播
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="idList">主播ID集合</param>
        /// <param name="userID">商户ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteShopAnchor(string idList, int userID)
        {
            var result = sysShopLogic.Delete(idList.ToStrArray(), userID);
            if (result)
            {
                return Success();
            }
            return Error();
        }
        /// <summary>
        /// 获取商户下拉框
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetShopSelectList()
        {
            var result = sysShopLogic.GetShopSelectList();
            return Content(result.ToJson());
        }
    }
}