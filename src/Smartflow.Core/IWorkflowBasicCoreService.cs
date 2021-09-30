﻿/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using Smartflow.Core.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smartflow.Core
{
    /// <summary>
    /// 定义核心流程引擎的基础服务
    /// </summary>
    public interface IWorkflowBasicCoreService : IWorkflowMarkerCoreService
    {
        /// <summary>
        /// 抽象工作流的服务
        /// </summary>
        AbstractWorkflow AbsWorkflowService { get; }

        /// <summary>
        ///  鉴定流转人是否拥有节点的流转权限
        /// </summary>
        /// <param name="node">当前节点</param>
        /// <returns>true|false</returns>
        bool Authentication(Node node);

        /// <summary>
        ///  执行自定义动作
        /// </summary>
        /// <param name="context">执行上下文</param>
        void DoPluginAction(ExecutingContext context);
    }
}
