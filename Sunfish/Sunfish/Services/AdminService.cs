using DolphinWebXplorer2.Middleware;

namespace DolphinWebXplorer2.Services
{
    [DefineConfigurator(typeof(AdminServiceConfigurator))]
    class AdminService : SunfishService
    {
        public AdminService(SunfishServiceConfiguration ssc) : base(ssc)
        {

        }

        public override void Process(string path, HttpCall call)
        {
            throw new System.NotImplementedException();
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
