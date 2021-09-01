using Smartflow.Core.Components;
using Smartflow.Core.Elements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Smartflow.Core
{
    public class WorkflowCoreService : IWorkflowCoreService, IWorkflowJumpCoreService
    {
        protected AbstractWorkflow WorkflowService
        {
            get
            {
                return WorkflowGlobalServiceProvider.Resolve<AbstractWorkflow>();
            }
        }

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

        public void Execute(WorkflowMarkerArg marker, WorkflowContext context)
        {
            IList<IWorkflowMarker> workflowMarkers = WorkflowGlobalServiceProvider.Query<IWorkflowMarker>();
            foreach (IWorkflowMarker workflowMarker in workflowMarkers)
            {
                if (Regex.IsMatch(marker.Node.Extra, workflowMarker.Pattern) && !String.IsNullOrEmpty(workflowMarker.Pattern))
                {
                    workflowMarker?.Execute(marker, () => WorkflowService.InstanceService.Transfer(WorkflowInstanceState.Hang, context.InstanceID), () => Transfer(marker, context));
                }
            }
        }

        private void Transfer(WorkflowMarkerArg marker, WorkflowContext context)
        {
            WorkflowService.InstanceService.Transfer(WorkflowInstanceState.Running, context.InstanceID);
            if (String.Equals(typeof(BackSenderService).Name, marker.Command, StringComparison.OrdinalIgnoreCase))
            {
                Back(context);
            }
            else if (String.Equals(typeof(NextService).Name, marker.Command, StringComparison.OrdinalIgnoreCase))
            {
                Next(context);
            }
        }
    }
}
