using Smartflow.Core.Components;
using Smartflow.Core.Elements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Smartflow.Core
{
    public class WorkflowCoreService : IWorkflowBasicCoreService, IWorkflowCoreService, IWorkflowMarkerCoreService
    {
        /// <summary>
        /// 抽象工作流的服务
        /// </summary>
        public AbstractWorkflow AbsWorkflowService => WorkflowGlobalServiceProvider.Resolve<AbstractWorkflow>();

        /// <summary>
        /// 否决
        /// </summary>
        public void Veto(WorkflowContext context)
        {
            new VetoService(this).Veto(context);
        }

        /// <summary>
        /// 原路退回
        /// </summary>
        public void Back(WorkflowContext context)
        {
            new BackService(this).Back(context);
        }

        /// <summary>
        /// 退回到发起者
        /// </summary>
        public void Sender(WorkflowContext context)
        {
            new BackSenderService(this).Sender(context);
        }

        /// <summary>
        /// 下一步流转
        /// </summary>
        public void Next(WorkflowContext context)
        {
            new NextService(this).Next(context);
        }

        /// <summary>
        /// 终止流程
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="context"></param>
        public void Kill(WorkflowContext context)
        {
            new KillService(this).Kill(context);
        }

        /// <summary>
        /// 书签功能，通过实现IWorkflowMarker 接口，控制流程实例动态的改变
        /// </summary>
        /// <param name="marker">参数</param>
        /// <param name="context">上下文</param>
        public void Execute(WorkflowMarkerArgs marker, WorkflowContext context)
        {
            IList<IWorkflowMarker> workflowMarkers = WorkflowGlobalServiceProvider.Query<IWorkflowMarker>();
            foreach (IWorkflowMarker workflowMarker in workflowMarkers)
            {
                if (Regex.IsMatch(marker.Node.Extra, workflowMarker.Pattern) && !String.IsNullOrEmpty(workflowMarker.Pattern))
                {
                    workflowMarker?.Execute(marker, () => AbsWorkflowService.InstanceService.Transfer(WorkflowInstanceState.Hang, context.InstanceID), () => Transfer(marker, context));
                }
            }
        }

        /// <summary>
        /// 内部状态的转移。对流程实例恢复到正常的状态后，执行相应的流转操作
        /// </summary>
        /// <param name="marker"></param>
        /// <param name="context"></param>
        private void Transfer(WorkflowMarkerArgs marker, WorkflowContext context)
        {
            AbsWorkflowService.InstanceService.Transfer(WorkflowInstanceState.Running, context.InstanceID);
            if (String.Equals(typeof(BackSenderService).Name, marker.Command, StringComparison.OrdinalIgnoreCase))
            {
                Back(context);
            }
            else if (String.Equals(typeof(NextService).Name, marker.Command, StringComparison.OrdinalIgnoreCase))
            {
                Next(context);
            }
        }

        /// <summary>
        /// 鉴定流转人是否拥有节点的流转权限
        /// </summary>
        /// <param name="node">节点信息</param>
        /// <returns>true|fale</returns>
        public Boolean Authentication(Node node)
        {
            IWorkflowAuthenticationService workflowAuthenticationService = WorkflowGlobalServiceProvider.Resolve<IWorkflowAuthenticationService>();
            if (workflowAuthenticationService == null || node.NodeType == WorkflowNodeCategory.Decision || node.NodeType == WorkflowNodeCategory.Fork || node.NodeType == WorkflowNodeCategory.Merge || node.NodeType == WorkflowNodeCategory.Start) return true;
            return workflowAuthenticationService.Authentication(node);
        }

        /// <summary>
        ///  执行自定义动作
        /// </summary>
        /// <param name="context">执行上下文</param>
        public void DoPluginAction(ExecutingContext context)
        {
            AbsWorkflowService.Actions.ForEach(pluin => pluin.ActionExecute(context));
        }
    }
}
