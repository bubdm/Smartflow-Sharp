using System;
using System.Collections.Generic;
using Smartflow.Bussiness.Interfaces;
using Smartflow.Bussiness.Models;
using Smartflow.Common;
using NHibernate;
using NHibernate.Criterion;

namespace Smartflow.Bussiness.Queries
{
    public class SummaryService : ISummaryService
    {
        public IList<Summary> Query(Dictionary<string, string> queryArg)
        {
            using ISession session = DbFactory.OpenSession();
            IQueryOver<Summary> queries = session.QueryOver<Summary>();
            SetQueryArg(queries.RootCriteria, queryArg);
            return queries.List();
        }

        public IList<Summary> Query(Paging info, out int total)
        {
            using ISession session = DbFactory.OpenSession();
            IQueryOver<Summary> queries = session.QueryOver<Summary>();
            SetQueryArg(queries.RootCriteria, info.Get());
            total = queries.RowCount();
            return queries
                    .Skip((info.Page - 1) * info.Limit)
                    .Take(info.Limit)
                    .List();
        }

        public IList<Supervise> QuerySupervise(Paging info, out int total)
        {
            using ISession session = DbFactory.OpenSession();
            IQueryOver<Supervise> queries = session.QueryOver<Supervise>();
            SetQueryArg(queries.RootCriteria, info.Get());
            total = queries.RowCount();

            var totallist = queries.List();

            IList<Supervise> list=queries
                    .Skip((info.Page - 1) * info.Limit)
                    .Take(info.Limit)
                    .List();

            return list;
        }

        private void SetQueryArg(ICriteria criteria, Dictionary<string, string> queryArg)
        {
            if (queryArg.ContainsKey("categoryCode"))
            {
                criteria.Add(Expression.Like("CategoryCode", queryArg["categoryCode"]));
            }
            if (queryArg.ContainsKey("type") && "3" == queryArg["type"])
            {
                criteria.Add(Expression.Eq("Creator", queryArg["actor"]));
            }
            //待办事项
            if (queryArg.ContainsKey("type") && "0" == queryArg["type"])
            {
                criteria.Add(Expression.Sql("  InstanceID IN (SELECT InstanceID FROM T_PENDING WHERE  ACTORID =? )", new object[] { queryArg["actor"] }, new NHibernate.Type.IType[] { NHibernate.NHibernateUtil.String }));
            }

            //抄送
            else if (queryArg.ContainsKey("type") && "1" == queryArg["type"])
            {
                criteria.Add(Expression.Sql("   InstanceID IN (SELECT InstanceID FROM t_carbonCopy WHERE  ACTORID =? )", new object[] { queryArg["actor"] }, new NHibernate.Type.IType[] { NHibernate.NHibernateUtil.String }));
            }

            //审批过
            else if (queryArg.ContainsKey("type") && "2" == queryArg["type"])
            {
                criteria.Add(Expression.Sql("   InstanceID IN (SELECT InstanceID FROM t_record WHERE Name<>'开始' AND AuditUserID =? )", new object[] { queryArg["actor"] }, new NHibernate.Type.IType[] { NHibernate.NHibernateUtil.String }));
            }

            //实例表 查询监督信息
            else if (queryArg.ContainsKey("type") && "4" == queryArg["type"])
            {
                criteria.Add(Expression.Sql("   InstanceID IN (SELECT InstanceID FROM T_INSTANCE WHERE  State='Running' )"));
            }

            //申请的
            else if (queryArg.ContainsKey("type") && "5" == queryArg["type"])
            {
                criteria.Add(Expression.Sql(" Creator =? AND InstanceID IN (SELECT InstanceID FROM T_INSTANCE WHERE  State='Running') ", new object[] { queryArg["actor"] }, new NHibernate.Type.IType[] { NHibernate.NHibernateUtil.String }));
            }

            criteria.AddOrder(new Order("CreateTime", false));
        }
    }
}
