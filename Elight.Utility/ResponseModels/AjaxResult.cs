using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Elight.Utility.ResponseModels
{
    /// <summary>
    /// 通用AJAX请求响应数据格式模型。
    /// </summary>
    public class AjaxResult
    {
        public AjaxResult(ResultType state, string message, object data = null)
        {
            this.state = state;
            this.message = message;
            this.data = data;
        }
        /// <summary>
        /// 结果类型。
        /// </summary>
        public object state { get; set; }
        /// <summary>
        /// 消息内容。
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 返回数据。
        /// </summary>
        public object data { get; set; }
    }
    /// <summary>
    /// 通用AJAX请求响应数据格式模型 新版。
    /// </summary>
    public class TableResult
    {
        public TableResult(int code, string message, object data , int count)
        {
            this.code = code;
            this.msg = message;
            this.data = data;
            this.count = count;
        }
        /// <summary>
        /// 结果类型。 0成功，1失败
        /// </summary>
        public object code { get; set; }
        /// <summary>
        /// 消息内容。
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 返回数据。
        /// </summary>
        public object data { get; set; }
        /// <summary>
        /// 数据总长度
        /// </summary>
        public int count { get; set; }
    }
    /// <summary>
    /// 结果类型枚举。
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// 警告。
        /// </summary>
        Warning = 0,

        /// <summary>
        /// 成功。
        /// </summary>
        Success = 1,

        /// <summary>
        /// 异常。
        /// </summary>
        Error = 2,

        /// <summary>
        /// 消息。
        /// </summary>
        Info = 6
    }
}