/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using Smartflow.Core.Components;
using Smartflow.Core.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smartflow.Core
{
    public abstract class AbstractWorkflow
    {
        public List<IWorkflowAction> Actions
        {
            get { return WorkflowGlobalServiceProvider.Query<IWorkflowAction>(); }
        }

        public IWorkflowNodeService NodeService
        {
            get { return WorkflowGlobalServiceProvider.Resolve<IWorkflowNodeService>(); }
        }

        public IWorkflowProcessService ProcessService
        {
            get { return WorkflowGlobalServiceProvider.Resolve<IWorkflowProcessService>(); }
        }

        public IWorkflowInstanceService InstanceService
        {
            get
            {
                return WorkflowGlobalServiceProvider.Resolve<IWorkflowInstanceService>();
            }
        }

        public abstract string Start(string resourceXml);
    }
}
