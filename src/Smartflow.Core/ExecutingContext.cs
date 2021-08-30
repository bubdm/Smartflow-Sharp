/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smartflow.Core.Elements;

namespace Smartflow.Core
{
    /// <summary>
    /// 工作流执行下文参数封装
    /// </summary>
    public class ExecutingContext
    {
        /// <summary>
        /// 当前节点
        /// </summary>
        public Node From
        {
            get;
            set;
        }

        /// <summary>
        /// 跳转到节点
        /// </summary>
        public Node To
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }


        /// <summary>
        /// 命令方向
        /// </summary>
        public WorkflowOpertaion Direction
        {
            get;
            set;
        }

        public string InstanceID
        {
            get;
            set;
        }


        ///// <summary>
        ///// 工作流实例
        ///// </summary>
        //public WorkflowInstance Instance
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// 传递数据
        /// </summary>
        public dynamic Data
        {
            get;
            set;
        }
    }
}
