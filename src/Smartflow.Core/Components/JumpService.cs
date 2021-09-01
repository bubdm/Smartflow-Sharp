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
        protected IWorkflowJumpCoreService CoreService
        {
            get;
            set;
        }

        protected JumpService(IWorkflowJumpCoreService coreService)
        {
            this.CoreService = coreService;
        }

        protected virtual void Invoke(WorkflowMarkerArg arg,WorkflowContext context)
        {
            CoreService?.Execute(arg, context);
        }
    }
}
