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
    public class AgentWithdrawalRecordController : BaseController
    {
        private readonly SysAgentWithdrawalRecordLogic sysAgentWithdrawalRecordLogic;
        public AgentWithdrawalRecordController()
        {
            sysAgentWithdrawalRecordLogic = new SysAgentWithdrawalRecordLogic();
        }
        // GET: System/AgentWithdrawalRecord
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
            var res = sysAgentWithdrawalRecordLogic.GetAgentWithdrawalRecordPage(parm, ref totalCount);
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
        public ActionResult Form(SysAgentWithdrawalRecordEntity model)
        {
            int row = sysAgentWithdrawalRecordLogic.Insert(model);
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
            SysAgentWithdrawalRecordEntity entity = sysAgentWithdrawalRecordLogic.Get(primaryKey);
            return Content(entity.ToJson());
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, AuthorizeChecked, ValidateAntiForgeryToken]
        public ActionResult Handle(SysAgentWithdrawalRecordEntity model)
        {
            int row ;
            var withModel = sysAgentWithdrawalRecordLogic.Get(model.id);
            if (withModel.Status != 3)
            {
                return Error("已经处理，不可重复处理!");
            }
            if (withModel.Status == 2)//驳回
            {
                row = sysAgentWithdrawalRecordLogic.Reject(model);
            }
            else//成功
            {
                if (model.WithdrawalAmount <= 0)
                {
                    return Error("提现金额需要大于0!");
                }
                var agentModel = new SysUserLogic().GetkUserByID(withModel.AgentID);
                if (agentModel.Balance < model.WithdrawalAmount)
                {
                    return Error("提现金额不可大于余额!可提现余额：" + agentModel.Balance);
                }
                row = sysAgentWithdrawalRecordLogic.Update(model, agentModel);
            }
            return row > 0 ? Success() : Error();
        }
    }
}