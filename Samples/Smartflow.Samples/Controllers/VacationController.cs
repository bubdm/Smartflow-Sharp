using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Smartflow.BussinessService.Interfaces;
using Smartflow.BussinessService.Models;
using Smartflow.Samples.Models;

namespace Smartflow.Samples.Controllers
{
    [ApiController]
    public class VacationController : ControllerBase
    {
        private readonly IVacationService _vacationService;
        private readonly IWorkflowService _workflowService;
        private readonly IMapper _mapper;
        public VacationController(IMapper mapper, IWorkflowService workflowService, IVacationService vacationService)
        {
            _mapper = mapper;
            _workflowService = workflowService;
            _vacationService = vacationService;
        }

        [Route("api/vacation/persistent"), HttpPost]
        public async Task<string> PostAsync(VacationDto vacation)
        {
            vacation.CreateTime = DateTime.Now;
            vacation.NID = Guid.NewGuid().ToString();
            var model = _mapper.Map<VacationDto, Vacation>(vacation);
            _vacationService.Persistent(model);
            return await _workflowService.StartAsync(new Brigde
            {
                CategoryCode = "001001",
                Key = vacation.NID,
                Creator = vacation.UID
            });
        }


        [Route("api/order/persistent"), HttpPost]
        public void PostTestAsync(PasswordDto dto)
        {
          
        }

        [Route("api/vacation/{id}/info"), HttpGet]
        public VacationDto Get(string id)
        {
            return _mapper.Map<Vacation, VacationDto>(_vacationService.GetVacationByID(id));
        }

        public static dynamic Success(Object data, int total)
        {
            return new
            {
                code = 0,
                msg = "操作成功",
                count = total,
                data
            };
        }
    }
}
