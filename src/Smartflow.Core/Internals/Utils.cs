/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using Smartflow;
using Smartflow.Core.Elements;

namespace Smartflow.Core.Internals
{
    internal class Utils
    {
        public static readonly string CONST_REJECT_TRANSITION_ID = "NID_REJECT_ID_80_11";
        public static readonly string CONST_BACK_TRANSITION_ID = "NID_BACK_ID_80_11";
        public static readonly string CONST_BACKSENDER_TRANSITION_ID = "NID_BACK_ID_80_12";
        public static readonly Transition CONST_REJECT_TRANSITION = new Transition
        {
            NID = Utils.CONST_REJECT_TRANSITION_ID,
            Name = "否决"
        };

        public static readonly Transition CONST_BACK_TRANSITION = new Transition
        {
            NID = Utils.CONST_BACK_TRANSITION_ID,
            Name = "原路退回"
        };

        public static readonly Transition CONST_BACKSENDER_TRANSITION = new Transition
        {
            NID = Utils.CONST_BACKSENDER_TRANSITION_ID,
            Name = "退回到发起人"
        };

        public static WorkflowNodeCategory Convert(string category)
        {
            return (WorkflowNodeCategory)Enum.Parse(typeof(WorkflowNodeCategory), category, true);
        }

        public static Object CreateInstance(Type createType)
        {
            return System.Activator.CreateInstance(createType);
        }

        public static Object CreateInstance(string typeName)
        {
            return Assembly.GetExecutingAssembly().CreateInstance(typeName);
        }
    }
}
