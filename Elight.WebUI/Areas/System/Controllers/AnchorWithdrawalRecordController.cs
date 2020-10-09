using Elight.Entity;
using Elight.Entity.Sys;
using Elight.Logic.Sys;
using Elight.Utility.Format;
using Elight.Utility.Model;
using Elight.WebUI.Controllers;
using Elight.WebUI.Filters;
using System.Web.Mvc;

namespace Elight.WebUI.Areas.System.Controllers
{
    [LoginChecked]
    public class AnchorWithdrawalRecordController : BaseController
    {
        private readonly SysAnchorWithdrawalRecordLogic sysAnchorWithdrawalRecordLogic;
        public AnchorWithdrawalRecordController()
        {
            sysAnchorWithdrawalRecordLogic = new SysAnchorWithdrawalRecordLogic();
        }
        // GET: System/AgentWithdrawalRecord
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取提现记录
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord">查询条件</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAgentWithdrawalRecordPage(PageParm parm)
        {
            int totalCount = 0;
            var res = sysAnchorWithdrawalRecordLogic.GetAgentWithdrawalRecordPage(parm, ref totalCount);
            return pageSuccess(res, totalCount);
        }
        /// <summary>
        /// 提现页面
        /// </summary>
        /// <returns></returns>
        [HttpGet, AuthorizeChecked]
        public ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, AuthorizeChecked, ValidateAntiForgeryToken]
        public ActionResult Form(SysAnchorWithdrawalRecordEntity model)
        {
            int row = sysAnchorWithdrawalRecordLogic.Insert(model);
            return row > 0 ? Success() : Error();
        }

        /// <summary>
        /// 处理提现页面
        /// </summary>
        /// <returns></returns>
        [HttpGet, AuthorizeChecked]
        public ActionResult Handle()
        {
            return View();
        }
        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name = "primaryKey" ></ param >
        /// < returns ></ returns >
        [HttpPost]
        public ActionResult GetForm(long primaryKey)
        {
            SysAnchorWithdrawalRecordEntity entity = sysAnchorWithdrawalRecordLogic.Get(primaryKey);
            return Content(entity.ToJson());
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, AuthorizeChecked, ValidateAntiForgeryToken]
        public ActionResult Handle(SysAnchorWithdrawalRecordEntity model)
        {
            int row;
            var withModel = sysAnchorWithdrawalRecordLogic.Get(model.id);
            if (withModel.Status != 3)
            {
                return Error("已经处理，不可重复处理!");
            }
            if (withModel.Status == 2)//驳回
            {
                row = sysAnchorWithdrawalRecordLogic.Reject(model);
            }
            else//成功
            {
                if (model.WithdrawalAmount <= 0)
                {
                    return Error("提现金额需要大于0!");
                }
                var agentModel = new SysUserAnchorLogic().GetAnchorBalance(withModel.AnchorID);
                if (agentModel.gold < model.WithdrawalAmount)
                {
                    return Error("提现金额不可大于余额!可提现余额：" + agentModel.gold);
                }
                row = sysAnchorWithdrawalRecordLogic.Update(model, agentModel);
            }
            return row > 0 ? Success() : Error();
        }
    }
}