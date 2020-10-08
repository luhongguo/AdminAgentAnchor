using Elight.Entity.Sys;
using Elight.Logic.Base;
using Elight.Utility.Log;
using Elight.Utility.Model;
using Elight.Utility.Operator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using SqlSugar;
using System.Linq;

namespace Elight.Logic.Sys
{
    public class SysShopLogic : BaseLogic
    {
        /// <summary>
        /// 商户分页
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public List<SysShopEntity> GetShopPage(PageParm parm, ref int totalCount)
        {
            var result = new List<SysShopEntity>();
            try
            {
                if (parm == null)
                {
                    parm = new PageParm();
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(parm.where))
                {
                    dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(parm.where);
                }
                using (var db = GetSqlSugarDB(DbConnType.QPAgentAnchorDB))
                {
                    return db.Queryable<SysShopEntity>()
                                    .WhereIF(dic.ContainsKey("Name") && !string.IsNullOrEmpty(dic["Name"].ToString()), (it) => it.Name.Contains(dic["Name"].ToString()))
                                    .Select(it => it)
                                    .ToPageList(parm.page, parm.limit, ref totalCount);
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "商户分页信息", ex.Message, ex.StackTrace);
            }
            return result;
        }
        /// <summary>
        /// 根据主键得到商户信息
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public SysShopEntity Get(int primaryKey)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysShopEntity>().Where((A) => A.ID == primaryKey).First();
            }
        }
        /// <summary>
        /// 新增商户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(SysShopEntity model)
        {
            try
            {
                using (var db = GetInstance())
                {
                    model.CreateTime = DateTime.Now;
                    model.ModifiedBy = OperatorProvider.Instance.Current.Account;
                    model.ModifiedTime = model.CreateTime;
                    return db.Insertable(model).ExecuteReturnIdentity();
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "新增商户", ex.Message, ex.StackTrace);
            }
            return 0;
        }

        /// <summary>
        /// 更新商户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(SysShopEntity model)
        {
            try
            {
                using (var db = GetInstance())
                {
                    model.ModifiedBy = OperatorProvider.Instance.Current.Account;
                    model.ModifiedTime = DateTime.Now;
                    return db.Updateable(model).UpdateColumns(it => new
                    {
                        it.Name,
                        it.ModifiedBy,
                        it.ModifiedTime
                    }).ExecuteCommand();
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "修改商户", ex.Message, ex.StackTrace);
            }
            return 0;
        }
        /// <summary>
        /// 批量删除商户信息
        /// </summary>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        public int Delete(List<int> idList)
        {
            using (var db = GetInstance())
            {
                return db.Deleteable<SysShopEntity>().In(idList).ExecuteCommand();
            }
        }
        /// <summary>
        /// 获取商户下拉框
        /// </summary>
        /// <returns></returns>
        public List<SelectModel> GetShopSelectList()
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysShopEntity>().Select(it => new SelectModel
                {
                    ID = it.ID,
                    Name = it.Name
                }).ToList();
            }
        }
        /// <summary>
        /// 商户获取 拥有的主播分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<SysAnchor> GetShopAnchorList(int pageIndex, int pageSize, string keyWord, ref int totalCount)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(keyWord))
            {
                dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(keyWord);
            }
            var result = new List<SysAnchor>();
            try
            {
                //lmstatus  连麦状态 live 直播 offline 离线 disabled禁用 normal正常 kickline踢线
                //statu 	正常unlock 禁用 lock 审核中 audit
                using (var db = GetInstance())
                {
                    result = db.Queryable<SysShopAnchorEntity, SysAnchor, SysAnchorInfoEntity>((st, it, at) => new object[] { JoinType.Left, st.AnchorID == it.id, JoinType.Left, it.id == at.aid })
                                 .WhereIF(dic.ContainsKey("anchorUserName") && !string.IsNullOrEmpty(dic["anchorUserName"].ToString()), (st, it) => it.anchorName.Contains(dic["anchorUserName"].ToString()) || it.nickName.Contains(dic["anchorUserName"].ToString()))
                                 .WhereIF(dic.ContainsKey("userID") && !string.IsNullOrEmpty(dic["userID"].ToString()), (st, it) => st.ShopID == Convert.ToInt32(dic["userID"]))
                                 //.WhereIF(dic.ContainsKey("isCollet") && Convert.ToInt32(dic["isCollet"]) != -1, (st, it) => it.isCollet == Convert.ToInt32(dic["isCollet"]))
                                 .WhereIF(dic.ContainsKey("isColletCode") && dic["isColletCode"].ToString() != "-1", (st, it) => it.isColletCode == dic["isColletCode"].ToString())
                                 .Select((st, it, at) => new SysAnchor
                                 {
                                     id = it.id,
                                     anchorName = it.anchorName,
                                     nickName = it.nickName,
                                     headUrl = Image_CDN + it.headUrl,
                                     balance = at.gold,
                                     follow = at.follow,
                                     birthday = it.birthday,
                                     status = at.status,
                                     createTime = it.createTime
                                 }).ToPageList(pageIndex, pageSize, ref totalCount);
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "商户获取 拥有的主播分页", ex.Message, ex.StackTrace);
            }
            return result;
        }
        /// <summary>
        /// 商户不拥有的主播 数据分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord">查询条件</param>
        /// <param name="totalCount">数据总数</param>
        /// <returns></returns>
        public List<SysAnchor> GetShopNoOwnedAnchorList(int pageIndex, int pageSize, string keyWord, ref int totalCount)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(keyWord))
            {
                dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(keyWord);
            }
            var result = new List<SysAnchor>();
            try
            {
                //lmstatus  连麦状态 live 直播 offline 离线 disabled禁用 normal正常 kickline踢线
                //statu 	正常unlock 禁用 lock 审核中 audit
                using (var db = GetSqlSugarDB(DbConnType.QPVideoAnchorDB))
                {
                    result = db.Queryable<SysAnchor, SysAnchorInfoEntity>((it, at) => new object[] { it.id == at.aid })
                                 .WhereIF(dic.ContainsKey("anchorUserName") && !string.IsNullOrEmpty(dic["anchorUserName"].ToString()), it => it.anchorName.Contains(dic["anchorUserName"].ToString()) || it.nickName.Contains(dic["anchorUserName"].ToString()))
                                 .WhereIF(dic.ContainsKey("isColletCode") && dic["isColletCode"].ToString() != "-1", (it) => it.isColletCode == dic["isColletCode"].ToString())
                                 .Where(it => SqlFunc.Subqueryable<SysShopAnchorEntity>().Where(st => st.ShopID == Convert.ToInt32(dic["userID"])).Where(st => st.AnchorID == it.id).NotAny())
                                 .Select((it,at) => new SysAnchor
                                 {
                                     id = it.id,
                                     anchorName = it.anchorName,
                                     nickName = it.nickName,
                                     headUrl = Image_CDN + it.headUrl,
                                     balance = at.gold,
                                     follow = at.follow,
                                     birthday = it.birthday,
                                     status = at.status,
                                     createTime = it.createTime
                                 }).ToPageList(pageIndex, pageSize, ref totalCount);
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "商户不拥有的主播 数据分页", ex.Message, ex.StackTrace);
            }
            return result;
        }
        /// <summary>
        /// 添加主播给商户
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="idList">主播ID集合</param>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        public bool AddShopAnchor(string[] idList, int userID)
        {
            var result = true;
            try
            {
                using (var db = GetInstance())
                {
                    List<SysShopAnchorEntity> list = new List<SysShopAnchorEntity>();
                    foreach (var item in idList)
                    {
                        list.Add(new SysShopAnchorEntity { ShopID = userID, AnchorID = Convert.ToInt32(item), CreateTime = DateTime.Now });
                    }
                    var count = db.Insertable(list.ToArray()).ExecuteCommand();
                    result = count == idList.Count() ? true : false;
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "添加主播给经纪人", ex.Message, ex.StackTrace);
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 删除商户主播
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="idList">主播ID集合</param>
        /// <param name="userID">商户ID</param>
        /// <returns></returns>
        public bool Delete(string[] idList, int userID)
        {
            var result = true;
            try
            {
                using (var db = GetInstance())
                {
                    var count = db.Deleteable<SysShopAnchorEntity>().Where(it => it.ShopID == userID).In(it => it.AnchorID, idList).ExecuteCommand();
                    result = count == idList.Count() ? true : false;
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "删除商户主播", ex.Message, ex.StackTrace);
                result = false;
            }
            return result;
        }
    }
}
