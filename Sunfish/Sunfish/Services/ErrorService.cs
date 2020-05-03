using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Services
{
    [DefineConfigurator(typeof(ErrorServiceConfigurator))]
    class ErrorService : SunfishService
    {
        public ErrorService(SunfishServiceConfiguration ssc) : base(ssc)
        {
        }

        protected override void Start()
        {
        }

        protected override void Stop()
        {
        }

        public override string Description => null;
    }
}
