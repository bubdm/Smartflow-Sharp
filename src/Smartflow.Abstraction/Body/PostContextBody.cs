/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 Github : https://github.com/chengderen/Smartflow-Sharp
 ********************************************************************
 */
using System.ComponentModel.DataAnnotations;

namespace Smartflow.Abstraction.Body
{
    public class PostContextBody
    {
        [Required]
        [StringLength(50)]
        public string InstanceID
        {
            get;
            set;
        }

        [Required]
        [StringLength(50)]
        public string ActorID
        {
            get;
            set;
        }

        [StringLength(2048)]
        public string Message
        {
            get;
            set;
        }

 
        [Required]
        public dynamic Data
        {
            get;
            set;
        }
    }
}