using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smartflow.Bussiness.Models;
using Smartflow.Common;
using NHibernate;
using Smartflow.Bussiness.Interfaces;

namespace Smartflow.Bussiness.Queries
{
    public class RecordService : IRecordService
    {
        public IList<Record> GetRecordByInstanceID(string instanceID)
        {
            using ISession session = DbFactory.OpenSession();
            return
                session.Query<Record>()
                .Where(r => r.InstanceID == instanceID)
                .OrderBy(e => e.CreateTime)
                .ThenBy(e => e.NodeID)
                .ToList();
        }
    }
}
