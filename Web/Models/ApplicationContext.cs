using Models;
using SheetsEF.Models;

namespace Web.Models
{
    public class ApplicationContext : ApplicationContextBase
    {
        public ServiceContext Service
        {
            get => GetData<ServiceContext>(nameof(Service));
        }
 
        public ApplicationContext() : base()
        { }
    }
}
