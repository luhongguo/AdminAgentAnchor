using Elight.Entity.Model;
using Elight.Entity.Sys;
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

namespace Elight.WebUI.Areas.System.Controllers
{
    [LoginChecked]
    public class TipIncomeDetailController : BaseController
    {
        SysTipIncomeDetailLogic sysTipIncomeDetailLogic;
        public TipIncomeDetailController()
        {
            sysTipIncomeDetailLogic = new SysTipIncomeDetailLogic();
        }
        // GET: System/TipIncomeDetail
        [HttpGet, AuthorizeChecked]
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
                    AnchorIncome = sumModel.AnchorIncome.ToString(),
                    UserIncome = sumModel.UserIncome.ToString(),
                    PlatformIncome = sumModel.PlatformIncome.ToString(),
                    totalamount = sumModel.totalamount.ToString(),
                }
            };
            return Content(result.ToJson());
        }
        public ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Form(AddTipDetailModel model)
        {
            var anchorModel = new SysUserAnchorLogic().CheckAnchorName(model.anchorName);
            if (anchorModel == null)
            {
                return Error("主播不存在!");
            }
            var agentModel = new SysAnchorRebateLogic().GetRebateByAccount(anchorModel.id);
            var addModel = new SysTipIncomeDetailEntity
            {
                ShopID = 0,
                AnchorID = anchorModel.id,
                AnchorIncome = model.money,
                StartDate = DateTime.Now,
                UserID = agentModel == null ? null : agentModel.parentID,
                TipType = Entity.Enum.TipTypeEnum.其他,
                IncomeType = Entity.Enum.IncomeTypeEnum.手动添加,
                CreateTime = DateTime.Now
            };
            var result = sysTipIncomeDetailLogic.AddTipIncome(addModel);
            return result ? Success() : Error();
        }
    }
}