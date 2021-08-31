/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smartflow.Core.Elements;

namespace Smartflow.Core.Components
{
    public class VetoService:JumpService
    {
        public VetoService(IWorkflowMarker marker):base(marker)
        {
           
        }

        public void Veto(WorkflowContext context)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(context.InstanceID);
            Node current = instance.Current.FirstOrDefault(e => e.NID == context.NodeID);
            if (instance.State == WorkflowInstanceState.Running)
            {
                WorkflowService.InstanceService.Transfer(WorkflowInstanceState.Reject, instance.InstanceID);
                WorkflowService.Actions.ForEach(pluin => pluin.ActionExecute(new ExecutingContext()
                {
                    From = current,
                    To = current,
                    InstanceID = context.InstanceID,
                    Message = context.Message,
                    Direction = WorkflowOpertaion.Decide,
                    Data = context.Data
                }));
            }
            
            base.Invoke(new WorkflowMarkerArg(current, WorkflowOpertaion.Decide, typeof(VetoService).Name), () => WorkflowService.InstanceService.Transfer(WorkflowInstanceState.Hang,instance.InstanceID), () => WorkflowService.InstanceService.Transfer(WorkflowInstanceState.Running, instance.InstanceID));
        }
    }
}
