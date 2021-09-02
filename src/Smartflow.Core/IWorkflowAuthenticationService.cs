using Smartflow.Core.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smartflow.Core
{
    public interface IWorkflowAuthenticationService
    {
        /// <summary>
        /// 检查是否授权
        /// </summary>
        /// <param name="node"></param>
        Boolean Authentication(Node node);
    }
}
