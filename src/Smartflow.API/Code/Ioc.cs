using Microsoft.Extensions.DependencyInjection;
using Smartflow.Bussiness.Interfaces;
using Smartflow.Bussiness.Models;
using Smartflow.Bussiness.Queries;
using Smartflow.Bussiness.WorkflowService;
using Smartflow.Common;
using Smartflow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smartflow.API.Code
{
    public class Ioc
    {
        public static void RegisterService(IServiceCollection services)
        {
            services.AddTransient<ISummaryService, SummaryService>();
            services.AddTransient<IBridgeService, BridgeService>();
            services.AddTransient<IQuery<IList<Category>>, CategoryService>();
            services.AddTransient<IPendingService, PendingService>();
            services.AddTransient<IRecordService, RecordService>();
            services.AddTransient<IQuery<IList<Constraint>>, ConstraintService>();
            services.AddTransient<AbstractBridgeService, WorkflowBridgeService>();
            services.AddTransient<IOrganizationService, OrganizationService>();
            services.AddTransient<IActorService, ActorService>();
            services.AddTransient<IWorkflowStructureService, WorkflowStructureService>();
        }
    }
}
