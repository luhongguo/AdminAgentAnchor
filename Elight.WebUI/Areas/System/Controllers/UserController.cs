﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Elight.WebUI.Controllers;
using Elight.WebUI.Filters;
using Elight.Logic.Sys;
using Elight.Entity.Sys;
using Elight.Utility.ResponseModels;
using Elight.Utility.Format;
using Elight.Utility.Extension;
using Elight.Utility.Operator;
using Elight.Utility.Log;
using Elight.Utility.Model;

namespace Elight.WebUI.Areas.System.Controllers
{
    [LoginChecked]
    public class UserController : BaseController
    {
        private SysUserLogic userLogic;
        private SysUserRoleRelationLogic userRoleRelationLogic;
        private SysUserLogOnLogic userLogOnLogic;
        private SysUserAnchorLogic userAnchorLogic;
        public UserController()
        {
            userLogic = new SysUserLogic();
            userRoleRelationLogic = new SysUserRoleRelationLogic();
            userLogOnLogic = new SysUserLogOnLogic();
            userAnchorLogic = new SysUserAnchorLogic();
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
            var pageData = userLogic.GetList(pageIndex, pageSize, keyWord, ref totalCount);
            var result = new LayPadding<SysUser>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return Content(result.ToJson());
        }

        [HttpGet, AuthorizeChecked]
        public ActionResult Form()
        {
            return View();
        }

        [HttpPost, AuthorizeChecked, ValidateAntiForgeryToken]
        public ActionResult Form(SysUser model, string password, string roleIds)
        {
            try
            {
                if (model.Id.IsNullOrEmpty())
                {
                    var userEntity = userLogic.CheckUserName(model.Account);
                    if (userEntity != null)
                    {
                        return Error("已存在当前用户名，请重新输入");
                    }
                    DateTime defaultDt = DateTime.Today;
                    DateTime.TryParse(model.StrBirthday + " 00:00:00", out defaultDt);
                    model.Birthday = defaultDt;
                    int row = userLogic.Insert(model, password, roleIds.ToStrArray());
                    return row > 0 ? Success() : Error();
                }
                else
                {
                    DateTime defaultDt = DateTime.Today;
                    DateTime.TryParse(model.StrBirthday + " 00:00:00", out defaultDt);
                    model.Birthday = defaultDt;
                    //更新用户基本信息。
                    int row = userLogic.UpdateAndSetRole(model, password, roleIds.ToStrArray());
                    //更新用户角色信息。
                    return row > 0 ? Success() : Error();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message + "---" + ex.StackTrace);
                return Error();
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
            SysUser entity = userLogic.Get(primaryKey);
            entity.RoleId = userRoleRelationLogic.GetList(entity.Id).Select(c => c.RoleId).ToList();
            entity.StrBirthday = entity.Birthday.Value.ToString("yyyy-MM-dd");
            return Content(entity.ToJson());
        }

        [HttpPost, AuthorizeChecked]
        public ActionResult Delete(string userIds)
        {
            SysUser entity = userLogic.GetUserRole(userIds);
            if (entity.ShopID == 0 && entity.SortCode == 1)
            {
                return Error("不能删除超管的管理员角色用户");
            }
            //删除用户信息,用户角色，用户登录，用户返点
            int row = userLogic.Delete(userIds.ToStrArray());
            userRoleRelationLogic.Delete(userIds.ToStrArray());
            userLogOnLogic.Delete(userIds.ToStrArray());
            new SysRebateLogic().DeleteRebateByUserID(userIds.ToStrArray());
            return row > 0 ? Success() : Error();
        }

        [HttpPost]
        public ActionResult CheckAccount(string userName)
        {
            var userEntity = userLogic.CheckUserName(userName);
            if (userEntity != null)
            {
                return Error("已存在当前用户名，请重新输入");
            }
            return Success("恭喜您，该用户名可以注册");
        }
        /// <summary>
        /// 经纪人名称下拉框
        /// </summary>
        /// <returns>new {id,username}</returns>
        [HttpGet]
        public string GetUserIDSelect()
        {
            return userLogic.GetUserIDSelect();
        }
    }
}
