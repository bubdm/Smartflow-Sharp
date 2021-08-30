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

namespace Smartflow.Core.Elements
{
    public abstract class ASTNode : Element
    {
        protected ISet<Transition> transitions = new HashSet<Transition>();
        protected WorkflowNodeCategory category = WorkflowNodeCategory.Node;
        protected ISet<Action> actions = new HashSet<Action>();
    
        public virtual ISet<Action> Actions
        {
            get { return actions; }
            set { actions = value; }
        }

        public virtual ISet<Transition> Transitions
        {
            get { return transitions; }
            set { transitions = value; }
        }

        public virtual WorkflowNodeCategory NodeType
        {
            get { return category; }
            set { category = value; }
        }
    }
}