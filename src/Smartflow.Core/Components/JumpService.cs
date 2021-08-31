using System;
using System.Collections.Generic;
using System.Text;

namespace Smartflow.Core.Components
{
    public class JumpService
    {
        
        protected AbstractWorkflow WorkflowService
        {
            get
            {
                return WorkflowGlobalServiceProvider.Resolve<AbstractWorkflow>();
            }
        }

        /// <summary>
        /// 工作流标记(类似书签)
        /// </summary>
        protected IWorkflowMarker Marker
        {
            get;
            set;
        }

        protected JumpService(IWorkflowMarker marker)
        {
            this.Marker = marker;
        }

        protected virtual void Invoke(WorkflowMarkerArg arg, Action hang, Action resume)
        {
            Marker?.Execute(arg, hang, resume);
        }
    }
}
