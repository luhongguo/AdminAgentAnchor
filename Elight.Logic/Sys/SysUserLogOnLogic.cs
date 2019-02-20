﻿using Elight.Entity.Sys;
using Elight.Logic.Base;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elight.Utility.Security;

namespace Elight.Logic.Sys
{
    public class SysUserLogOnLogic : BaseLogic
    {

        /// <summary>
        /// 根据用户Id得到登录账号信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public SysUserLogOn GetByAccount(string userId)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysUserLogOn>().Where(it => it.UserId == userId).First();
            }
        }

        /// <summary>
        /// 更新用户登录账号信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateLogin(SysUserLogOn model)
        {
            using (var db = GetInstance())
            {
                model.IsOnLine = "1";
                model.LastVisitTime = DateTime.Now;
                model.PrevVisitTime = model.LastVisitTime;
                model.LoginCount += 1;
                return db.Updateable<SysUserLogOn>(model).UpdateColumns(it => new
                {
                    it.IsOnLine,
                    it.PrevVisitTime,
                    it.LastVisitTime,
                    it.LoginCount,
                }).ExecuteCommand();
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userLoginEntity"></param>
        /// <returns></returns>
        public int ModifyPwd(SysUserLogOn userLoginEntity)
        {
            using (var db = GetInstance())
            {
                userLoginEntity.ChangePwdTime = DateTime.Now;
                return db.Updateable<SysUserLogOn>(userLoginEntity).UpdateColumns(it => new
                {
                    it.Password,
                    it.ChangePwdTime,
                }).ExecuteCommand();
            }
        }

        /// <summary>
        /// 删除用户登录信息
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public int Delete(params string[] userIds)
        {
            using (var db = GetInstance())
            {
                try
                {
                    db.Ado.BeginTran();
                    foreach (string userId in userIds)
                    {
                        db.Deleteable<SysUserLogOn>().Where(it => it.UserId == userId).ExecuteCommand();
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
        /// 新增用户登录账号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(SysUserLogOn model)
        {
            using (var db = GetInstance())
            {
                model.Id = Guid.NewGuid().ToString().Replace("-", "");
                model.SecretKey = model.Id.DESEncrypt().Substring(0, 8);
                model.Password = model.Password.MD5Encrypt().DESEncrypt(model.SecretKey).MD5Encrypt();
                model.LoginCount = 0;
                model.IsOnLine = "0";
                return db.Insertable<SysUserLogOn>(model).ExecuteCommand();
            }
        }

        /// <summary>
        /// 更新用户登录账号信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateInfo(SysUserLogOn model)
        {
            using (var db = GetInstance())
            {
                return db.Updateable<SysUserLogOn>(model).UpdateColumns(it => new
                {
                    it.AllowMultiUserOnline,
                    it.Question,
                    it.AnswerQuestion,
                    it.CheckIPAddress,
                    it.Language,
                    it.Theme
                }).ExecuteCommand();
            }
        }
    }
}
