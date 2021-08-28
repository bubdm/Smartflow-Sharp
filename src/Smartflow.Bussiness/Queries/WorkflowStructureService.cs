/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using Smartflow.Bussiness.Interfaces;
using Smartflow.Bussiness.Models;
using Smartflow.Common;

namespace Smartflow.Bussiness.Queries
{
    public class WorkflowStructureService : IWorkflowStructureService
    {
        public WorkflowStructure GetStructureByID(string id)
        {
            using ISession session = DbFactory.OpenSession();
            return session.Get<WorkflowStructure>(id);
        }

        public IList<WorkflowStructure> Query()
        {
            using ISession session = DbFactory.OpenSession();
            return session.Query<WorkflowStructure>().OrderByDescending(e => e.CreateTime).ToList();
        }

        public IList<WorkflowStructure> Query(int pageIndex, int pageSize, out int total, Dictionary<string, string> queryArg)
        {
            using ISession session = DbFactory.OpenSession();
            IQueryOver<WorkflowStructure> queries=session.QueryOver<WorkflowStructure>();
            SetQueryArg(queries.RootCriteria, queryArg);
            total = queries.RowCount();
            return queries
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .List();
        }
        
        private void SetQueryArg(ICriteria criteria, Dictionary<string, string> queryArg)
        {
            if (queryArg.ContainsKey("CategoryCode"))
            {
                criteria.Add(Expression.Eq("CategoryCode", queryArg["CategoryCode"]));
            }

            if (queryArg.ContainsKey("Key"))
            {
                criteria.Add(Expression.Like("Name", String.Format("%{0}%", queryArg["Key"])));
            }
        }
    }
}
