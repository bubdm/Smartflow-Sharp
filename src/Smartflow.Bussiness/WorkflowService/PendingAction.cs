using System;
using System.Collections.Generic;
using System.Linq;
using Smartflow.Bussiness.Commands;
using Smartflow.Bussiness.Models;
using Smartflow.Bussiness.Queries;
using Smartflow.Common;
using Smartflow.Core;
using Smartflow.Core.Elements;

namespace Smartflow.Bussiness.WorkflowService
{
    public class PendingAction : IWorkflowAction
    {
        private readonly WorkflowBridgeService bridgeService = new WorkflowBridgeService();

        public void ActionExecute(ExecutingContext executeContext)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(executeContext.InstanceID);
            if (instance.State == WorkflowInstanceState.Reject || instance.State == WorkflowInstanceState.Kill)
            {
                CommandBus.Dispatch(new DeletePending(), instance.InstanceID);
            }
            else
            {
                var current = executeContext.To;
                if (current.NodeType == WorkflowNodeCategory.Decision)
                {
                    DecisionJump(executeContext);
                }
                else
                {
                    string instanceID = instance.InstanceID;
                    if (current.NodeType == WorkflowNodeCategory.End)
                    {
                        CommandBus.Dispatch(new DeletePending(), instanceID);
                    }
                    else
                    {
                        AssignToPendingUser(executeContext, current);
                    }
                }
            }
        }

        /// <summary>
        /// 分派到人
        /// </summary>
        private void AssignToPendingUser(ExecutingContext executeContext,Node current)
        {
            string instanceID = executeContext.InstanceID;
            //会签或分支节点取默认参与人者；非会签取用户选择的参与者
            bool result = (executeContext.From.NodeType == WorkflowNodeCategory.Decision || executeContext.From.NodeType == WorkflowNodeCategory.Fork || executeContext.From.NodeType == WorkflowNodeCategory.Merge);
            List<User> userList = result ?
                bridgeService.GetActorByGroup(current, executeContext.Direction) :
                bridgeService.GetActorByGroup((String)executeContext.Data.Actor, (String)executeContext.Data.Group, (String)executeContext.Data.Organization, current, executeContext.Direction);

            CommandBus.Dispatch(new DeletePendingByMultipleCondition(), new Dictionary<string, object>(){
                { "instanceID",instanceID},
                { "nodeID", executeContext.From.NID }
            });

            foreach (User user in userList)
            {
                WritePending(user.ID, executeContext);
            }
        }


        /// <summary>
        /// 多条件跳转
        /// </summary>
        /// <param name="executeContext">执行上下文</param>
        private void DecisionJump(ExecutingContext executeContext)
        {
            string instanceID = executeContext.InstanceID;
            string NID = executeContext.From.NID;
            Dictionary<string, object> deleteArg = new Dictionary<string, object>()
            {
                { "instanceID",instanceID},
                { "nodeID",NID }
            };
            CommandBus.Dispatch(new DeletePendingByMultipleCondition(), deleteArg);
        }

        /// <summary>
        /// 写待办信息
        /// </summary>
        /// <param name="actorID">参与者</param>
        /// <param name="executeContext">执行上下文</param>
        public void WritePending(string actorID, ExecutingContext executeContext)
        {
            var node = executeContext.To;
            string CategoryCode = (String)executeContext.Data.CategoryCode;
            string instanceID =executeContext.InstanceID;

            Category model = new CategoryService().Query()
                 .FirstOrDefault(cate => cate.NID == CategoryCode);
            
            CommandBus.Dispatch(new CreatePending(), new Pending
            {
                NID = Guid.NewGuid().ToString(),
                ActorID = actorID,
                InstanceID = instanceID,
                NodeID = node.NID,
                Url = model.Url,
                CreateTime = DateTime.Now,
                NodeName = node.Name,
                CategoryCode = CategoryCode,
                CategoryName = model.Name
            });
        }
    }
}
