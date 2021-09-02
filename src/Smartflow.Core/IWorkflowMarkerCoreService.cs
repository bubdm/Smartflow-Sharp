using System;
using System.Collections.Generic;
using System.Text;

namespace Smartflow.Core
{
    public interface IWorkflowMarkerCoreService
    {
        /// <summary>
        /// 通过书签实现对流程实例的控制
        /// </summary>
        /// <param name="marker">标记</param>
        /// <param name="context">上下文</param>
        void Execute(WorkflowMarkerArgs marker, WorkflowContext context);
    }
}
