/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using Smartflow.Core.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smartflow.Core.Components
{
    public class BackSenderService: JumpService
    {
        public BackSenderService(IWorkflowMarker marker):base(marker)
        {

        }

        public void Sender(WorkflowContext context)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(context.InstanceID);
            Node current = instance.Current.FirstOrDefault(e => e.NID == context.NodeID);
            while (current.NodeType != WorkflowNodeCategory.Start&&instance.State== WorkflowInstanceState.Running)
            {
                Transition transition = base.WorkflowService.NodeService.GetPreviousTransition(current);
                var to = base.WorkflowService.NodeService.GetPrevious(transition);
                this.Invoke(transition.NID, new ExecutingContext
                {
                    From = current,
                    To = to,
                    Direction = WorkflowOpertaion.Back,
                    InstanceID = context.InstanceID,
                    Data = context.Data,
                    Message = context.Message
                }, context);
                current = to;
                base.Invoke(new WorkflowMarkerArg(to, WorkflowOpertaion.Back, typeof(BackSenderService).Name), () =>WorkflowService.InstanceService.Transfer(WorkflowInstanceState.Hang, instance.InstanceID), () =>WorkflowService.InstanceService.Transfer(WorkflowInstanceState.Running, instance.InstanceID));
            }
        }

        private void Invoke(string transitionID, ExecutingContext executeContext, WorkflowContext context)
        {
            string instanceID = context.InstanceID;
            WorkflowService.InstanceService.Jump(executeContext.From.ID, executeContext.To.ID, instanceID, new WorkflowProcess()
            {
                RelationshipID = executeContext.From.NID,
                CreateTime = DateTime.Now,
                ActorID = context.ActorID,
                Origin = executeContext.From.ID,
                Destination = executeContext.To.ID,
                TransitionID = transitionID,
                InstanceID = executeContext.InstanceID,
                NodeType = executeContext.From.NodeType,
                Direction = WorkflowOpertaion.Back
            }, WorkflowService.ProcessService);

            WorkflowService.Actions.ForEach(pluin => pluin.ActionExecute(executeContext));

            if (executeContext.To.NodeType == WorkflowNodeCategory.Decision)
            {
                this.Sender(new WorkflowContext()
                {
                    InstanceID = context.InstanceID,
                    NodeID = executeContext.To.NID,
                    Message = "系统流转",
                    Data = context.Data,
                    ActorID = context.ActorID,
                });
            }
        }
    }
}
