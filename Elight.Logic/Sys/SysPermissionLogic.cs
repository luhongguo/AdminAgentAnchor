using Elight.Entity.Sys;
using Elight.Logic.Base;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elight.Utility.Web;
using Elight.Utility.Operator;
using Elight.Utility.Extension;

namespace Elight.Logic.Sys
{
    /// <summary>
    /// 权限处理
    /// </summary>
    public class SysPermissionLogic : BaseLogic
    {

        public bool ActionValidate(string userId, string action)
        {
            var authorizeModules = GetList(userId);
            foreach (var item in authorizeModules)
            {
                if (!string.IsNullOrEmpty(item.Url))
                {
                    string[] url = item.Url.Split('?');
                    if (url[0].ToLower() == action.ToLower())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 根据用户ID获取权限集合
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<SysPermission> GetList(string userId)
        {
            using (var db = GetInstance())
            {
                return
                    db.Queryable<SysUserRoleRelation, SysRoleAuthorize, SysPermission>((A, B, C) => new object[] {
                      JoinType.Left,A.RoleId == B.RoleId,
                      JoinType.Left,C.Id == B.ModuleId,
                    })
                    .Where((A, B, C) => A.UserId == userId && C.IsEnable == "1" && C.DeleteMark == "0" && C.ShopID == OperatorProvider.Instance.Current.ShopID)
                    .OrderBy((A, B, C) => C.SortCode)
                    .Select((A, B, C) => new SysPermission
                    {
                        Id = C.Id,
                        ParentId = C.ParentId,
                        Layer = C.Layer,
                        EnCode = C.EnCode,
                        Name = C.Name,
                        JsEvent = C.JsEvent,
                        Icon = C.Icon,
                        Url = C.Url,
                        Remark = C.Remark,
                        Type = C.Type,
                        SortCode = C.SortCode,
                        IsEnable = C.IsEnable,
                        DeleteMark = C.DeleteMark,
                        CreateUser = C.CreateUser,
                        CreateTime = C.CreateTime,
                        ModifyUser = C.ModifyUser,
                        ModifyTime = C.ModifyTime
                    }).WithCache(60).ToList();
            }
        }

        public List<SysPermission> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount)
        {
            using (var db = GetInstance())
            {
                if (keyWord.IsNullOrEmpty())
                {
                    totalCount = db.Queryable<SysPermission>().Where(it => it.DeleteMark == "0" && it.ShopID == OperatorProvider.Instance.Current.ShopID).Count();
                    return db.Queryable<SysPermission>().Where(it => it.DeleteMark == "0" && it.ShopID == OperatorProvider.Instance.Current.ShopID).OrderBy(it => it.SortCode).ToPageList(pageIndex, pageSize);
                }
                totalCount = db.Queryable<SysPermission>().Where(it => it.DeleteMark == "0" && it.ShopID == OperatorProvider.Instance.Current.ShopID && (it.Name.Contains(keyWord) || it.EnCode.Contains(keyWord))).Count();
                return db.Queryable<SysPermission>().Where(it => it.DeleteMark == "0" && it.ShopID == OperatorProvider.Instance.Current.ShopID && (it.Name.Contains(keyWord) || it.EnCode.Contains(keyWord))).OrderBy(it => it.SortCode).ToPageList(pageIndex, pageSize);
            }
        }

        public int Delete(params string[] primaryKeys)
        {
            using (var db = GetInstance())
            {

                try
                {
                    db.Ado.BeginTran();
                    //删除权限与角色的对应关系。
                    List<string> list = db.Queryable<SysPermission>().In(primaryKeys).Select(it => it.Id).ToList();
                    db.Deleteable<SysPermission>().In(primaryKeys).ExecuteCommand();
                    db.Deleteable<SysRoleAuthorize>().Where(it => list.Contains(it.ModuleId)).ExecuteCommand();
                    db.Ado.CommitTran();
                    return 1;
                }
                catch (Exception ex)
                {
                    db.Ado.RollbackTran();
                    return 0;
                }
            }

        }

        public int GetChildCount(string parentId)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysPermission>().Where(it => it.ParentId == parentId).ToList().Count();
            }
        }
        /// <summary>
        /// 根据商户获取权限集合
        /// </summary>
        /// <param name="shopID">商户ID</param>
        /// <returns></returns>
        public List<SysPermission> GetShopPowersList(int shopID)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysPermission>().Where(it => it.DeleteMark == "0" && it.ShopID == shopID).OrderBy(it => it.SortCode).ToList();
            }
        }

        public SysPermission Get(string primaryKey)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysPermission>().InSingle(primaryKey);
            }
        }


        public int Insert(SysPermission model)
        {
            using (var db = GetInstance())
            {
                model.Id = Guid.NewGuid().ToString().Replace("-", "");
                //最高级菜单id为1
                model.Layer = model.ParentId == "1" ? 0 : Get(model.ParentId).Layer += 1;
                model.IsEnable = model.IsEnable == null ? "0" : "1";
                model.IsEdit = "1";
                model.IsPublic = "1";
                model.DeleteMark = "0";
                model.CreateUser = OperatorProvider.Instance.Current.Account;
                model.CreateTime = DateTime.Now;
                model.ModifyUser = model.CreateUser;
                model.ModifyTime = model.CreateTime;
                model.ShopID = OperatorProvider.Instance.Current.ShopID;
                return db.Insertable<SysPermission>(model).ExecuteCommand();
            }
        }

        public int Update(SysPermission model)
        {
            using (var db = GetInstance())
            {
                model.Layer = model.ParentId == "1" ? 0 : Get(model.ParentId).Layer += 1;
                model.IsEnable = model.IsEnable == null ? "0" : "1";
                //model.IsEdit = model.IsEdit == null ? "0" : "1";
                //model.IsPublic = model.IsPublic == null ? "0" : "1";
                model.ModifyUser = OperatorProvider.Instance.Current.Account;
                model.ModifyTime = DateTime.Now;
                return db.Updateable<SysPermission>(model).UpdateColumns(it => new
                {
                    it.ParentId,
                    it.Layer,
                    it.EnCode,
                    it.Name,
                    it.JsEvent,
                    it.Icon,
                    it.Url,
                    it.Remark,
                    it.Type,
                    it.SortCode,
                    it.IsEnable,
                    it.ModifyUser,
                    it.ModifyTime,
                }).ExecuteCommand();
            }
        }
    }
}
