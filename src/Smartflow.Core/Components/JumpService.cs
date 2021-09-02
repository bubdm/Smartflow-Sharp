using Smartflow.Core.Elements;
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
        protected IWorkflowBasicCoreService CoreService
        {
            get;
            set;
        }

        protected JumpService(IWorkflowBasicCoreService coreService)
        {
            this.CoreService = coreService;
        }

        protected virtual void Invoke(WorkflowMarkerArgs arg,WorkflowContext context)
        {
            CoreService?.Execute(arg, context);
        }

        protected virtual Boolean Authentication(Node node)
        {
           return CoreService.Authentication(node);
        }
    }
}
