using Smartflow.Core.Components;
using Smartflow.Core.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smartflow.Core
{
    public class WorkflowCoreService : IWorkflowCoreService, IWorkflowMarker
    {
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
        public void Next(WorkflowJumpContext context)
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

        public virtual void Execute(WorkflowMarkerArg marker, System.Action hang, System.Action resume)
        {
            IWorkflowMarker workflowMarker = WorkflowGlobalServiceProvider.Resolve<IWorkflowMarker>();
            workflowMarker?.Execute(marker, hang, resume);
        }
     }
}
