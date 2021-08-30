/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using System;
using Smartflow.Core.Elements;

namespace Smartflow.Core.Internals
{
    internal class XMLServiceFactory
    {
        public static Workflow Create(string resouceXml)
        {
            return XMLResolver.Parse(resouceXml);
        }
    }
}
