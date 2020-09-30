using Elight.Entity.Sys;
using Elight.Logic.Sys;
using Elight.WebUI.Filters;
using Elight.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elight.Utility.Security;
using Elight.Utility.Web;
using Elight.Utility.Extension;
using Elight.Utility.Operator;
using Elight.Utility.Log;
using Elight.Utility.Files;
using Elight.Utility.Format;
using Elight.Utility;
using System.Data;
using Elight.Utility.ResponseModels;
using Microsoft.Office.Interop.Excel;

namespace Elight.WebUI.Controllers
{
    public class AccountController : BaseController
    {
        private SysUserLogic userlogic;
        private SysUserLogOnLogic userLogOnLogic;
        private LogLogic logLogic;
        public AccountController()
        {
            userlogic = new SysUserLogic();
            userLogOnLogic = new SysUserLogOnLogic();
            logLogic = new LogLogic();
        }

        /// <summary>
        /// 登陆页面视图。
        /// </summary>
        /// <returns></returns>
        [Route("Account/Login")]
        [Route("admin")]
        [Route("admin.html")]
        [Route("")]
        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.SoftwareName = ConstUtils.SoftwareName;
            return View();
        }



        [Route("Account/PostTest")]
        [HttpPost]
        public ActionResult PostTest(string str)
        {
            var obj = new { Method = "POST", Data = str };
            return Content(obj.ToJson());
        }

        /// <summary>
        /// 获取验证码图片。
        /// </summary>
        /// <returns></returns>
        [Route("Account/VerifyCode")]
        [HttpGet]
        public ActionResult VerifyCode()
        {
            VerifyCode verify = new VerifyCode();
            WebHelper.SetSession(Keys.SESSION_KEY_VCODE, verify.Text.ToLower());
            return File(verify.Image, "image/jpeg");
        }

        /// <summary>
        /// 提交登陆信息。
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="verifycode">验证码</param>
        /// <returns></returns>
        [Route("Account/Login")]
        [HttpPost]
        public ActionResult Login(string userName, string password, string verifyCode)
        {
            if (userName.IsNullOrEmpty() || password.IsNullOrEmpty() || verifyCode.IsNullOrEmpty())
            {
                return Error("请求失败，缺少必要参数。");
            }
            if (verifyCode.ToLower() != WebHelper.GetSession(Keys.SESSION_KEY_VCODE))
            {
                return Warning("验证码错误，请重新输入。");
            }
            var userEntity = userlogic.GetByUserName(userName);
            if (userEntity == null)
            {
                return Warning("该账户不存在，请重新输入。");
            }
            if (userEntity.IsEnabled != "1")
            {
                return Warning("该账户已被禁用，请联系管理员。");
            }
            var userLogOnEntity = userLogOnLogic.GetByAccount(userEntity.Id);
            string inputPassword = password.DESEncrypt(userLogOnEntity.SecretKey).MD5Encrypt();
            if (inputPassword != userLogOnEntity.Password)
            {
                //LogHelper.Write(Level.Info, "系统登录", "密码错误", userEntity.Account, userEntity.RealName);
                logLogic.Write(Level.Info, "系统登录", "密码错误","", userEntity.Account, userEntity.RealName);
                return Warning("密码错误，请重新输入。");
            }
            else
            {
                Operator operatorModel = new Operator();
                operatorModel.UserId = userEntity.Id;
                operatorModel.Account = userEntity.Account;
                operatorModel.RealName = userEntity.RealName;
                operatorModel.Avatar = userEntity.Avatar;
                operatorModel.CompanyId = userEntity.CompanyId;
                //operatorModel.DepartmentId = userEntity.DepartmentId;
                operatorModel.LoginTime = DateTime.Now;
                operatorModel.Token = Guid.NewGuid().ToString().Replace("-", "").DESEncrypt();
                operatorModel.ShopID = userEntity.ShopID;
                OperatorProvider.Instance.Current = operatorModel;
                userLogOnLogic.UpdateLogin(userLogOnEntity);
                logLogic.Write(Level.Info, "系统登录", "登录成功","", userEntity.Account, userEntity.RealName);
                return Success();
            }
        }

        [Route("Account/AppLogin")]
        [HttpPost]
        public ActionResult AppLogin(string userName, string password)
        {
            if (userName.IsNullOrEmpty() || password.IsNullOrEmpty())
            {
                return Error("请求失败，缺少必要参数。");
            }
            var userEntity = userlogic.GetByUserName(userName);
            if (userEntity == null)
            {
                return Warning("该用户不存在，请重新输入。");
            }
            if (userEntity.IsEnabled != "1")
            {
                return Warning("该账户已被禁用，请联系管理员。");
            }
            var userLogOnEntity = userLogOnLogic.GetByAccount(userEntity.Id);
            string inputPassword = password.DESEncrypt(userLogOnEntity.SecretKey).MD5Encrypt();
            if (inputPassword != userLogOnEntity.Password)
            {
                return Warning("密码错误，请重新输入。");
            }
            else
            {
                return Success();
            }
        }

        /// <summary>
        /// 安全退出系统。
        /// </summary>
        /// <returns></returns>
        [Route("Account/Exit")]
        [HttpGet]
        public ActionResult Exit()
        {
            if (OperatorProvider.Instance.Current != null)
            {
                OperatorProvider.Instance.Remove();
            }
            return Redirect("/Account/Login");
        }

        /// <summary>
        /// 锁定登陆用户。
        /// </summary>
        /// <returns></returns>
        [Route("Account/Lock")]
        [HttpPost, LoginChecked]
        public ActionResult Lock()
        {
            if (OperatorProvider.Instance.Current != null)
            {
                OperatorProvider.Instance.Remove();
            }
            return Success();
        }

        /// <summary>
        /// 解锁登陆用户。
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [Route("Account/Unlock")]
        [HttpPost]
        public ActionResult Unlock(string username, string password)
        {
            var userEntity = userlogic.GetByUserName(username);
            var userLogOnEntity = userLogOnLogic.GetByAccount(userEntity.Id);
            string inputPassword = password.DESEncrypt(userLogOnEntity.SecretKey).MD5Encrypt();
            if (inputPassword != userLogOnEntity.Password)
            {
                return Warning("密码错误，请重新输入。");
            }
            else
            {
                //重新保存用户信息。
                Operator operatorModel = new Operator();
                operatorModel.UserId = userEntity.Id;
                operatorModel.Account = userEntity.Account;
                operatorModel.RealName = userEntity.RealName;
                operatorModel.Avatar = userEntity.Avatar;
                operatorModel.CompanyId = userEntity.CompanyId;
                operatorModel.LoginTime = DateTime.Now;
                operatorModel.Token = Guid.NewGuid().ToString().Replace("-", "").DESEncrypt();
                OperatorProvider.Instance.Current = operatorModel;
            }
            return Success();
        }

        /// <summary>
        /// 账户管理视图。
        /// </summary>
        /// <returns></returns>
        [Route("Account/InfoCard")]
        [HttpGet, LoginChecked]
        public ActionResult InfoCard()
        {
            string userId = OperatorProvider.Instance.Current.UserId;
            var userLogOnEntity = userLogOnLogic.GetByAccount(userId);
            ViewBag.LastVisitTime = userLogOnEntity.LastVisitTime;
            ViewBag.LoginCount = userLogOnEntity.LoginCount;
            return View();
        }

        /// <summary>
        /// 更新用户基础资料。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Account/InfoCard")]
        [HttpPost, LoginChecked]
        public ActionResult InfoCard(SysUser model)
        {
            DateTime defaultDt = DateTime.Today;
            DateTime.TryParse(model.StrBirthday, out defaultDt);
            model.Birthday = defaultDt;
            int row = userlogic.UpdateBasicInfo(model);
            return row > 0 ? Success() : Error();
        }

        [Route("Account/GetInfoCardForm")]
        [HttpPost, LoginChecked]
        public ActionResult GetInfoCardForm()
        {
            string userId = OperatorProvider.Instance.Current.UserId;
            SysUser userEntity = userlogic.Get(userId);
            userEntity.StrBirthday = userEntity.Birthday.Value.ToString("yyyy-MM-dd");
            return Content(userEntity.ToJson());
        }

        /// <summary>
        /// 上传头像。
        /// </summary>
        /// <returns></returns>
        [Route("Account/UploadAvatar")]
        [HttpPost, LoginChecked]
        public ActionResult UploadAvatar()
        {
            try
            {
                var file = Request.Files[0];
                if (file == null) { return Error(); }
                string userId = OperatorProvider.Instance.Current.UserId;
                string virtualPath = Path.Combine("/Uploads/Avatar/", userId + Path.GetExtension(file.FileName));
                string filePath = Request.MapPath(virtualPath);
                if (FileUtil.Exists(filePath))
                {
                    FileUtil.Delete(filePath);
                }
                file.SaveAs(filePath);
                return Success("上传成功。", virtualPath);
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "上传头像错误", ex.Message, ex.StackTrace);
                return Error("上传失败。请联系管理员！");
            }
        }

        /// <summary>
        /// 加载修改密码界面视图。
        /// </summary>
        /// <returns></returns>
        [Route("Account/ModifyPwd")]
        [HttpGet, LoginChecked]
        public ActionResult ModifyPwd()
        {
            return View();
        }

        /// <summary>
        /// 修改密码。
        /// </summary>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <param name="confirmPassword">确认密码</param>
        /// <returns></returns>
        [Route("Account/ModifyPwd")]
        [HttpPost, LoginChecked]
        public ActionResult ModifyPwd(string oldPassword, string newPassword, string confirmPassword)
        {
            if (oldPassword.IsNullOrEmpty() || newPassword.IsNullOrEmpty() || confirmPassword.IsNullOrEmpty())
            {
                return Error("请求失败，缺少必要参数。");
            }
            if (!newPassword.Equals(confirmPassword))
            {
                return Warning("两次密码输入不一致，请重新确认。");
            }
            string userId = OperatorProvider.Instance.Current.UserId;
            var userLoginEntity = userLogOnLogic.GetByAccount(userId);
            if (oldPassword.DESEncrypt(userLoginEntity.SecretKey).MD5Encrypt() != userLoginEntity.Password)
            {
                return Warning("旧密码验证失败。");
            }
            userLoginEntity.Password = newPassword.DESEncrypt(userLoginEntity.SecretKey).MD5Encrypt();
            int row = userLogOnLogic.ModifyPwd(userLoginEntity);
            return row > 0 ? Success() : Error();
        }

    }
}
