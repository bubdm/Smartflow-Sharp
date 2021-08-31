using System;
using System.Collections.Generic;
using System.Text;

namespace Smartflow.Core
{
    public interface IWorkflowMarker
    {
        void Execute(WorkflowMarkerArg marker,Action hang,Action resume);
    }
}
