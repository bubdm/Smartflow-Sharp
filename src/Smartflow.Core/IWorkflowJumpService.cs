using System;
using System.Collections.Generic;
using System.Text;

namespace Smartflow.Core
{
    public interface IWorkflowJumpCoreService
    {
        void Execute(WorkflowMarkerArg marker, WorkflowContext context);
    }
}
