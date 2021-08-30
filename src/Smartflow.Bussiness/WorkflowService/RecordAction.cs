using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smartflow.Bussiness.Commands;
using Smartflow.Bussiness.Models;
using Smartflow.Common;
using Smartflow.Core;


namespace Smartflow.Bussiness.WorkflowService
{
    public class RecordAction : IWorkflowAction
    {
        private readonly WorkflowBridgeService bridgeService = new WorkflowBridgeService();
        public void ActionExecute(ExecutingContext executeContext)
        {
            var current = executeContext.From;
            WorkflowInstance instance = WorkflowInstance.GetInstance(executeContext.InstanceID);
            if (instance.State != WorkflowInstanceState.Kill && current.NodeType != WorkflowNodeCategory.Decision&& current.NodeType!=WorkflowNodeCategory.Fork && current.NodeType != WorkflowNodeCategory.Merge)
            {
                string UUID = (String)executeContext.Data.UUID;
                string auditUserName = (String)executeContext.Data.Name;

                CommandBus.Dispatch(new CreateRecord(), new Record
                {
                    InstanceID = executeContext.InstanceID,
                    Name = executeContext.From.Name,
                    NodeID = executeContext.From.NID,
                    Comment = executeContext.Message,
                    CreateTime = DateTime.Now,
                    AuditUserID = UUID,
                    Url = string.Empty,
                    AuditUserName = auditUserName,
                    NID = Guid.NewGuid().ToString(),
                });
            }
        }
    }
}