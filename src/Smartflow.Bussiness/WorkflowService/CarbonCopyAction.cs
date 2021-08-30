using System;
using System.Collections.Generic;
using Smartflow.Bussiness.Commands;
using Smartflow.Bussiness.Models;
using Smartflow.Common;
using Smartflow.Core;

namespace Smartflow.Bussiness.WorkflowService
{
    public class CarbonCopyAction : IWorkflowAction
    {
        private readonly WorkflowBridgeService bridgeService = new WorkflowBridgeService();
        public void ActionExecute(ExecutingContext executeContext)
        {
            var current = executeContext.To;
            WorkflowInstance instance = WorkflowInstance.GetInstance(executeContext.InstanceID);
            if (instance.State != WorkflowInstanceState.Kill && current.NodeType != WorkflowNodeCategory.Decision)
            {
                List<User> userList = bridgeService.GetCarbonCopies(current, (String)executeContext.Data.Carbon);
                foreach (User user in userList)
                {
                    WriteCarbon(user.ID, current.NID, instance.InstanceID);
                }
            }
        }


        /// <summary>
        /// 抄送
        /// </summary>
        /// <param name="actorID">参与者</param>
        /// <param name="executeContext"></param>
        public void WriteCarbon(string actorID, string nodeID, string instanceID)
        {
            CommandBus.Dispatch(new CreateCarbonCopy(), new CarbonCopy
            {
                ActorID = actorID,
                InstanceID = instanceID,
                NodeID = nodeID,
                CreateTime = DateTime.Now
            });
        }
    }
}