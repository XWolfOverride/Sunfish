using DolphinWebXplorer2.Middleware;
using System.IO;

namespace DolphinWebXplorer2.Services
{
    [DefineConfigurator(typeof(WebServiceConfigurator))]
    class WebService : SunfishService
    {
        private string index;
        private bool allowNavigation;
        private bool allowSubfolderNavigation;
        public WebService(SunfishServiceConfiguration ssc) : base(ssc)
        {
            vfs.AddVirtualFolder(null, new VFSFolderFileSystem(ssc.GetConf<string>(WebServiceConfigurator.CFG_PATH)));
            index = ssc.GetConf<string>(WebServiceConfigurator.CFG_INDEX);
            if (string.IsNullOrWhiteSpace(index))
                index = null;
            allowNavigation = ssc.GetConf<bool>(WebServiceConfigurator.CFG_SHARE);
            allowSubfolderNavigation = ssc.GetConf<bool>(WebServiceConfigurator.CFG_NAVIGATION);
        }

        public override void Process(string path, HttpCall call)
        {
            if (path.EndsWith("/"))
            {
                //Directory entry, go for index file or navigation
            }
            else
            {
                using (Stream s = vfs.OpenRead(path))
                {
                    //call.Out.
                }
            }
            call.Write("PoFale!!");
        }

        protected override void Start()
        {
        }

        protected override void Stop()
        {
        }

        public override string Description => "For Webpages or file sharing";
    }
}
