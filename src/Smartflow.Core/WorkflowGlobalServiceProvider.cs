/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Smartflow.Core.Components;

namespace Smartflow.Core
{
    public static class WorkflowGlobalServiceProvider
    {
        private static readonly IList<Type> _globalTypeCollection = new List<Type>();

        private static readonly IList<IWorkflowAction> _partCollection = new List<IWorkflowAction>();

        static WorkflowGlobalServiceProvider()
        {
            _globalTypeCollection.Add(typeof(MailService));
            _globalTypeCollection.Add(typeof(WorkflowService));
            _globalTypeCollection.Add(typeof(WorkflowNodeService));
            _globalTypeCollection.Add(typeof(WorkflowProcessService));
            _globalTypeCollection.Add(typeof(WorkflowInstanceService));
            _globalTypeCollection.Add(typeof(DefaultActionService));
            _globalTypeCollection.Add(typeof(ScriptActionService));
            _globalTypeCollection.Add(typeof(WorkflowCooperationService));
            _globalTypeCollection.Add(typeof(WorkflowTransitionService));
            _globalTypeCollection.Add(typeof(WorkflowLinkService));
            _globalTypeCollection.Add(typeof(WorkflowAssistantService));
        }

        public static void RegisterGlobalService(Type registerType)
        {
            _globalTypeCollection.Add(registerType);
        }

        public static void RegisterPartService(IWorkflowAction action)
        {
            _partCollection.Add(action);
        }

        public static T Resolve<T>()
        {
            Type map = _globalTypeCollection
                      .Where(e => typeof(T).IsAssignableFrom(e))
                      .FirstOrDefault();

            return (map == null) ? default : (T)Smartflow.Core.Internals.Utils.CreateInstance(map);
        }

        /// <summary>
        /// 移除全局服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Remove<T>() where T : class
        {
            _globalTypeCollection
               .Where(o => typeof(T).IsAssignableFrom(o))
               .ToList()
               .ForEach((entry) => _globalTypeCollection.Remove(entry));
        }

        /// <summary>
        /// 查询注册到全局里面的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> Query<T>() where T : class
        {
            List<T> services = new List<T>();
            _globalTypeCollection.Where(o => typeof(T).IsAssignableFrom(o)).ToList().ForEach(s =>
            {
                services.Add((T)Smartflow.Core.Internals.Utils.CreateInstance(s));
            });

            return services;
        }

        public static IList<IWorkflowAction> QueryActions()
        {
            return WorkflowGlobalServiceProvider._partCollection;
        }
    }
}
