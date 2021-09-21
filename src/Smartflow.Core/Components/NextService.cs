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
    public class NextService: JumpService
    {
        public NextService(IWorkflowBasicCoreService coreService) : base(coreService)
        {

        }

        public void Next(WorkflowContext context)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(context.InstanceID);
            Node current = instance.Current.FirstOrDefault(e => e.NID == context.NodeID);
            if (!base.Authentication(current)) return;
            Transition transition = current.Transitions.FirstOrDefault();
            IList<Node> nodes = AbsWorkflowService.NodeService.Query(instance.InstanceID);
            Node to = nodes.FirstOrDefault(e => e.ID == transition.Destination);
            this.Invoke(transition, new ExecutingContext
            {
                From = current,
                To = to,
                Direction = WorkflowOpertaion.Go,
                InstanceID = instance.InstanceID,
                Data = context.Data,
                Message = context.Message
            }, context);
        }

        private void Next(WorkflowContext context, Transition transition)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(context.InstanceID);
            IList<Node> nodes = AbsWorkflowService.NodeService.Query(instance.InstanceID);
            Node current = nodes.FirstOrDefault(e => e.NID == context.NodeID);
            Node to = nodes.FirstOrDefault(e => e.ID == transition.Destination);
            this.Invoke(transition, new ExecutingContext
            {
                From = current,
                To = to,
                Direction = WorkflowOpertaion.Go,
                InstanceID = instance.InstanceID,
                Data = context.Data,
                Message = context.Message
            }, context);
        }

        private void Invoke(Transition selectTransition, ExecutingContext executeContext, WorkflowContext context)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(context.InstanceID);
            if (instance.State == WorkflowInstanceState.Running)
            {
                string instanceID = context.InstanceID;
                Node to = executeContext.To;
                AbsWorkflowService.InstanceService.Jump(selectTransition.Origin, selectTransition.Destination, instanceID, new WorkflowProcess()
                {
                    RelationshipID = executeContext.From.NID,
                    CreateTime = DateTime.Now,
                    ActorID = context.ActorID,
                    Origin = executeContext.From.ID,
                    Destination = executeContext.To.ID,
                    TransitionID = selectTransition.NID,
                    InstanceID = executeContext.InstanceID,
                    NodeType = executeContext.From.NodeType,
                    Direction = WorkflowOpertaion.Go
                }, AbsWorkflowService.ProcessService);
                
                base.DoPluginAction(executeContext);
                if (to.NodeType == WorkflowNodeCategory.End)
                {
                    AbsWorkflowService.InstanceService.Transfer(WorkflowInstanceState.End, instanceID);
                }
                else if (to.NodeType == WorkflowNodeCategory.Decision)
                {
                    Transition transition = base.AbsWorkflowService.NodeService.GetTransition(to);
                    if (transition == null) return;
                    Next(new WorkflowJumpContext()
                    {
                        InstanceID = instanceID,
                        NodeID = to.NID,
                        Data = context.Data,
                        Message = "系统流转",
                        ActorID = context.ActorID

                    }, transition);
                }
                else if (to.NodeType == WorkflowNodeCategory.Fork)
                {
                    foreach (Transition transition in to.Transitions)
                    {
                        Next(new WorkflowJumpContext()
                        {
                            InstanceID = instanceID,
                            NodeID = to.NID,
                            Data = context.Data,
                            Message = "系统流转",
                            ActorID = context.ActorID
                        }, transition);
                    }
                }
                else if (to.NodeType == WorkflowNodeCategory.Merge)
                {
                    IList<Transition> transitions = WorkflowGlobalServiceProvider.Resolve<IWorkflowTransitionService>().Query(instanceID);
                    int linkCount = WorkflowGlobalServiceProvider.Resolve<IWorkflowLinkService>().GetLink(to.ID, instanceID);
                    if (transitions.Count(e => e.Destination == to.ID) == linkCount)
                    {
                        foreach (Transition transition in to.Transitions)
                        {
                            Next(new WorkflowJumpContext()
                            {
                                InstanceID = instanceID,
                                NodeID = to.NID,
                                Data = context.Data,
                                Message = "系统流转",
                                ActorID = context.ActorID
                            }, transition);
                        }
                    }
                }
            }

            base.Invoke(new WorkflowMarkerArgs(executeContext.To, WorkflowOpertaion.Go, typeof(NextService).Name),context);
        }
    }
}
