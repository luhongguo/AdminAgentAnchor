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
    public class SysAnchorBankLogic : BaseLogic
    {
        /// <summary>
        /// 主播银行卡分页
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public List<SysAnchorBankEntity> GetAgentBankPage(PageParm parm, ref int totalCount)
        {
            var result = new List<SysAnchorBankEntity>();
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
                    return db.Queryable<SysAnchorBankEntity, SysAnchor,SysShopAnchorEntity>((it, st,ot) => new object[] { JoinType.Left, it.AnchorID == st.id,JoinType.Left,st.id==ot.AnchorID })
                                    .WhereIF(dic.ContainsKey("Name") && !string.IsNullOrEmpty(dic["Name"].ToString()), (it, st) => st.anchorName.Contains(dic["Name"].ToString()) || st.nickName.Contains(dic["Name"].ToString()))
                                     .WhereIF(dic.ContainsKey("ShopID") && Convert.ToInt32(dic["ShopID"]) != -1, (it, st, ot) => ot.ShopID == Convert.ToInt32(dic["ShopID"]))
                                    .Select((it, st) => new SysAnchorBankEntity
                                    {
                                        id = it.id,
                                        AgentName = st.anchorName,
                                        NickName = st.nickName,
                                        address = it.address,
                                        bankaccount = it.bankaccount,
                                        bankano = it.bankano,
                                        CategoryCode = it.CategoryCode,
                                        createtime = it.createtime,
                                        payType = it.payType,
                                        ImgUrl = Image_CDN + it.ImgUrl
                                    })
                                    .WithCache(60)
                                    .ToPageList(parm.page, parm.limit, ref totalCount);
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "主播银行卡分页", ex.Message, ex.StackTrace);
            }
            return result;
        }
        /// <summary>
        /// 根据主键得到商户信息
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public SysAnchorBankEntity Get(long primaryKey)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysAnchorBankEntity, SysAnchor>((A, B) => new Object[] { JoinType.Left, A.AnchorID == B.id })
                    .Where((A) => A.id == primaryKey)
                    .Select((A, B) => new SysAnchorBankEntity
                    {
                        id = A.id,
                        AgentName = B.anchorName,
                        NickName = B.nickName,
                        address = A.address,
                        bankaccount = A.bankaccount,
                        bankano = A.bankano,
                        CategoryCode = A.CategoryCode,
                        payType = A.payType
                    })
                    .First();
            }
        }
        /// <summary>
        /// 新增主播银行卡
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(SysAnchorBankEntity model)
        {
            try
            {
                using (var db = GetInstance())
                {
                    model.createtime = DateTime.Now;
                    return db.Insertable(model).ExecuteReturnIdentity();
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "新增主播银行卡", ex.Message, ex.StackTrace);
            }
            return 0;
        }

        /// <summary>
        /// 更新主播银行卡
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(SysAnchorBankEntity model)
        {
            try
            {
                using (var db = GetInstance())
                {
                    return db.Updateable<SysAnchorBankEntity>().SetColumns(it => new SysAnchorBankEntity
                    {
                        bankano = model.bankano,
                        CategoryCode = model.CategoryCode,
                        payType = model.payType,
                        bankaccount = model.bankaccount,
                        address = model.address
                    }).Where(it => it.id == model.id).ExecuteCommand();
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "更新主播银行卡", ex.Message, ex.StackTrace);
            }
            return 0;
        }
        /// <summary>
        /// 批量删除银行卡信息
        /// </summary>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        public int Delete(List<long> idList)
        {
            using (var db = GetInstance())
            {
                return db.Deleteable<SysAnchorBankEntity>().In(idList).ExecuteCommand();
            }
        }
        /// <summary>
        /// 经纪人银行卡下拉框
        /// </summary>
        /// <param name="id">经纪人id</param>
        /// <returns></returns>
        public string GetUserBankSelect(int id)
        {
            var result = "";
            try
            {
                using (var db = GetInstance())
                {
                    result = db.Queryable<SysAnchorBankEntity>()
                        .Where(it => it.AnchorID == id)
                                .Select((it) => new
                                {
                                    BankName = it.CategoryCode + "--" + it.bankano,
                                    it.id,
                                }).ToJson();
                }
            }
            catch (Exception ex)
            {
                new LogLogic().Write(Level.Error, "经纪人银行卡下拉框", ex.Message, ex.StackTrace);
            }
            return result;
        }
    }
}
