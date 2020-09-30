using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elight.Utility.Extension.DataCache;
using Elight.Utility.Extension.SqlSugar;
using Elight.Utility.Operator;

namespace Elight.Logic.Base
{
    public class BaseLogic
    {
        public static readonly bool _RedisCache = Convert.ToBoolean(ConfigurationManager.AppSettings["RedisCache"]);
        public static readonly string _RedisHost = ConfigurationManager.AppSettings["RedisHost"];
        public static readonly int _RedisPort = Convert.ToInt32(ConfigurationManager.AppSettings["RedisPort"]);
        public static readonly string QPAgentAnchorDB = ConfigurationManager.AppSettings["QPAgentAnchorDB"];
        public static readonly string QPVideoAnchorDB = ConfigurationManager.AppSettings["QPVideoAnchorDB"];
        public static readonly string _DBType = ConfigurationManager.AppSettings["DBType"];
        public enum DbConnType
        {
            /// <summary>
            /// 管理后台
            /// </summary>
            QPVideoAnchorDB,
            /// <summary>
            /// 流水记录
            /// </summary>
            QPAnchorRecordDB,
            /// <summary>
            /// 电影记录
            /// </summary>
            MovieDB,
            /// <summary>
            /// 主播个性化
            /// </summary>
            AnchorPersonaliseDb,
            /// <summary>
            /// 经纪人主播管理
            /// </summary>
            QPAgentAnchorDB
        }

        /// <summary>
        /// QPAgentAnchorDB数据库操作
        /// </summary>
        /// <returns></returns>
        public static SqlSugarClient GetInstance()
        {
            ICacheService cacheService = null;
            if (_RedisCache)
            {
                cacheService = new RedisCache(_RedisHost, _RedisPort);
            }
            else
            {
                cacheService = new HttpRuntimeCache();
            }

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = QPAgentAnchorDB,
                DbType = DbType.SqlServer,
                InitKeyType = InitKeyType.Attribute,//InitKeyType 是读取主键和自增列信息的方式
                IsAutoCloseConnection = true,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    SqlFuncServices = ExtMethods.GetExpMethods,
                    DataInfoCacheService = cacheService
                }
            });
            return db;
        }
        /// <summary>
        /// QPVideoAnchorDB数据库操作
        /// </summary>
        /// <returns></returns>
        public static SqlSugarClient GetSqlSugarDB(DbConnType type)
        {
            var strConnectionString = "";
            switch (type)
            {
                case DbConnType.QPAgentAnchorDB:
                    strConnectionString = ConfigurationManager.AppSettings["QPAgentAnchorDB"];
                    break;
                case DbConnType.QPVideoAnchorDB:
                    strConnectionString= ConfigurationManager.AppSettings["QPVideoAnchorDB"];
                    break;
                case DbConnType.QPAnchorRecordDB:
                    strConnectionString= ConfigurationManager.AppSettings["QPAnchorRecordDB"];
                    break;
                case DbConnType.MovieDB:
                    strConnectionString= ConfigurationManager.AppSettings["MovieRecordDb"];
                    break;
                case DbConnType.AnchorPersonaliseDb:
                    strConnectionString= ConfigurationManager.AppSettings["AnchorPersonaliseDb"];
                    break;
                default:
                    strConnectionString= ConfigurationManager.AppSettings["QPVideoAnchorDB"];
                    break;
            }
            ICacheService cacheService = null;
            if (_RedisCache)
            {
                cacheService = new RedisCache(_RedisHost, _RedisPort);
            }
            else
            {
                cacheService = new HttpRuntimeCache();
            }

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = strConnectionString,
                DbType = DbType.SqlServer,
                InitKeyType = InitKeyType.Attribute,//InitKeyType 是读取主键和自增列信息的方式
                IsAutoCloseConnection = true,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    SqlFuncServices = ExtMethods.GetExpMethods,
                    DataInfoCacheService = cacheService
                }
            });
            return db;
        }
    }
}
