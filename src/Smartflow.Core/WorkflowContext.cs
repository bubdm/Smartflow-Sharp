/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using Smartflow.Core.Components;
using Smartflow.Core.Elements;
using System;

namespace Smartflow.Core
{
    public class WorkflowContext
    {
        public string InstanceID
        {
            get;
            set;
        }

        public string NodeID
        {
            get;
            set;
        }

        public string ActorID
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 传递数据
        /// </summary>
        public dynamic Data
        {
            get;
            set;
        }
    }
}
