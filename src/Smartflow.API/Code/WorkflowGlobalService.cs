using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smartflow.Bussiness.WorkflowService;
using Smartflow.Core;

namespace Smartflow.API.Code
{
    public class WorkflowGlobalService
    {
        public static void RegisterService()
        {
            WorkflowGlobalServiceProvider.RegisterGlobalService(typeof(PendingAction));
            WorkflowGlobalServiceProvider.RegisterGlobalService(typeof(RecordAction));
            WorkflowGlobalServiceProvider.RegisterGlobalService(typeof(CarbonCopyAction));
            WorkflowGlobalServiceProvider.RegisterPartService(new EmptyAction());
        }
    }
}
