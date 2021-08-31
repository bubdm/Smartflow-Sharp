/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Smartflow.Core.Elements;

namespace Smartflow.Core.Components
{
    public class KillService:JumpService
    {
        private readonly AbstractWorkflow workflowService = WorkflowGlobalServiceProvider.Resolve<AbstractWorkflow>();

        public KillService(IWorkflowMarker marker):base(marker)
        {
          
        }


        public void Kill(WorkflowContext context)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(context.InstanceID);
            Node current = instance.Current.FirstOrDefault(e => e.NID == context.NodeID);
            if (instance.State == WorkflowInstanceState.Running)
            {
                workflowService.InstanceService.Transfer(WorkflowInstanceState.Kill, instance.InstanceID);
                workflowService.Actions.ForEach(pluin => pluin.ActionExecute(new ExecutingContext()
                {
                    From = current,
                    To = current,
                    InstanceID = instance.InstanceID,
                    Message = "终止流程",
                    Data = context.Data
                }));
            }
            base.Invoke(new WorkflowMarkerArg(current, WorkflowOpertaion.Decide, typeof(KillService).Name), () => workflowService.InstanceService.Transfer(WorkflowInstanceState.Hang, instance.InstanceID), () => workflowService.InstanceService.Transfer(WorkflowInstanceState.Running, instance.InstanceID));
        }
    }
}
