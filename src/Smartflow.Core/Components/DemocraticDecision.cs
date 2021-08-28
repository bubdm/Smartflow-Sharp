/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smartflow.Core.Components
{
    public class DemocraticDecision: IWorkflowCooperationDecision
    {
        public string Execute(IList<WorkflowCooperation> records)
        {
            IList<string> selectDestinations = new List<string>();
            foreach (WorkflowCooperation entry in records)
            {
                selectDestinations.Add(entry.TransitionID);
            }

            var data = from d in selectDestinations
                       group d by d into g
                       orderby g.Count() descending
                       select g.Key;

            return data.FirstOrDefault();
        }
    }
}
