using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Services
{
    [DefineConfigurator(typeof(AdminServiceConfigurator))]
    class AdminService : SunfishService
    {
        private const string CFG_ADMIN_PWD = "adminPwd";
        public AdminService(SunfishServiceConfiguration ssc) : base(ssc)
        {

        }

        protected override void Start()
        {
        }

        protected override void Stop()
        {
        }

        public override string Description => "Allow remote configuration of Sunfish server";
    }
}
