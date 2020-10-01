using System;
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
using Elight.Utility.Model;
using System.Threading.Tasks;
using Elight.Entity.Model;

namespace Elight.WebUI.Areas.System.Controllers
{
    [LoginChecked]
    public class UserAnchorController : BaseController
    {
        private SysUserAnchorLogic sysUserAnchorLogic;
        public UserAnchorController()
        {
            sysUserAnchorLogic = new SysUserAnchorLogic();
        }
        /// <summary>
        /// 经纪人主播页面
        /// </summary>
        /// <returns></returns>
        [HttpGet, AuthorizeChecked]
        public ActionResult Distribution()
        {
            return View();
        }
        /// <summary>
        /// 经纪人新增主播页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddUserAnchor()
        {
            return View();
        }
        /// <summary>
        /// 经纪人查看主播信息展示页面
        /// </summary>
        /// <returns></returns>
        [HttpGet, AuthorizeChecked]
        public ActionResult GetAnchorInfo()
        {
            return View();
        }

        /// <summary>
        /// 经纪人名下主播
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord">查询条件</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetUserAnchorList(int pageIndex, int pageSize, PageParm pageParm)
        {
            int totalCount = 0;
            var pageData = sysUserAnchorLogic.GetUserAnchorList(pageIndex, pageSize, pageParm.where, ref totalCount);
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
            var pageData = sysUserAnchorLogic.GetUserNoOwnedAnchorList(pageIndex, pageSize, pageParm.where, ref totalCount);
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
        /// 添加主播给经纪人
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="idList">主播ID集合</param>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult addUserAnchor(string idList, string userID)
        {
            var result = sysUserAnchorLogic.AddUserAnchor(idList.ToStrArray(), userID);
            if (result)
            {
                return Success();
            }
            return Error();
        }
        /// <summary>
        /// 删除经纪人的主播
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="idList">主播ID集合</param>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(string idList, string userID)
        {
            var result = sysUserAnchorLogic.Delete(idList.ToStrArray(), userID);
            if (result)
            {
                return Success();
            }
            return Error();
        }
        /// <summary>
        /// 经纪人名下主播信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord">查询条件</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UserSelectAnchorList(PageParm parm)
        {
            int totalCount = 0;
            var pageData = sysUserAnchorLogic.UserSelectAnchorList(parm, ref totalCount);
            var result = new
            {
                code = 0,
                msg = "success",
                data = pageData,
                count = totalCount// pageData.Count
            };
            return Content(result.ToJson());
        }
        #region  金额流水
        /// <summary>
        /// 主播财务报表
        /// </summary>
        /// <returns></returns>
        [HttpGet, AuthorizeChecked]
        public ActionResult AnchorReport()
        {
            return View();
        }
        /// <summary>
        /// 获取主播财务报表信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAnchorReportPage(PageParm parm)
        {
            int totalCount = 0;
            var sumModel = new IncomeTemplateModel();
            var res = sysUserAnchorLogic.GetAnchorReportPage(parm, ref totalCount, ref sumModel);
            var result = new
            {
                code = 0,
                msg = "success",
                data = res,
                count = totalCount,// pageData.Count
                totalRow = new
                {
                    hour_income = sumModel.hour_income.ToString(),
                    agent_income = sumModel.agent_income.ToString(),
                    tip_income = sumModel.tip_income.ToString(),
                    test_income = sumModel.test_income.ToString(),
                    Balance = sumModel.Balance.ToString()
                }
            };
            return Content(result.ToJson());
        }
        /// <summary>
        /// 主播流水明细
        /// </summary>
        /// <returns></returns>
        [HttpGet, AuthorizeChecked]
        public ActionResult FlowDetails()
        {
            return View();
        }
        /// <summary>
        /// 获取主播打赏明细
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetFlowDetailsPage(PageParm parm)
        {
            int totalCount = 0;
            decimal sumTotalAmount = 0;
            var res = sysUserAnchorLogic.GetFlowDetailsPage(parm, ref totalCount, ref sumTotalAmount);
            var result = new
            {
                code = 0,
                msg = "success",
                data = res,
                count = totalCount,// pageData.Count
                totalRow = new  //合计
                {
                    totalamount = sumTotalAmount
                }
            };
            return Content(result.ToJson());
        }
        /// <summary>
        /// 主播工时
        /// </summary>
        /// <returns></returns>
        [HttpGet, AuthorizeChecked]
        public ActionResult HourDetails()
        {
            return View();
        }
        /// <summary>
        /// 主播工时明细
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetHourDetailsPage(PageParm parm)
        {
            int totalCount = 0;
            decimal sumAmount = 0;
            int sumDuration = 0;
            var res = sysUserAnchorLogic.GetHourDetailsPage(parm, ref totalCount, ref sumAmount, ref sumDuration);
            var result = new
            {
                code = 0,
                msg = "success",
                data = res,
                count = totalCount,// pageData.Count
                totalRow = new  //合计
                {
                    amount = sumAmount.ToString(),
                    duration = sumDuration.ToString()
                }
            };
            return Content(result.ToJson());
        }
        #endregion
        /// <summary>
        /// 主播下拉框 名称和昵称
        /// </summary>
        /// <returns>new {id,username}</returns>
        [HttpGet]
        public string AnchorUserNameSelect()
        {
            return sysUserAnchorLogic.AnchorUserNameSelect();
        }
        /// <summary>
        /// 主播下拉框 昵称和ID
        /// </summary>
        /// <returns>new {id,username}</returns>
        [HttpGet]
        public string AnchorUserIDSelect()
        {
            return sysUserAnchorLogic.AnchorUserIDSelect();
        }
        /// <summary>
        /// 公司代码下拉框
        /// </summary>
        /// <returns>new {id,username}</returns>
        [HttpGet]
        public string selectCompanyCodeList()
        {
            return sysUserAnchorLogic.selectCompanyCodeList();
        }
        //[HttpPost] 
        //public ActionResult GetAnchorReportPage(PageParm parm)
        //{
        //    var res = sysUserAnchorLogic.GetAnchorReportPage(parm);
        //    int totalCount = 0;
        //    var result = new
        //    {
        //        code = 0,
        //        msg = "success",
        //        data = res,
        //        count = totalCount// pageData.Count
        //    };
        //    return Content(result.ToJson());
        //}
    }
}