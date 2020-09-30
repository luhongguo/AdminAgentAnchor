using Elight.Entity.Sys;
using Elight.Logic.Base;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elight.Utility.Operator;
using Elight.Utility.Security;
using Elight.Utility.Extension;

namespace Elight.Logic.Sys
{
    public class SysUserLogic : BaseLogic
    {
        /// <summary>
        /// 根据账号得到用户信息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public SysUser GetByUserName(string account)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysUser>().Where((A) => A.Account == account && A.ShopID == 0).Select((A) => new SysUser
                {
                    Id = A.Id,
                    Account = A.Account,
                    RealName = A.RealName,
                    CompanyCode = A.CompanyCode,
                    Avatar = A.Avatar,
                    Gender = A.Gender,
                    Birthday = A.Birthday,
                    MobilePhone = A.MobilePhone,
                    Email = A.Email,
                    Signature = A.Signature,
                    Address = A.Address,
                    CompanyId = A.CompanyId,
                    IsEnabled = A.IsEnabled,
                    SortCode = A.SortCode,
                    DepartmentId = A.DepartmentId,
                    DeleteMark = A.DeleteMark,
                    CreateUser = A.CreateUser,
                    CreateTime = A.CreateTime,
                    ModifyUser = A.ModifyUser,
                    ModifyTime = A.ModifyTime,
                }).First();
            }
        }

        /// <summary>
        /// 修改用户基础信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateBasicInfo(SysUser model)
        {
            using (var db = GetInstance())
            {
                model.ModifyUser = OperatorProvider.Instance.Current.Account;
                model.ModifyTime = DateTime.Now;
                return db.Updateable<SysUser>(model).UpdateColumns(it => new
                {
                    it.RealName,
                    it.Gender,
                    it.Birthday,
                    it.MobilePhone,
                    it.Avatar,
                    it.Email,
                    it.Signature,
                    it.Address,
                    it.ModifyUser,
                    it.ModifyTime
                }).ExecuteCommand();
            }
        }

        public int Insert(SysUser model, string password, string[] roleIds)
        {
            using (var db = GetInstance())
            {
                try
                {
                    db.Ado.BeginTran();
                    ////新增用户基本信息。
                    model.Id = Guid.NewGuid().ToString().Replace("-", "");
                    model.DeleteMark = "0";
                    model.CreateUser = OperatorProvider.Instance.Current.Account;
                    model.CreateTime = DateTime.Now;
                    model.ModifyUser = model.CreateUser;
                    model.ModifyTime = model.CreateTime;
                    model.Avatar = "/Content/framework/images/avatar.png";
                    int row = db.Insertable<SysUser>(model).ExecuteCommand();
                    if (row == 0)
                    {
                        db.Ado.RollbackTran();
                        return row;
                    }
                    //新增新的角色
                    List<SysUserRoleRelation> list = new List<SysUserRoleRelation>();
                    foreach (string roleId in roleIds)
                    {
                        SysUserRoleRelation roleRelation = new SysUserRoleRelation
                        {
                            Id = Guid.NewGuid().ToString().Replace("-", ""),
                            UserId = model.Id,
                            RoleId = roleId,
                            CreateUser = OperatorProvider.Instance.Current.Account,
                            CreateTime = DateTime.Now
                        };
                        list.Add(roleRelation);
                    }
                    row = db.Insertable<SysUserRoleRelation>(list).ExecuteCommand();
                    if (row == 0)
                    {
                        db.Ado.RollbackTran();
                        return row;
                    }
                    //新增用户登陆信息。
                    SysUserLogOn userLogOnEntity = new SysUserLogOn();
                    userLogOnEntity.Id = Guid.NewGuid().ToString().Replace("-", "");
                    userLogOnEntity.UserId = model.Id;
                    userLogOnEntity.SecretKey = userLogOnEntity.Id.DESEncrypt().Substring(0, 8);
                    userLogOnEntity.Password = password.MD5Encrypt().DESEncrypt(userLogOnEntity.SecretKey).MD5Encrypt();
                    userLogOnEntity.LoginCount = 0;
                    userLogOnEntity.IsOnLine = "0";
                    row = db.Insertable<SysUserLogOn>(userLogOnEntity).ExecuteCommand();
                    if (row == 0)
                    {
                        db.Ado.RollbackTran();
                        return row;
                    }
                    db.Ado.CommitTran();
                    return row;
                }
                catch
                {
                    db.Ado.RollbackTran();
                    return 0;
                }


            }
        }



        /// <summary>
        /// 根据主键得到用户信息
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public SysUser Get(string primaryKey)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysUser>().Where((A) => A.Id == primaryKey).Select((A) => new SysUser
                {
                    Id = A.Id,
                    Account = A.Account,
                    RealName = A.RealName,
                    Gender = A.Gender,
                    Birthday = A.Birthday,
                    Address = A.Address,
                    Avatar = A.Avatar,
                    MobilePhone = A.MobilePhone,
                    IsEnabled = A.IsEnabled,
                    CompanyCode = A.CompanyCode,
                    Email = A.Email,
                    Signature = A.Signature
                }).First();
            }
        }

        /// <summary>
        /// 获得用户列表分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord">查询参数</param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<SysUser> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysUser,SysShopEntity>((A,B)=>new object[] {JoinType.Left,A.ShopID==B.ID })
                         .WhereIF(!keyWord.IsNullOrEmpty(), it => (it.Account.Contains(keyWord) || it.RealName.Contains(keyWord)))
                         .Where((A) => A.DeleteMark == "0").OrderBy((A) => A.SortCode).Select((A,B) => new SysUser
                         {
                             Id = A.Id,
                             Account = A.Account,
                             RealName = A.RealName,
                             Avatar = A.Avatar,
                             CompanyCode = A.CompanyCode,
                             IsEnabled = A.IsEnabled,
                             ShopName=B.Name
                         }).ToPageList(pageIndex, pageSize, ref totalCount);
            }
        }

        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        public int Delete(params string[] primaryKeys)
        {
            using (var db = GetInstance())
            {
                try
                {
                    db.Ado.BeginTran();
                    foreach (string primaryKey in primaryKeys)
                    {
                        db.Deleteable<SysUser>().Where(it => it.Id == primaryKey).ExecuteCommand();
                    }
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

        /// <summary>
        /// 新增用户基础信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(SysUser model)
        {
            using (var db = GetInstance())
            {
                model.Id = Guid.NewGuid().ToString().Replace("-", "");
                model.DeleteMark = "0";
                model.CreateUser = OperatorProvider.Instance.Current.Account;
                model.CreateTime = DateTime.Now;
                model.ModifyUser = model.CreateUser;
                model.ModifyTime = model.CreateTime;
                model.Avatar = "/Content/framework/images/avatar.png";
                return db.Insertable<SysUser>(model).ExecuteCommand();
            }
        }
        /// <summary>
        /// 更新用户基础信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(SysUser model)
        {
            using (var db = GetInstance())
            {
                model.ModifyUser = OperatorProvider.Instance.Current.Account;
                model.ModifyTime = DateTime.Now;

                return db.Updateable<SysUser>(model).UpdateColumns(it => new
                {
                    it.RealName,
                    it.Birthday,
                    it.Gender,
                    it.Email,
                    it.DepartmentId,
                    it.RoleId,
                    it.MobilePhone,
                    it.Address,
                    it.Signature,
                    it.SortCode,
                    it.IsEnabled,
                    it.ModifyUser,
                    it.ModifyTime
                }).ExecuteCommand();
            }
        }

        public int UpdateAndSetRole(SysUser model, string password, string[] roleIds)
        {
            using (var db = GetInstance())
            {
                try
                {
                    db.Ado.BeginTran();
                    model.ModifyUser = OperatorProvider.Instance.Current.Account;
                    model.ModifyTime = DateTime.Now;
                    int row = db.Updateable<SysUser>(model).UpdateColumns(it => new
                    {
                        it.Account,
                        it.RealName,
                        it.RoleId,
                        it.MobilePhone,
                        it.IsEnabled,
                        it.ModifyUser,
                        it.ModifyTime
                    }).ExecuteCommand();
                    if (row == 0)
                    {
                        db.Ado.RollbackTran();
                        return row;
                    }
                    //修改密码
                    if (!string.IsNullOrEmpty(password))
                    {
                        var logOnModel = db.Queryable<SysUserLogOn>().Where(it => it.UserId == model.Id).First();
                        logOnModel.Password = password.MD5Encrypt().DESEncrypt(logOnModel.SecretKey).MD5Encrypt();
                        row = db.Updateable<SysUserLogOn>(logOnModel).UpdateColumns(it => new
                        {
                            it.Password
                        }).ExecuteCommand();
                        if (row == 0)
                        {
                            db.Ado.RollbackTran();
                            return row;
                        }
                    }
                    //删除原来的角色
                    row = db.Deleteable<SysUserRoleRelation>().Where(it => it.UserId == model.Id).ExecuteCommand();
                    if (row == 0)
                    {
                        db.Ado.RollbackTran();
                        return row;
                    }
                    //新增新的角色
                    List<SysUserRoleRelation> list = new List<SysUserRoleRelation>();
                    foreach (string roleId in roleIds)
                    {
                        SysUserRoleRelation roleRelation = new SysUserRoleRelation
                        {
                            Id = Guid.NewGuid().ToString().Replace("-", ""),
                            UserId = model.Id,
                            RoleId = roleId,
                            CreateUser = OperatorProvider.Instance.Current.Account,
                            CreateTime = DateTime.Now
                        };
                        list.Add(roleRelation);
                    }
                    row = db.Insertable<SysUserRoleRelation>(list).ExecuteCommand();
                    if (row == 0)
                    {
                        db.Ado.RollbackTran();
                        return row;
                    }
                    db.Ado.CommitTran();
                    return row;
                }
                catch
                {
                    db.Ado.RollbackTran();
                    return 0;
                }
            }
        }
    }
}
