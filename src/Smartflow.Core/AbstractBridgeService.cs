/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using Smartflow.Core.Elements;
using Smartflow.Core.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smartflow.Core
{
    /// <summary>
    /// 定义与业务系统中基础数据衔接接口
    /// </summary>
    public abstract class AbstractBridgeService
    {
        protected IWorkflowProcessService ProcessService
        {
            get
            {
                return WorkflowGlobalServiceProvider.Resolve<WorkflowService>().ProcessService;
            }
        }

        protected IWorkflowQuery<IEnumerable<WorkflowConfiguration>> ConfigurationService
        {
            get
            {
                return WorkflowGlobalServiceProvider.Resolve<WorkflowService>().NodeService.ConfigurationService;
            }
        }

        /// <summary>
        /// 获取参与组
        /// </summary>
        /// <returns>组列表</returns>
        public abstract List<WorkflowGroup> GetGroup();

        /// <summary>
        /// 获取参与者列表
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public abstract List<WorkflowActor> GetActor(int pageIndex, int pageSize, out int total, Dictionary<string, string> queryArg);


        /// <summary>
        /// 获取参与者列表
        /// </summary>
        /// <param name="queryArg">查询条件</param>
        /// <returns></returns>
        public abstract List<WorkflowActor> GetActor(Dictionary<string, string> queryArg);


        /// <summary>
        /// 获取抄送列表
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public abstract List<WorkflowCarbon> GetCarbon(int pageIndex, int pageSize, out int total, Dictionary<string, string> queryArg);


        /// <summary>
        /// 获取抄送列表
        /// </summary>
        /// <param name="queryArg">查询条件</param>
        /// <returns></returns>
        public abstract List<WorkflowCarbon> GetCarbon(Dictionary<string, string> queryArg);



        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public List<WorkflowConfiguration> GetDatabaseSourceList()
        {
            return ConfigurationService.Query().ToList();
        }


        /// <summary>
        /// 获取当前执行节点的记录
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public dynamic GetJumpProcess(string instanceID)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(instanceID);
            dynamic records = ProcessService.Query(instanceID);

            List<string> runNodes = new List<string>();
            foreach (Node n in instance.Current)
            {
                runNodes.Add(n.ID);
            }
            return new
            {
                structure = instance.Resource,
                link = runNodes,
                record = records
            };
        }
    }
}
