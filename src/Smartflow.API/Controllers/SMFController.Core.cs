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
using Smartflow.Bussiness.Queries;
using Smartflow.Common;
using Smartflow.Common.Logging;
using Smartflow.Core;
using Smartflow.Core.Elements;

namespace Smartflow.API.Controllers
{
    [ApiController]
    public class SMFController : ControllerBase
    {
        private readonly IPendingService _pendingService;
        private readonly IBridgeService _bridgeService;
        private readonly IActorService _actorService;
        private readonly IWorkflowStructureService _workflowStructureService;
        private readonly IMapper _mapper;
        protected IWorkflowNodeService NodeService
        {
            get
            {
                return WorkflowGlobalServiceProvider.Resolve<AbstractWorkflow>().NodeService;
            }
        }
        public SMFController(IWorkflowStructureService workflowStructureService, IPendingService pendingService, IBridgeService bridgeService, IActorService actorService, IMapper mapper)
        {
            _pendingService = pendingService;
            _bridgeService = bridgeService;
            _actorService = actorService;
            _workflowStructureService = workflowStructureService;
            _mapper = mapper;
        }

        /// <summary>
        /// 启动
        /// </summary>
        [Route("api/smf/start"), HttpPost]
        public string Start(BridgeBody dto)
        {
            Category category = new CategoryService().Query().FirstOrDefault(cate => cate.NID == dto.CategoryCode);
            WorkflowStructure workflowStructure =
                _workflowStructureService.Query().FirstOrDefault(e => e.CategoryCode == category.NID && e.Status == 1);
            string instanceID = WorkflowEngine.Instance.Start(workflowStructure.Resource);

            Bridge model = _mapper.Map<Bridge>(dto);
            model.InstanceID = instanceID;
            model.Comment = String.IsNullOrEmpty(model.Comment) ? category.Name : model.Comment;
            model.CreateTime = DateTime.Now;
            CommandBus.Dispatch(new CreateBridge(), model);
            var user = _actorService.GetUserByID(model.Creator);
            WorkflowInstance Instance = WorkflowInstance.GetInstance(instanceID);
            var current = GetCurrent(Instance, model.Creator);
            string serialObject = GetAuditNext(current, model.CategoryCode, model.Creator, user.Name);

            WorkflowEngine.Instance.Next(new WorkflowJumpContext()
            {
                InstanceID = instanceID,
                ActorID = model.Creator,
                Message = category.Name,
                NodeID = current.NID,
                Data = Newtonsoft.Json.JsonConvert.DeserializeObject(serialObject)
            });

            LogProxy.Instance.Info(string.Format("启动{0}流程 实例ID{1}", model.CategoryCode, instanceID));
            return instanceID;
        }

        /// <summary>
        /// 向下一步流转
        /// </summary>
        /// <param name="context"></param>
        [Route("api/smf/next"), HttpPost]
        public void Next(PostContextBody context)
        {
            WorkflowInstance Instance = WorkflowInstance.GetInstance(context.InstanceID);
            var current = GetCurrent(Instance, context.ActorID);
            WorkflowEngine.Instance.Next(new WorkflowJumpContext()
            {
                NodeID = current.NID,
                InstanceID = context.InstanceID,
                ActorID = context.ActorID,
                Message = context.Message,
                Data = context.Data
            });
        }

        /// <summary>
        /// 原路退回
        /// </summary>
        /// <param name="context"></param>
        [Route("api/smf/back"), HttpPost]
        public void Back(PostContextBody context)
        {
            WorkflowInstance Instance = WorkflowInstance.GetInstance(context.InstanceID);
            var current = GetCurrent(Instance, context.ActorID);
            WorkflowEngine.Instance.Back(new WorkflowContext()
            {
                NodeID = current.NID,
                InstanceID = context.InstanceID,
                ActorID = context.ActorID,
                Message = context.Message,
                Data = context.Data
            });
        }

        /// <summary>
        /// 退回到发起者
        /// </summary>
        /// <param name="context"></param>
        [Route("api/smf/sender"), HttpPost]
        public void Sender(PostContextBody context)
        {
            WorkflowInstance Instance = WorkflowInstance.GetInstance(context.InstanceID);
            var current = GetCurrent(Instance, context.ActorID);
            WorkflowEngine.Instance.BackSender(new WorkflowContext()
            {
                NodeID = current.NID,
                InstanceID = context.InstanceID,
                ActorID = context.ActorID,
                Message = context.Message,
                Data = context.Data
            });
        }

        /// <summary>
        /// 否决
        /// </summary>
        /// <param name="context"></param>
        [Route("api/smf/veto"), HttpPost]
        public void Veto(PostContextBody context)
        {
            WorkflowInstance Instance = WorkflowInstance.GetInstance(context.InstanceID);
            var current = GetCurrent(Instance, context.ActorID);
            WorkflowEngine.Instance.Veto(new WorkflowContext()
            {
                NodeID = current.NID,
                InstanceID = context.InstanceID,
                ActorID = context.ActorID,
                Message = context.Message,
                Data = context.Data
            });
        }

        /// <summary>
        /// 重新发起流程
        /// </summary>
        /// <param name="dto"></param>
        [Route("api/smf/{instanceID}/{categoryCode}/reboot/{id}"), HttpPost]
        public void Reboot(string instanceID, string categoryCode, string id)
        {
            WorkflowInstance wfInstance = WorkflowInstance.GetInstance(instanceID);
            string resourceXml = wfInstance.Resource;
            CommandBus.Dispatch(new DeleteWFRecord(), instanceID);
            string newInstanceID = WorkflowEngine.Instance.Start(resourceXml);
            Bridge bridge = _bridgeService.GetBridge(id);
            bridge.InstanceID = newInstanceID;
            CommandBus.Dispatch(new UpdateBridge(), bridge);

            var user = _actorService.GetUserByID(bridge.Creator);
            WorkflowInstance instance = WorkflowInstance.GetInstance(newInstanceID);
            var current = GetCurrent(instance, bridge.Creator);
            string serialObject = GetAuditNext(current, categoryCode, bridge.Creator, user.Name);
            WorkflowEngine.Instance.Next(new WorkflowJumpContext()
            {
                InstanceID = newInstanceID,
                ActorID = bridge.Creator,
                Message = String.Empty,
                NodeID = current.NID,
                Data = Newtonsoft.Json.JsonConvert.DeserializeObject(serialObject)
            });
        }

        /// <summary>
        /// 获取当前节点信息
        /// </summary>
        /// <param name="id">实例ID</param>
        /// <returns></returns>

        [Route("api/smf/{instanceID}/node/{actorID}"), HttpGet]
        public dynamic GetNode(string instanceID, string actorID)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(instanceID);
            IList<Pending> pendings = _pendingService.GetPending(instance.InstanceID, actorID);

            if (pendings.Count > 0)
            {
                var current = GetCurrent(instance, actorID);
                Dictionary<string, object> queryArg = new Dictionary<string, object>
                {
                    { "actorID",actorID },
                    { "instanceID",instanceID },
                    { "nodeID",current.NID}
                };

                var pending = _pendingService.Query(queryArg).FirstOrDefault();
                var hasAuth = (current.NodeType == WorkflowNodeCategory.Start && instance.State == WorkflowInstanceState.Running) || instance.State == WorkflowInstanceState.Running && pending != null;
                return new
                {
                    current.NID,
                    current.Name,
                    Category = current.NodeType.ToString(),
                    HasAuth = hasAuth
                };
            }
            else
            {
                return new
                {
                    NID = Guid.NewGuid(),
                    Name = instance.State == WorkflowInstanceState.End ? "结束" : "审批中",
                    Category = instance.State == WorkflowInstanceState.End ?
                    WorkflowNodeCategory.End.ToString() : WorkflowNodeCategory.Fork.ToString(),
                    HasAuth = false
                };
            }
        }

        /// <summary>
        /// 获取当前节点
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="actorID"></param>
        /// <returns></returns>
        private Node GetCurrent(WorkflowInstance instance, string actorID)
        {
            IList<Pending> pendings = _pendingService.GetPending(instance.InstanceID, actorID);
            Pending model = pendings.FirstOrDefault();
            var current = instance.Current;
            return (current.Count() > 1) ?
                current.FirstOrDefault(e => e.NID == model.NodeID) :
                current.FirstOrDefault();
        }

        private string GetAuditNext(Node current, string categoryCode, string creator, string name)
        {
            string instanceID = current.InstanceID;
            Transition transitionSelect = current.Transitions.FirstOrDefault();
            Node node = NodeService.FindNodeByID(transitionSelect.Destination, instanceID);
            List<string> groupIDs = new List<string>();
            List<string> actorIDs = new List<string>();
            List<string> carbonIDs = new List<string>();
            List<string> organizationIDs = new List<string>();

            foreach (Group g in node.Groups)
            {
                groupIDs.Add(g.ID.ToString());
            }
            foreach (Carbon c in node.Carbons)
            {
                carbonIDs.Add(c.ID.ToString());
            }
            foreach (Actor item in node.Actors)
            {
                actorIDs.Add(item.ID);
            }
            foreach (Smartflow.Core.Elements.Organization item in node.Organizations)
            {
                organizationIDs.Add(item.ID);
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                CategoryCode = categoryCode,
                UUID = creator,
                Name = name,
                Group = string.Join(",", groupIDs),
                Actor = string.Join(",", actorIDs),
                Carbon = string.Join(",", carbonIDs),
                Organization = string.Join(",", organizationIDs)
            });
        }

        /// <summary>
        /// 跳转
        /// </summary>
        [Route("api/smf/{categoryCode}/{instanceID}/kill/{destination}"), HttpPost]
        public void Kill(string categoryCode, string instanceID, string destination)
        {
            Node current = NodeService.FindNodeByID(destination, instanceID);
            WorkflowEngine.Instance.Kill(new WorkflowContext()
            {
                Message = "终止流程",
                InstanceID = instanceID,
                NodeID = current.NID,
                Data = new { CategoryCode = categoryCode }
            });
        }

        [Route("api/smf/{instanceID}/transition/{actorID}/list"), HttpGet]
        public IEnumerable<CommandTransitionDto> GetTransition(string instanceID, string actorID)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(instanceID);
            var current = GetCurrent(instance, actorID);
            IEnumerable<Transition> transitions = NodeService.GetExecuteTransition(current);
            IList<CommandTransitionDto> commands = new List<CommandTransitionDto>();
            foreach (Transition transition in transitions)
            {
                commands.Add(new CommandTransitionDto { Code = transition.NID, Name = transition.Name });
            }
            return commands;
        }

        /// <summary>
        ///  获取参与者信息
        /// </summary>
        [Route("api/smf/{instanceID}/actor/{destination}/next"), HttpGet]
        public dynamic GetActorByNext(string instanceID, string destination)
        {
            Transition transition = NodeService.GetNextTransition(destination, instanceID);
            Node node = NodeService.FindNodeByID(transition.Destination, instanceID);
            return new
            {
                NodeType = node.NodeType.ToString(),
                Actor = node.Actors,
                Group = node.Groups,
                Organization = node.Organizations,
                Carbon = node.Carbons
            };
        }

        /// <summary>
        ///  获取节点类型
        /// </summary>
        [Route("api/smf/{instanceID}/next/{destination}"), HttpGet]
        public string GetNodeTypeByNext(string instanceID, string destination)
        {
            Transition transition = NodeService.GetNextTransition(destination, instanceID);
            Node current = NodeService.FindNodeByID(transition.Destination, instanceID);
            return current.NodeType.ToString();
        }

        /// <summary>
        ///  获取参与者信息
        /// </summary>
        [Route("api/smf/{instanceID}/actor/current/{destination}"), HttpGet]
        public dynamic GetCurrentNodeActor(string instanceID, string destination)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(instanceID);
            Node current = instance.Current.Where(e => e.NID == destination).FirstOrDefault();
            return new
            {
                NodeType = current.NodeType.ToString(),
                Actor = current.Actors,
                Group = current.Groups,
                Organization = current.Organizations,
                Carbon = current.Carbons
            };
        }
    }
}