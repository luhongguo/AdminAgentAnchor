using Elight.Entity.Model;
using Elight.Logic.Sys;
using Elight.Utility.Format;
using Elight.Utility.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Elight.WebUI.Areas.System.Controllers
{
    public class TipIncomeDetailController : Controller
    {
        SysTipIncomeDetailLogic sysTipIncomeDetailLogic;
        public TipIncomeDetailController()
        {
            sysTipIncomeDetailLogic = new SysTipIncomeDetailLogic();
        }
        // GET: System/TipIncomeDetail
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取打赏礼物 返点明细
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetFlowDetailsPage(PageParm parm)
        {
            int totalCount = 0;
            TipIncomeDetailModel sumModel = new TipIncomeDetailModel();
            var res = sysTipIncomeDetailLogic.GetTipIncomeDetailPage(parm, ref totalCount, ref sumModel);
            var result = new
            {
                code = 0,
                msg = "success",
                data = res,
                count = totalCount,// pageData.Count
                totalRow = new  //合计
                {
                    AnchorIncome=sumModel.AnchorIncome.ToString(),
                    UserIncome=sumModel.UserIncome.ToString(),
                    PlatformIncome =sumModel.PlatformIncome.ToString(),
                    totalamount =sumModel.totalamount.ToString(),
                }
            };
            return Content(result.ToJson());
        }
    }
}