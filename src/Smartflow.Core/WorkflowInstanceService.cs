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
using Smartflow.Core.Elements;
using Smartflow.Core.Internals;
using System.Data;
using NHibernate;
using Smartflow.Common;

namespace Smartflow.Core
{
    public class WorkflowInstanceService :IWorkflowInstanceService, IWorkflowQuery<WorkflowInstance,string>
    {
        public void Jump(string origin, string destination, String instanceID, WorkflowProcess process, IWorkflowPersistent<WorkflowProcess, Action<Object>> processService)
        {
            IList<Action<ISession>> commands = new List<Action<ISession>>
            {
                (session) => 
                    session
                    .CreateQuery("delete from WorkflowLink c where c.RelationshipID=:RelationshipID and c.InstanceID=:InstanceID")
                    .SetParameter("RelationshipID",origin)
                    .SetParameter("InstanceID",instanceID)
                    .ExecuteUpdate(),
                (session) => session.Persist(new WorkflowLink {  InstanceID = instanceID,RelationshipID = destination,CreateTime=DateTime.Now }),
                (session) => processService.Persistent(process, (entry) => session.Persist(entry))
            };

            DbFactory.Execute(DbFactory.OpenSession(),commands);
        }

        public string CreateInstance(string nodeID, string resource, Action<object> callback)
        {
            string instanceID = Guid.NewGuid().ToString();
          
            callback(new WorkflowInstance
            {
                InstanceID = instanceID,
                State = WorkflowInstanceState.Running,
                Resource = resource
            });

            callback(new WorkflowLink
            {
                InstanceID = instanceID,
                RelationshipID = nodeID,
                CreateTime=DateTime.Now
            });

            return instanceID;
        }

        public void Transfer(WorkflowInstanceState state, string instanceID)
        {
            using ISession session = DbFactory.OpenSession();
            WorkflowInstance instance = session.Get<WorkflowInstance>(instanceID);
            instance.State = state;
            session.Update(instance);
            session.Flush();
        }

        public WorkflowInstance Query(string instanceID)
        {
            try
            {
                using ISession session = DbFactory.OpenSession();
                WorkflowInstance instance = session.Get<WorkflowInstance>(instanceID);
                instance.Current = WorkflowGlobalServiceProvider.Resolve<IWorkflowNodeService>().GetNode(instanceID);
                return instance;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
