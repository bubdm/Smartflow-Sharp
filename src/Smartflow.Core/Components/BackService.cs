﻿/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using System;
using Smartflow.Core.Elements;
using System.Linq;
using System.Text;

namespace Smartflow.Core.Components
{
    public class BackService: JumpService
    {
        public BackService(IWorkflowBasicCoreService coreService) : base(coreService)
        {

        }
 
        public void Back(WorkflowContext context)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(context.InstanceID);
            Node current = instance.Current.FirstOrDefault(e => e.NID == context.NodeID); 
            if (!base.Authentication(current)) return;
            Transition transition = AbsWorkflowService.NodeService.GetPreviousTransition(current);
            var to = AbsWorkflowService.NodeService.GetPrevious(transition);
            if (instance.State == WorkflowInstanceState.Running)
            {
                this.Invoke(transition.NID, new ExecutingContext
                {
                    From = current,
                    To = to,
                    Direction = WorkflowOpertaion.Back,
                    InstanceID = context.InstanceID,
                    Data = context.Data,
                    Message = context.Message
                }, context);
            }

            base.Invoke(new WorkflowMarkerArgs(to, WorkflowOpertaion.Back, typeof(BackService).Name),context);
        }

        private void Invoke(string transitionID, ExecutingContext executeContext, WorkflowContext context)
        {
            string instanceID = context.InstanceID;
            AbsWorkflowService.InstanceService.Jump(executeContext.From.ID, executeContext.To.ID, instanceID, new WorkflowProcess()
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

            }, AbsWorkflowService.ProcessService);

            base.DoPluginAction(executeContext);
            if (executeContext.To.NodeType == WorkflowNodeCategory.Decision)
            {
                this.Back(new WorkflowContext()
                {
                    InstanceID = context.InstanceID,
                    Message = "系统流转",
                    ActorID = context.ActorID,
                    NodeID = executeContext.To.NID
                });
            }
        }
    }
}

