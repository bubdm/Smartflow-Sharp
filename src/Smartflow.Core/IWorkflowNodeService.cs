/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using Smartflow.Core.Elements;
using System;
using System.Collections.Generic;

namespace Smartflow.Core
{
    public interface IWorkflowNodeService : IWorkflowPersistent<Element, Action<object>>, IWorkflowQuery<IList<Node>,string>, IWorkflowParse
    {
        IWorkflowQuery<IList<WorkflowConfiguration>>  ConfigurationService
        {
            get;
        }

         WorkflowTransitionService TransitionService
        {
            get;
            
        }

        Transition GetTransition(Node n);

        IEnumerable<Node> GetNode(string instanceID);

        void Execute(Node entry);

        Node FindNodeByID(string id, string instanceID);

        List<Transition> GetExecuteTransition(Node entry);

        Node GetPrevious(Transition transition);

        Transition GetNextTransition(string id, string instanceID);

        Transition GetPreviousTransition(Node entry);
    }
}
