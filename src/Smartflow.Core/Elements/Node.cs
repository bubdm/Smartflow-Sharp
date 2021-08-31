/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Smartflow;
using System.Xml.Linq;

namespace Smartflow.Core.Elements
{
    public class Node : ASTNode
    {
        protected ISet<Actor> actors = new HashSet<Actor>();
        protected ISet<Group> groups = new HashSet<Group>();
        protected ISet<Rule> rules = new HashSet<Rule>();
        protected ISet<Organization> organizations = new HashSet<Organization>();
        protected ISet<Carbon> carbons = new HashSet<Carbon>();

        protected Command command;

        public virtual Command Command
        {
            get { return command; }
            set { command = value; }
        }

        public virtual ISet<Group> Groups
        {
            get { return groups; }
            set { groups = value; }
        }

        public virtual ISet<Actor> Actors
        {
            get { return actors; }
            set { actors = value; }
        }

        public virtual ISet<Organization> Organizations
        {
            get { return organizations; }
            set { organizations = value; }
        }

        public virtual ISet<Carbon> Carbons
        {
            get { return carbons; }
            set { carbons = value; }
        }

        public virtual ISet<Rule> Rules
        {
            get { return rules; }
            set { rules = value; }
        }

        public virtual string Veto
        {
            get;
            set;
        }

        public virtual string Back
        {
            get;
            set;
        }

        public virtual string Url
        {
            get;
            set;
        }

        public virtual string Extra
        {
            get;
            set;
        }
    }
}
