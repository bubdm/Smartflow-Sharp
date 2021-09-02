using System;
using System.Collections.Generic;
using System.Text;

namespace Smartflow.Core
{
    public interface IWorkflowMarker
    {
        /// <summary>
        /// 与node.extra 数据进行匹配
        /// </summary>
        string Pattern { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="marker"></param>
        /// <param name="hang"></param>
        /// <param name="resume"></param>
        void Execute(WorkflowMarkerArgs marker, Action hang, Action resume);
    }
}
