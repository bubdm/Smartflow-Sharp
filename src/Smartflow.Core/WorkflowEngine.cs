/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smartflow.Core.Components;
using Smartflow.Core.Elements;
using Smartflow.Core.Internals;

namespace Smartflow.Core
{
    public sealed class WorkflowEngine
    {
        private static readonly WorkflowEngine singleton = new WorkflowEngine();

        private readonly AbstractWorkflow workflowService = WorkflowGlobalServiceProvider.Resolve<AbstractWorkflow>();

        private WorkflowEngine()
        {
        }

        private IWorkflowCoreService CoreService
        {
            get
            {
                return new WorkflowCoreService();
            }
        }

        public static WorkflowEngine Instance
        {
            get { return singleton; }
        }

        /// <summary>
        /// 根据传递的流程XML字符串,启动工作流
        /// </summary>
        /// <param name="resourceXml"></param>
        /// <returns></returns>
        public string Start(string resourceXml)
        {
            return workflowService.Start(resourceXml);
        }

        /// <summary>
        /// 否决
        /// </summary>
        public void Veto(WorkflowContext context)
        {
            CoreService.Veto(context);
        }

        /// <summary>
        /// 原路退回
        /// </summary>
        public void Back(WorkflowContext context)
        {
            CoreService.Back(context);
        }

        /// <summary>
        /// 退回到发起者
        /// </summary>
        public void BackSender(WorkflowContext context)
        {
            CoreService.Back(context);
        }

        /// <summary>
        /// 下一步流转
        /// </summary>
        public void Next(WorkflowJumpContext context)
        {
            CoreService.Next(context);
        }

        /// <summary>
        /// 终止流程
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="context"></param>
        public void Kill(WorkflowContext context)
        {
            CoreService.Kill(context);
        }
    }
}
