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

namespace Elight.Logic.Sys
{
    public class SysRoleLogic : BaseLogic
    {
        /// <summary>
        /// 得到角色列表(树形)
        /// </summary>
        /// <returns></returns>
        public List<SysRole> GetList(int shopID)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysRole>().Where((A) => A.DeleteMark == "0" && A.ShopID == shopID).Select((A) => new SysRole
                {
                    Id = A.Id,
                    Name = A.Name,
                }).ToList();
            }
        }

        /// <summary>
        /// 获得角色列表分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<SysRole> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysRole, SysShopEntity>((it, at) => new object[] { JoinType.Left, it.ShopID == at.ID })
                             .WhereIF(!keyWord.IsNullOrEmpty(), it => it.Name.Contains(keyWord))
                             .Where(it => it.DeleteMark == "0")
                             .OrderBy((it, at) => at.ID).Select((it, at) => new SysRole
                             {
                                 Id = it.Id,
                                 ShopID = it.ShopID,
                                 Name = it.Name,
                                 IsEnabled = it.IsEnabled,
                                 Remark = it.Remark,
                                 SortCode = it.SortCode,
                                 ShopName = at.Name
                             }).ToPageList(pageIndex, pageSize, ref totalCount);
            }
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
                model.Type = model.Type;
                model.DeleteMark = "0";
                model.CreateUser = OperatorProvider.Instance.Current.Account;
                model.CreateTime = DateTime.Now;
                model.ModifyUser = model.CreateUser;
                model.ModifyTime = model.CreateTime;
                model.ShopID = model.ShopID;
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
                    it.ModifyTime,
                    it.Type,
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
                    ShopID = A.ShopID,
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
                try
                {
                    db.Ado.BeginTran();
                    db.Deleteable<SysRole>().In(primaryKeys).ExecuteCommand();
                    db.Deleteable<SysRoleAuthorize>().In(it => it.RoleId, primaryKeys).ExecuteCommand();//角色权限
                    db.Deleteable<SysUserRoleRelation>().In(it => it.RoleId, primaryKeys).ExecuteCommand();//用户角色
                    db.Ado.CommitTran();
                }
                catch (Exception)
                {
                    db.Ado.RollbackTran();
                    return 0;
                }
                return 1;
            }
        }
    }
}
