/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Smartflow.Abstraction.Body;
using Smartflow.Abstraction.DTO;
using Smartflow.Bussiness.Commands;
using Smartflow.Bussiness.Interfaces;
using Smartflow.Bussiness.Models;
using Smartflow.Common;
using Smartflow.Core;
using Smartflow.Core.Elements;
using Smartflow.API.Code;

namespace Smartflow.API.Controllers
{
    [ApiController]
    public class SettingController : ControllerBase
    {
        protected IWorkflowNodeService NodeService
        {
            get
            {
                return WorkflowGlobalServiceProvider.Resolve<AbstractWorkflow>().NodeService;
            }
        }
        private readonly AbstractBridgeService _abstractService;
        private readonly IQuery<IList<Constraint>> _constraintService;
        private readonly IOrganizationService _organizationService;
        private readonly IActorService _actorService;
        private readonly IQuery<IList<Category>> _categoryService;
        private readonly IBridgeService _bridgeService;
        private readonly IRecordService _recordService;
        private readonly ISummaryService _summaryService;
        private readonly IWorkflowStructureService _workflowStructureService;

        private readonly IMapper _mapper;
        public SettingController(AbstractBridgeService abstractService, IWorkflowStructureService workflowStructureService, ISummaryService summaryService, IRecordService recordService, IQuery<IList<Constraint>> constraintService, IOrganizationService organizationService, IActorService actorService, IQuery<IList<Category>> categoryService, IBridgeService bridgeService, IMapper mapper)
        {
            _abstractService = abstractService;
            _constraintService = constraintService;
            _organizationService = organizationService;
            _actorService = actorService;
            _categoryService = categoryService;
            _bridgeService = bridgeService;
            _recordService = recordService;
            _summaryService = summaryService;
            _workflowStructureService = workflowStructureService;
            _mapper = mapper;
        }

        [Route("api/setting/group/list"), HttpGet]
        public IEnumerable<WorkflowGroup> GetGroup()
        {
            return _abstractService.GetGroup();
        }

        [Route("api/setting/action/list"), HttpGet]
        public IEnumerable<dynamic> GetAction()
        {
            IList<dynamic> types = new List<dynamic>();
            foreach (IWorkflowAction action in WorkflowGlobalServiceProvider.QueryActions())
            {
                Type type = action.GetType();
                types.Add(new
                {
                    id = type.FullName,
                    name = type.Name
                });
            }
            return types;
        }

        [Route("api/setting/database-source/list"), HttpGet]
        public IEnumerable<WorkflowConfiguration> GetDatabaseSourceList()
        {
            return _abstractService.GetDatabaseSourceList();
        }

        [Route("api/setting/actor/page"), HttpPost]
        public dynamic GetActor(Paging info)
        {
            return CommonMethods.Response(_abstractService
                .GetActor(info.Page, info.Limit, out int total, info.Get()), total);
        }

        [Route("api/setting/assign-actor/page"), HttpPost]
        public dynamic GetAssignActor(Paging info)
        {
            IList<WorkflowActor> list = _abstractService.GetActor(info.Get());
            return CommonMethods.Response(list, list.Count);
        }

        [Route("api/setting/carbon/page"), HttpPost]
        public dynamic Getcarbon([FromBody] Paging info)
        {
            return CommonMethods
                .Response(_abstractService
                .GetCarbon(info.Page, info.Limit, out int total, info.Get()), total);
        }

        [Route("api/setting/assign-carbon/page"), HttpPost]
        public dynamic GetAssignCarbon([FromBody] Paging info)
        {
            IList<WorkflowCarbon> list = _abstractService.GetCarbon(info.Get());
            return CommonMethods.Response(list, list.Count);
        }

        [Route("api/setting/constraint/list"), HttpGet]
        public IEnumerable<Constraint> GetConstraint()
        {
            return _constraintService.Query();
        }

        [Route("api/setting/organization/list"), HttpGet]
        public IEnumerable<Bussiness.Models.Organization> GetOrganization()
        {
            IList<Bussiness.Models.Organization> orgs = new List<Bussiness.Models.Organization>();
            _organizationService.Load("0", orgs);
            return orgs;
        }

        [Route("api/setting/actor/{instanceID}/user/{nodeID}/list"), HttpGet]
        public ResultData GetAuditUser(string instanceID, string nodeID)
        {
            Node current = NodeService.FindNodeByID(nodeID, instanceID);
            Dictionary<String, string> queryArg = new Dictionary<string, string>
            {
                { "instanceID",instanceID },
                { "nodeID", current.NID}
            };
            IList<User> users = _actorService.Query(queryArg);
            return CommonMethods.Response(_mapper.Map<IList<User>, IList<UserDto>>(users), users.Count);
        }

        [Route("api/setting/category/list"), HttpGet]
        public IEnumerable<Category> GetCategories()
        {
            return _categoryService.Query();
        }

        [Route("api/setting/category/{id}/info"), HttpGet]
        public Category GetCategory(string id)
        {
            return _categoryService.Query().FirstOrDefault(cate => cate.NID == id);
        }

        [Route("api/setting/process/{instanceID}/list"), HttpGet]
        public dynamic GetJumpProcesses(string instanceID)
        {
            return _abstractService.GetJumpProcess(instanceID);
        }

        [Route("api/setting/bridge/{id}/info"), HttpGet]
        public BridgeDto GetBridge(string id)
        {
            return _mapper.Map<Bridge, BridgeDto>(_bridgeService.GetBridge(id));
        }

        [Route("api/setting/bridge/{categoryCode}/{id}/single"), HttpGet]
        public BridgeDto GetBridgeByMultipleKeys(string id, string categoryCode)
        {
            Dictionary<string, string> queryArg = new Dictionary<string, string>
            {
                { "Key", id },
                { "CategoryCode", categoryCode}
            };
            return _mapper.Map<Bridge, BridgeDto>(_bridgeService.Query(queryArg));
        }

        [Route("api/setting/record/{instanceID}/list"), HttpGet]
        public IEnumerable<RecordDto> GetRecords(string instanceID)
        {
            return _mapper.Map<IList<Record>, IList<RecordDto>>(_recordService.GetRecordByInstanceID(instanceID));
        }

        [Route("api/setting/pending/delete"), HttpPost]
        public void Delete(PendingDeleteBody dto)
        {
            string[] ids = dto.ActorIDs.Split(',');
            if (ids.Length > 0)
            {
                WorkflowInstance instance = WorkflowInstance.GetInstance(dto.ID);
                var node = instance.Current.FirstOrDefault(e => e.ID == dto.NodeID);
                foreach (string actor in ids)
                {
                    CommandBus.Dispatch(new DeletePendingByActor(), new Dictionary<string, object>
                    {
                        { "instanceID", dto.ID },
                        { "nodeID", node.NID},
                        { "actorID", actor }
                    });
                }
            }
        }

        [Route("api/setting/pending/persistent"), HttpPost]
        public void Persistent(PendingBody dto)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(dto.ID);
            var node = instance.Current.FirstOrDefault(e => e.ID == dto.NodeID);
            Category model = _categoryService.Query().FirstOrDefault(cate => cate.NID == dto.CategoryCode);
            string[] ids = dto.ActorIDs.Split(',');
            foreach (string id in ids)
            {
                CommandBus.Dispatch(new CreatePending(), new Pending
                {
                    NID = Guid.NewGuid().ToString(),
                    ActorID = id,
                    InstanceID = instance.InstanceID,
                    NodeID = node.NID,
                    Url = model.Url,
                    CreateTime = DateTime.Now,
                    NodeName = node.Name,
                    CategoryCode = dto.CategoryCode,
                    CategoryName = model.Name
                });
            }
        }

        [Route("api/setting/summary/query/page"), HttpPost]
        public ResultData Paging(Paging paging)
        {
            List<Summary> list = _summaryService.Query(paging, out int total).ToList();
            return CommonMethods.Response(_mapper.Map<List<Summary>, List<SummaryDto>>(list), total);
        }

        [Route("api/setting/summary/supervise/page"), HttpPost]
        public ResultData PagingSupervise(Paging info)
        {
            List<Supervise> list = _summaryService.QuerySupervise(info, out int total).ToList();
            return CommonMethods.Response(_mapper.Map<List<Supervise>, List<SummaryDto>>(list), total);
        }

        [Route("api/setting/summary/{instanceID}/{categoryCode}/delete/{id}"), HttpPost]
        public void Delete(string instanceID, string categoryCode, string id)
        {
            CommandBus.Dispatch(new DeleteAllRecord(), new Script
            {
                CategoryCode = categoryCode,
                InstanceID = instanceID,
                Key = id
            });
        }

        [Route("api/setting/structure/query/page"), HttpPost]
        public dynamic Query(Paging paging)
        {
            IList<WorkflowStructure> structs =
                _workflowStructureService.Query(paging.Page, paging.Limit, out int total, paging.Get());
            return CommonMethods.Response(_mapper.Map<IList<WorkflowStructure>, IList<WorkflowStructureDto>>(structs), total);
        }

        [Route("api/setting/structure/{id}/info"), HttpGet]
        public WorkflowStructureDto GetWorkflowStructureByID(string id)
        {
            return _mapper.Map<WorkflowStructure, WorkflowStructureDto>(_workflowStructureService.GetStructureByID(id));
        }

        [Route("api/setting/structure/{id}/update/{status}"), HttpPost]
        public void Update(string id, int status)
        {
            WorkflowStructure model = _workflowStructureService.GetStructureByID(id);
            model.Status = status;
            if (model.Status == 1)
            {
                IList<WorkflowStructure> wfList = _workflowStructureService.Query().Where(e => e.CategoryCode == model.CategoryCode && e.NID != model.NID && e.Status == 1)
                    .ToList<WorkflowStructure>();

                foreach (WorkflowStructure entry in wfList)
                {
                    entry.Status = 0;
                    CommandBus.Dispatch(new CreateWorkflowStructure(), entry);
                }
            }
            CommandBus.Dispatch(new CreateWorkflowStructure(), model);
        }

        [Route("api/setting/structure/persistent"), HttpPost]
        public void Persistent(WorkflowStructureBody commandDto)
        {
            commandDto.Resource = Uri.UnescapeDataString(commandDto.Resource);
            WorkflowStructure structure = _mapper.Map<WorkflowStructureBody, WorkflowStructure>(commandDto);
            structure.CreateTime = DateTime.Now;
            if (!String.IsNullOrEmpty(commandDto.NID))
            {
                WorkflowStructure model = _workflowStructureService.GetStructureByID(structure.NID);
                if (model != null)
                {
                    if (structure.CategoryCode != model.CategoryCode)
                    {
                        structure.Status = 0;
                    }
                }
            }
            CommandBus.Dispatch(new CreateWorkflowStructure(), structure);
        }

        [Route("api/setting/structure/{id}/delete"), HttpPost]
        public void Delete(string id)
        {
            CommandBus.Dispatch(new DeleteWorkflowStructure(), id);
        }
    }
}