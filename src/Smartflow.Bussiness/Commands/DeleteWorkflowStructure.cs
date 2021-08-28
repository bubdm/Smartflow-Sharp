using System;
using System.Collections.Generic;
using System.Text;
using Smartflow.Common;
using NHibernate;
using Smartflow.Bussiness.Models;

namespace Smartflow.Bussiness.Commands
{
    public class DeleteWorkflowStructure : ICommand
    {
        public void Execute(Object o)
        {
            using ISession session = DbFactory.OpenSession();
            var model = session.Get<WorkflowStructure>(o.ToString());
            session.Delete(model);
            session.Flush();
        }
    }
}
