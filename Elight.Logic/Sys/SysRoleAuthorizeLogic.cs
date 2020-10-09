using Elight.Entity.Sys;
using Elight.Logic.Base;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elight.Utility.Operator;
using Elight.Utility.Log;

namespace Elight.Logic.Sys
{
    public class SysRoleAuthorizeLogic : BaseLogic
    {

        /// <summary>
        /// 获得角色权限关系
        /// </summary>
        /// <returns></returns>
        public List<SysRoleAuthorize> GetList()
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysRoleAuthorize>().ToList();
            }
        }

        /// <summary>
        /// 根据角色ID获得角色权限关系
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<SysRoleAuthorize> GetList(string roleId)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysRoleAuthorize>().Where(it => it.RoleId == roleId).ToList();
            }
        }

        /// <summary>
        /// 给某个角色授权
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="perIds"></param>
        public void Authorize(string roleId, params string[] perIds)
        {
            using (var db = GetInstance())
            {
                //a.角色需要重新设置的权限ID集合。
                var listNewPerIds = perIds.ToList();
                //b.角色原有的授权信息。
                var listOldPers = GetList(roleId);


                //c.删除角色新设置和原有授权信息集合中相同的记录。
                for (int i = listOldPers.Count - 1; i >= 0; i--)
                {
                    if (listNewPerIds.Contains(listOldPers[i].ModuleId))
                    {
                        listNewPerIds.Remove(listOldPers[i].ModuleId);
                        listOldPers.Remove(listOldPers[i]);
                    }
                }
                //事物处理
                try
                {
                    db.Ado.BeginTran();
                    //d.新集合中剩下的授权信息新增到数据库。
                    listNewPerIds.ForEach((perId) =>
                    {
                        db.Insertable<SysRoleAuthorize>(new SysRoleAuthorize()
                        {
                            RoleId = roleId,
                            ModuleId = perId,
                            Id = Guid.NewGuid().ToString().Replace("-", ""),
                            CreateUser = OperatorProvider.Instance.Current.Account,
                            CreateTime = DateTime.Now
                        }).ExecuteCommand();
                    });

                    //e.旧集合中剩下的授权信息从数据库中删除。
                    listOldPers.ForEach((perObj) =>
                    {
                        db.Deleteable<SysRoleAuthorize>(perObj).ExecuteCommand();
                    });
                    db.Ado.CommitTran();
                }
                catch (Exception ex)
                {
                    db.Ado.RollbackTran();
                }
            }
        }
        /// <summary>
        /// 给某个商户授权
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="perIds"></param>
        public void ShopAuthorize(int shopID, params string[] perIds)
        {
            using (var db = GetInstance())
            {
                //a.商户需要重新设置的权限ID集合。
                var listNewPerIds = perIds.ToList();
                var listNewPermission = db.Queryable<SysPermission>().In(listNewPerIds).ToList();
                var newEncodeList = listNewPermission.Select(it => it.EnCode).ToList(); ;
                //b.商户原有的授权信息。
                var listOldPers = new SysPermissionLogic().GetShopPowersList(shopID).Select(it => new { it.EnCode, it.Id }).ToList();
                var encodeList = listOldPers.Select(it => it.EnCode).ToList();
                //c.找到原有权限不存在的id集合 删除他
                var removePerIDList = new List<string>();
                for (int i = encodeList.Count - 1; i >= 0; i--)
                {
                    if (!newEncodeList.Contains(encodeList[i]))
                    {
                        removePerIDList.Add(listOldPers.Where(it => it.EnCode == encodeList[i]).FirstOrDefault().Id);
                    }
                }
                //事物处理
                try
                {
                    db.Ado.BeginTran();
                    //d.新集合中剩下的授权信息新增到数据库。
                    var addList = new List<SysPermission>();
                    listNewPermission.FindAll(it => it.Layer == 0).ToList().ForEach(it =>
                    {
                        var menuId = it.Id;
                        var menu = new SysPermission() { };
                        //判断以前是否存在  不存在新增  存在的话下级的ParentId不变
                        if (!encodeList.Contains(it.EnCode))
                        {
                            menu = it;
                            menu.ShopID = shopID;
                            menu.Id = Guid.NewGuid().ToString().Replace("-", "");
                            menu.CreateTime = DateTime.Now;
                            addList.Add(menu);
                        }
                        else//存在就获取以前的id
                        {
                            menu.Id = listOldPers.Where(A => A.EnCode == it.EnCode).FirstOrDefault().Id;
                        }
                        //获取列表 Layer=1
                        listNewPermission.FindAll(gt => gt.ParentId == menuId).ToList().ForEach(gt =>
                        {
                            var layerOne = gt.Id;
                            var modelLayerOne = new SysPermission() { };
                            //判断以前是否存在  不存在新增
                            if (!encodeList.Contains(gt.EnCode))
                            {
                                modelLayerOne = gt;
                                modelLayerOne.ParentId = menu.Id;
                                modelLayerOne.ShopID = shopID;
                                modelLayerOne.Id = Guid.NewGuid().ToString().Replace("-", "");
                                modelLayerOne.CreateTime = DateTime.Now;
                                addList.Add(modelLayerOne);
                            }
                            else
                            {
                                modelLayerOne.Id = listOldPers.Where(B => B.EnCode == gt.EnCode).FirstOrDefault().Id;
                            }
                            //获取按钮 Layer=2
                            listNewPermission.FindAll(st => st.ParentId == layerOne).ToList().ForEach(st =>
                            {
                                var modelLayerTwo = new SysPermission() { };
                                //判断以前是否存在  不存在新增
                                if (!encodeList.Contains(st.EnCode))
                                {
                                    modelLayerTwo = st;
                                    modelLayerTwo.ParentId = modelLayerOne.Id;
                                    modelLayerTwo.ShopID = shopID;
                                    modelLayerTwo.Id = Guid.NewGuid().ToString().Replace("-", "");
                                    menu.CreateTime = DateTime.Now;
                                    addList.Add(modelLayerTwo);
                                }
                            });
                        });
                    });
                    db.Insertable(addList).ExecuteCommand();
                    //不存在的就删除
                    //批量删除商户权限 ,商户角色权限
                    db.Deleteable<SysRoleAuthorize>().Where(it => removePerIDList.Contains(it.ModuleId)).ExecuteCommand();
                    db.Deleteable<SysPermission>().In(removePerIDList).ExecuteCommand();
                    db.Ado.CommitTran();
                }
                catch (Exception ex)
                {
                    db.Ado.RollbackTran();
                    new LogLogic().Write(Level.Error, "给某个商户授权", ex.Message, ex.StackTrace);
                }
            }
        }
        /// <summary>
        /// 从角色权限关系中删除某个模块
        /// </summary>
        /// <param name="moduleIds"></param>
        /// <returns></returns>
        public int Delete(params string[] moduleIds)
        {
            using (var db = GetInstance())
            {
                try
                {
                    db.Ado.BeginTran();
                    foreach (string moduleId in moduleIds)
                    {
                        db.Deleteable<SysRoleAuthorize>().Where(it => it.ModuleId == moduleId).ExecuteCommand();
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

    }
}
