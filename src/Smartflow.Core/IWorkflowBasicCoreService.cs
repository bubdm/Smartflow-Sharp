/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using Smartflow.Core.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smartflow.Core
{
    /// <summary>
    /// 定义核心流程引擎的基础服务
    /// </summary>
    public interface IWorkflowBasicCoreService : IWorkflowMarkerCoreService
    {
        /// <summary>
        ///  鉴定流转人是否拥有节点的流转权限
        /// </summary>
        /// <param name="node">当前节点</param>
        /// <returns>true|false</returns>
        Boolean Authentication(Node node);
    }
}
