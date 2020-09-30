using Elight.Logic.Sys;
using Elight.Utility.Format;
using Elight.Utility.Model;
using Elight.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Elight.WebUI.Areas.System.Controllers
{
    public class RebateController : BaseController
    {
        private SysRebateLogic sysRebateLogic;
        public RebateController()
        {
            sysRebateLogic = new SysRebateLogic();
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
        public ActionResult GetRebateList(PageParm parm)
        {
            int totalCount = 0;
            var res = sysRebateLogic.GetHourDetailsPage(parm, ref totalCount);
            return pageSuccess(res,totalCount);
        }
    }
}