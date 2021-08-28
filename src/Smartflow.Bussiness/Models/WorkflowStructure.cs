/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using System;

namespace Smartflow.Bussiness.Models
{
    public class WorkflowStructure
    {
        public virtual string NID
        {
            get;
            set;
        }

        public virtual string Name
        {
            get;
            set;
        }

        public virtual string CategoryCode
        {
            get;
            set;
        }

        public virtual string CategoryName
        {
            get;
            set;
        }

        public virtual string Resource
        {
            get;
            set;
        }

        public virtual DateTime CreateTime
        {
            get;
            set;
        }

        public virtual int Status
        {
            get;
            set;
        }

        public virtual string Memo
        {
            get;
            set;
        }
    }
}
