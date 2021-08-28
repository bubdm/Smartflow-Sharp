using Smartflow.Bussiness.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smartflow.Bussiness.Interfaces
{
    public interface IWorkflowStructureService
    {
        public WorkflowStructure GetStructureByID(string id);

        public IList<WorkflowStructure> Query();

        public IList<WorkflowStructure> Query(int pageIndex, int pageSize, out int total, Dictionary<string, string> queryArg);
    }
}
