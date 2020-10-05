using Elight.Logic.Sys;
using Elight.Utility.Format;
using Elight.Utility.Model;
using Elight.WebUI.Controllers;
using Elight.WebUI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elight.Entity;
using Elight.Entity.Sys;
using Elight.Utility.Extension;
using Microsoft.Ajax.Utilities;

namespace Elight.WebUI.Areas.System.Controllers
{
    /// <summary>
    /// 主播返点
    /// </summary>
    [LoginChecked]
    public class AnchorRebateController : BaseController
    {
        private SysAnchorRebateLogic sysAnchorRebateLogic;
        public AnchorRebateController()
        {
            sysAnchorRebateLogic = new SysAnchorRebateLogic();
        }
        // GET: System/Rebate
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取返点集合
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord">查询条件</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetRebateListPage(PageParm parm)
        {
            int totalCount = 0;
            var res = sysAnchorRebateLogic.GetAnchorRebatePage(parm, ref totalCount);
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
        public ActionResult Form(SysAnchorRebateEntity model)
        {
            if (model.id == 0)
            {
                var userModel = sysAnchorRebateLogic.CheckAnchor(model.AnchorName);
                if (userModel == null)
                {
                    return Error("当前主播账号不存在，请重新输入");
                }
                var rebateModel = sysAnchorRebateLogic.GetRebateByAccount(userModel.id);
                if (rebateModel != null)
                {
                    return Error("主播已有返点信息，不可重复添加");
                }
                var agentModel = new SysUserLogic().CheckUserName(model.UserAccount);
                if (agentModel == null)
                {
                    return Error("上级账号不存在!");
                }
                model.ShopID = 0;
                model.AnchorID = userModel.id;
                model.parentID = agentModel.Id;
                int row = sysAnchorRebateLogic.Insert(model);
                return row > 0 ? Success() : Error();
            }
            else
            {
                int row = sysAnchorRebateLogic.Update(model);
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
            SysAnchorRebateEntity entity = sysAnchorRebateLogic.Get(primaryKey);
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
            int row = sysAnchorRebateLogic.Delete(idlist);
            return row > 0 ? Success() : Error();
        }
    }
}