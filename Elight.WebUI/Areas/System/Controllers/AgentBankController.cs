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
    public class AgentBankController : BaseController
    {
        private readonly SysAgentBankLogic sysAgentBankLogic;
        public AgentBankController()
        {
            sysAgentBankLogic = new SysAgentBankLogic();
        }
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取代理银行卡
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord">查询条件</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAgentBankPage(PageParm parm)
        {
            int totalCount = 0;
            var res = sysAgentBankLogic.GetAgentBankPage(parm, ref totalCount);
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
        public ActionResult Form(SysAgentBankEntity model)
        {
            if (model.id == 0)
            {
                var agentModel = new SysUserLogic().CheckUserName(model.AgentName);
                if (agentModel == null)
                {
                    return Error("经纪人不存在!");
                }
                model.AgentID = agentModel.Id;
                int row = sysAgentBankLogic.Insert(model);
                return row > 0 ? Success() : Error();
            }
            else
            {
                int row = sysAgentBankLogic.Update(model);
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
            SysAgentBankEntity entity = sysAgentBankLogic.Get(primaryKey);
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
            List<long> idlist = new List<long>();
            idArr.ForEach(it =>
            {
                idlist.Add(Convert.ToInt64(it));
            });
            int row = sysAgentBankLogic.Delete(idlist);
            return row > 0 ? Success() : Error();
        }
        /// <summary>
        /// 经纪人银行卡
        /// </summary>
        /// <returns>new {id,username}</returns>
        [HttpGet]
        public string GetUserBankSelect(string id)
        {
            return sysAgentBankLogic.GetUserBankSelect(id);
        }
    }
}