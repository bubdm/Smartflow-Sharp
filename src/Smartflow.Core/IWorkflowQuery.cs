/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using System;
using System.Collections.Generic;

namespace Smartflow.Core
{
    public interface IWorkflowQuery<out T> where T : class
    {
        T Query();
    }

    public interface IWorkflowQuery<out T, in S> where T : class
    {
        T Query(S queryArg);
    }
}
