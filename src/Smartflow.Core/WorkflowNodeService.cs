/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using Smartflow.Core.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smartflow.Core.Internals;
using System.Xml.Linq;
using Action = Smartflow.Core.Elements.Action;
using System.Data;
using Smartflow.Core.Components;
using Smartflow.Common;
using NHibernate;
using NHibernate.Engine;
using System.Security.Cryptography;
using NHibernate.Criterion;
using NHibernate.Util;

namespace Smartflow.Core
{
    public class WorkflowNodeService : IWorkflowNodeService, IWorkflowPersistent<Element, Action<object>>, IWorkflowQuery<IList<Node>, string>, IWorkflowParse
    {
        public IWorkflowQuery<IList<WorkflowConfiguration>> ConfigurationService
        {
            get { return new WorkflowConfigurationService(); }
        }

        public WorkflowTransitionService TransitionService
        {
            get
            {
                return new WorkflowTransitionService();
            }
        }

        protected WorkflowCommandService CommandService
        {
            get
            {
                return new WorkflowCommandService();
            }
        }

        protected IWorkflowProcessService ProcessService
        {
            get { return WorkflowGlobalServiceProvider.Resolve<IWorkflowProcessService>(); }
        }

        public IWorkflowCooperationService WorkflowCooperationService
        {
            get { return WorkflowGlobalServiceProvider.Resolve<IWorkflowCooperationService>(); }
        }

        public Element Parse(XElement element)
        {
            Node node = new Node
            {
                Name = element.Attribute("name").Value,
                ID = element.Attribute("id").Value,
                Cooperation = element.Attribute("cooperation").Value,
                Assistant = element.Attribute("assistant").Value
            };

            XAttribute back = element.Attribute("back");
            XAttribute url = element.Attribute("url");
            XAttribute attr = element.Attribute("veto");

            node.Veto = attr != null && attr.Value == "0" ? String.Empty : "Smartflow.Core.Components.VetoService";
            node.Back = back == null ? String.Empty : back.Value;
            node.Url = url == null ? String.Empty : url.Value;

            string category = element.Attribute("category").Value;
            node.NodeType = Internals.Utils.Convert(category);
            if (element.HasElements)
            {
                List<Element> nodes = new List<Element>();
                element.Elements().ToList().ForEach(entry =>
                {
                    string nodeName = entry.Name.LocalName;
                    if (ServiceContainer.Contains(nodeName))
                    {
                        IWorkflowParse parseService = (ServiceContainer.Resolve(nodeName) as IWorkflowParse);
                        nodes.Add(parseService.Parse(entry));
                    }
                });

                node.Transitions.AddRange(nodes.Where(transition => (transition is Transition)).Cast<Transition>());
                node.Actions.AddRange(nodes.Where(action => (action is Action)).Cast<Action>());
                node.Groups.AddRange(nodes.Where(group => (group is Group)).Cast<Group>());
                node.Actors.AddRange(nodes.Where(actor => (actor is Actor)).Cast<Actor>());
                node.Rules.AddRange(nodes.Where(rule => (rule is Elements.Rule)).Cast<Elements.Rule>());
                node.Carbons.AddRange(nodes.Where(carbon => (carbon is Elements.Carbon)).Cast<Elements.Carbon>());
                node.Organizations.ToList().AddRange(nodes.Where(org => (org is Elements.Organization)).Cast<Elements.Organization>());
                node.Command = nodes.Where(command => (command is Command)).Cast<Command>().FirstOrDefault();

                if (node.Command != null)
                {
                    node.Command.Node = node;
                }
            }
            return node;
        }

        public void Persistent(Element entry, Action<object> callback)
        {
            Node n = (entry as Node);
            n.Transitions.ForEach(e =>
            {
                e.Origin = entry.ID;
                e.InstanceID = entry.InstanceID;
            });
            n.Actions.ForEach(e => e.InstanceID = entry.InstanceID);
            n.Groups.ForEach(e => e.InstanceID = entry.InstanceID);
            n.Organizations.ForEach(e => e.InstanceID = entry.InstanceID);
            n.Actors.ForEach(e => e.InstanceID = entry.InstanceID);
            n.Carbons.ForEach(e => e.InstanceID = entry.InstanceID);
            n.Rules.ForEach(e => e.InstanceID = entry.InstanceID);
            if (n.Command != null)
            {
                n.Command.InstanceID = entry.InstanceID;
            }

            callback(n);
        }

        public IList<Node> Query(string instanceID)
        {
            using ISession session = DbFactory.OpenSession();
            return session.Query<Node>().Where(e => e.InstanceID == instanceID).ToList();
        }


        /// <summary>
        /// 动态获取路线，根据决策节点设置条件表达式，自动去判断流转的路线
        /// </summary>
        /// <returns>路线</returns>
        public Transition GetTransition(Node n)
        {
            Command command = n.Command;
            IList<WorkflowConfiguration> settings = ConfigurationService.Query();
            WorkflowConfiguration config = settings
                .Where(cfg => cfg.ID == long.Parse(command.ID))
                .FirstOrDefault();

            using ISession session = DbFactory.CreateSessionFactory(config.ConnectionString, config.ProviderName).OpenSession();
            try
            {
                DataTable resultSet = new DataTable(Guid.NewGuid().ToString());
                IDbCommand cmd = session.Connection.CreateCommand();
                IDbDataParameter dataParameter = cmd.CreateParameter();
                dataParameter.ParameterName = "InstanceID";
                dataParameter.Value = n.InstanceID;
                dataParameter.Size = 50;
                dataParameter.DbType = DbType.String;
                dataParameter.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(dataParameter);
                cmd.CommandText = command.Text;
                DataTable result = new DataTable();
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    resultSet.Load(reader);
                    reader.Close();
                }
                Transition instance = null;

                List<Transition> transitions = n.Transitions.ToList();

                if (resultSet.Rows.Count > 0)
                {
                    foreach (Transition transition in transitions)
                    {
                        if (!String.IsNullOrEmpty(transition.Expression) && resultSet.Select(transition.Expression).Length > 0)
                        {
                            instance = transition;
                            break;
                        }
                    }
                }
                resultSet.Dispose();
                return instance;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Execute(Node n)
        {
            if (n.Command != null)
            {
                Command command = n.Command;
                IList<WorkflowConfiguration> settings = ConfigurationService.Query();
                WorkflowConfiguration config = settings.Where(cfg => cfg.ID == long.Parse(command.ID)).FirstOrDefault();
                try
                {
                    using ISession session = DbFactory.CreateSessionFactory(config.ConnectionString, config.ProviderName).OpenSession();

                    session.CreateSQLQuery(command.Text)
                            .SetParameter("InstanceID", n.InstanceID)
                            .ExecuteUpdate();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public List<Transition> GetExecuteTransition(Node entry)
        {
            List<Transition> transitions = new List<Transition>();
            transitions.AddRange(entry.Transitions);
            if (entry.NodeType != WorkflowNodeCategory.Start && !String.IsNullOrWhiteSpace(entry.Veto))
            {
                transitions.Add(Internals.Utils.CONST_REJECT_TRANSITION);
            }
            if (entry.NodeType != WorkflowNodeCategory.Start && !String.IsNullOrWhiteSpace(entry.Back))
            {
                transitions.Add(Internals.Utils.CONST_BACK_TRANSITION);
            }
            return transitions;
        }

        public Node FindNodeByID(string ID, string instanceID)
        {
            using ISession session = DbFactory.OpenSession();
            return session.Query<Node>().Where(e => e.InstanceID == instanceID && e.ID == ID)
                .ToList().FirstOrDefault();
        }

        public Transition GetNextTransition(string id, string instanceID)
        {
            return TransitionService
                        .Query(instanceID)
                        .FirstOrDefault(e => e.NID == id);
        }


        /// <summary>
        /// 上一个执行跳转节点
        /// </summary>
        /// <returns></returns>
        public Node GetPrevious(Node entry)
        {
            Transition transition = GetHistoryTransition(entry);
            if (transition == null) return null;

            using ISession session = DbFactory.OpenSession();
            Node node = session.Query<Node>()
                .Where(e => e.InstanceID == entry.InstanceID && e.ID == transition.Origin)
                .ToList().FirstOrDefault();
            return node;
        }

        /// <summary>
        /// 获取回退线路
        /// </summary>
        /// <returns>路线</returns>
        protected Transition GetHistoryTransition(Node entry)
        {
            Transition transition = null;

            Dictionary<String, Object> queryArg = new Dictionary<string, object>
                {
                    { "InstanceID", entry.InstanceID },
                    { "Direction",(int)WorkflowOpertaion.Go}
                };

            WorkflowProcess process = ProcessService
                                        .Query(queryArg).Where(c => c.Destination == entry.ID)
                                        .FirstOrDefault();

            if (process != null && entry.NodeType != WorkflowNodeCategory.Start)
            {
                transition = TransitionService.Query(entry.InstanceID).FirstOrDefault(e => e.NID == process.TransitionID);
            }

            return transition;
        }

        public IEnumerable<Node> GetNode(string instanceID)
        {
            using ISession session = DbFactory.OpenSession();
            string hql = "select n from Node n where n.InstanceID=:InstanceID and n.ID in (SELECT l.RelationshipID FROM WorkflowLink l WHERE l.InstanceID=:InstanceID)";
            return session.CreateQuery(hql)
                .SetParameter("InstanceID", instanceID)
                .List<Node>();
        }
    }
}
