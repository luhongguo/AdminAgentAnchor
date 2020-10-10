using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Elight.Utility.Network;
using System.Reflection;
using System.IO;
using System.Configuration;
namespace Elight.Utility.Log
{
    /// <summary>
    /// NLog日志框架辅助类。
    /// </summary>
    public static class LogHelper
    {
        public static object logLock = new object();
        /// <summary>
        /// 后台日志处理
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteLog(string msg)
        {
            lock (logLock)
            {
                try
                {
                    //日期文件夹
                    string today = DateTime.Today.ToString("yyyyMMdd");
                    //string filePath = "d:/QD/";
                    string filePath = ConfigurationManager.AppSettings["EloghtLogRoute"];
                    //日志目录
                    filePath = filePath + today + "/";
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    StreamWriter stream = new StreamWriter(filePath + "log.txt", true, Encoding.UTF8);
                    stream.Write(DateTime.Now.ToString() + ":" + msg);
                    stream.Write("\r\n");
                    stream.Flush();
                    stream.Close();
                }
                catch
                {

                }
            }
        }
        /// <summary>
        /// 采集程序日志处理
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteLogTips(string msg)
        {
            lock (logLock)
            {
                try
                {
                    //日期文件夹
                    string today = DateTime.Today.ToString("yyyyMMdd");
                    //string filePath = "d:/QD/";
                    string filePath = ConfigurationManager.AppSettings["TipCollectLogRoute"];
                    //日志目录
                    filePath = filePath + today + "/";
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    StreamWriter stream = new StreamWriter(filePath + "log.txt", true, Encoding.UTF8);
                    stream.Write(DateTime.Now.ToString() + ":" + msg);
                    stream.Write("\r\n");
                    stream.Flush();
                    stream.Close();
                }
                catch
                {

                }
            }
        }
        public static string GetEnumDescription(Enum enumValue)
        {
            string value = enumValue.ToString();
            FieldInfo field = enumValue.GetType().GetField(value);
            object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);    //获取描述属性
            if (objs == null || objs.Length == 0)    //当描述属性没有时，直接返回名称
                return value;
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
            return descriptionAttribute.Description;
        }
    }

    public enum Level
    {
        [Description("普通输出")]
        Trace,
        [Description("一般调试")]
        Debug,
        [Description("普通消息")]
        Info,
        [Description("警告信息")]
        Warn,
        [Description("一般错误")]
        Error,
        [Description("致命错误")]
        Fatal
    }

}
