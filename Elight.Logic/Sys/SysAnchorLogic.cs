using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elight.Entity.Sys;
using Elight.Logic.Base;
using SqlSugar;
using Elight.Utility.Operator;
using Elight.Utility.Extension;
using SyntacticSugar;
using Elight.Utility.Model;
using Newtonsoft.Json;
using Elight.Utility.Log;

namespace Elight.Logic.Sys
{
    public class SysAnchorLogic : BaseLogic
    {

        /// <summary>
        /// 获得所有主播列表分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<SysAnchor> GetList(PageParm parm, ref int totalCount)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(parm.where))
            {
                dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(parm.where);
            }
            var result = new List<SysAnchor>();
            try
            {
                //lmstatus  连麦状态 live 直播 offline 离线  normal正常 kickline踢线  disabled禁用
                //statu 	正常unlock 禁用 lock 审核中 audit
                using (var db = GetInstance())
                {
                    //result = db.Queryable<SysAnchor>()
                    //             .WhereIF(dic.ContainsKey("Name") && !string.IsNullOrEmpty(dic["Name"].ToString()), (it) => it.username.Contains(dic["Name"].ToString()) || it.nickname.Contains(dic["Name"].ToString()))
                    //             .WhereIF(dic.ContainsKey("startTime") && !string.IsNullOrEmpty(dic["startTime"].ToString()) && !string.IsNullOrEmpty(dic["endTime"].ToString()), (it) => it.regtime >= Convert.ToDateTime(dic["startTime"]) && it.regtime <= Convert.ToDateTime(dic["endTime"]))
                    //             .WhereIF(dic.ContainsKey("isCollet") && Convert.ToInt32(dic["isCollet"]) != -1, (it) => it.isCollet == Convert.ToInt32(dic["isCollet"]))
                    //             .Select((it) => new SysAnchor
                    //             {
                    //                 id = it.id,
                    //                 username = it.username,
                    //                 nickname = it.nickname,
                    //                 photo = it.photo,
                    //                 balance = it.balance,
                    //                 atteCount = it.atteCount,
                    //                 ishot = it.ishot,
                    //                 isrecommend = it.isrecommend,
                    //                 regtime = it.regtime,
                    //                 viplevel = it.viplevel,
                    //                 birthday = it.birthday,
                    //                 lmstatus = it.lmstatus,
                    //                 isCollet = it.isCollet
                    //             }).ToPageList(parm.page, parm.limit, ref totalCount);
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "获得所有主播列表分页", ex.Message,ex.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(SysRole model)
        {
            using (var db = GetInstance())
            {
                model.Id = Guid.NewGuid().ToString().Replace("-", "");
                model.IsEnabled = model.IsEnabled == null ? "0" : "1";
                model.AllowEdit = model.AllowEdit == null ? "0" : "1";
                model.DeleteMark = "0";
                model.CreateUser = OperatorProvider.Instance.Current.Account;
                model.CreateTime = DateTime.Now;
                model.ModifyUser = model.CreateUser;
                model.ModifyTime = model.CreateTime;
                return db.Insertable<SysRole>(model).ExecuteCommand();
            }
        }

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(SysRole model)
        {
            using (var db = GetInstance())
            {
                model.IsEnabled = model.IsEnabled == null ? "0" : "1";
                model.AllowEdit = model.AllowEdit == null ? "0" : "1";
                model.ModifyUser = OperatorProvider.Instance.Current.Account;
                model.ModifyTime = DateTime.Now;
                return db.Updateable<SysRole>(model).UpdateColumns(it => new
                {
                    it.Name,
                    it.IsEnabled,
                    it.Remark,
                    it.SortCode,
                    it.ModifyUser,
                    it.ModifyTime
                }).ExecuteCommand();
            }
        }

        /// <summary>
        /// 根据主键得到角色信息
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public SysRole Get(string primaryKey)
        {
            using (var db = GetInstance())
            {
                //return db.Queryable<SysRole>().InSingle(primaryKey);
                return db.Queryable<SysRole>().Where((A) => A.Id == primaryKey).Select((A) => new SysRole
                {
                    Id = A.Id,
                    Name = A.Name,
                    DeleteMark = A.DeleteMark,
                    IsEnabled = A.IsEnabled,
                    Remark = A.Remark,
                    SortCode = A.SortCode,
                }).First();
            }
        }
        /// <summary>
        /// 删除角色信息
        /// </summary>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        public int Delete(string[] primaryKeys)
        {
            using (var db = GetInstance())
            {
                return db.Deleteable<SysRole>().In(primaryKeys).ExecuteCommand();
            }
        }
    }
}
