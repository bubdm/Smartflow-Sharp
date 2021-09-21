using Smartflow.Core.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smartflow.Core.Components
{
    public class JumpService
    {
       
        protected AbstractWorkflow AbsWorkflowService
        {
            get;
        }

        /// <summary>
        /// 工作流核心基础服务
        /// </summary>
        protected IWorkflowBasicCoreService CoreService
        {
            get;
            set;
        }

        protected JumpService(IWorkflowBasicCoreService coreService)
        {
            this.CoreService = coreService;
            this.AbsWorkflowService = coreService.AbsWorkflowService;
        }

        /// <summary>
        /// 书签功能，通过实现IWorkflowMarker 接口，控制流程实例动态的改变
        /// </summary>
        /// <param name="arg">标记参数</param>
        /// <param name="context">上下文</param>
        protected virtual void Invoke(WorkflowMarkerArgs arg,WorkflowContext context)
        {
            CoreService?.Execute(arg, context);
        }

        /// <summary>
        /// 鉴定流转人是否拥有节点的流转权限
        /// </summary>
        /// <param name="node">节点信息</param>
        /// <returns>true|fale</returns>
        protected virtual Boolean Authentication(Node node)
        {
           return CoreService.Authentication(node);
        }

        /// <summary>
        ///  执行自定义动作
        /// </summary>
        /// <param name="context">执行上下文</param>
        protected virtual void DoPluginAction(ExecutingContext context)
        {
            CoreService?.DoPluginAction(context);
        }
    }
}
