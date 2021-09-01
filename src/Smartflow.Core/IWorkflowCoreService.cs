using System;
using System.Collections.Generic;
using System.Text;

namespace Smartflow.Core
{
    public interface IWorkflowCoreService
    {
        void Back(WorkflowContext context);

        void Sender(WorkflowContext context);

        void Veto(WorkflowContext context);

        void Next(WorkflowContext context);

        void Kill(WorkflowContext context);
    }
}
