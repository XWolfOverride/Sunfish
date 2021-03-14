using Sunfish;
using Sunfish.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunfish_Test_Plugin
{
    [DefineConfigurator(typeof(HelloWorldServiceConfigurator))]
    public class HelloWorldService : SunfishService
    {
        private string thing;
        private string pass;
        private bool ronly;

        public HelloWorldService(SunfishServiceConfiguration ssc) : base(ssc)
        {
            thing = ssc.GetConf<string>(HelloWorldServiceConfigurator.CFG_THING, "default-value");
            pass = ssc.GetConf<string>(HelloWorldServiceConfigurator.CFG_PWD);
            ronly = ssc.GetConf<bool>(HelloWorldServiceConfigurator.CFG_RONLY);
        }

        public override string Description => throw new NotImplementedException();

        public override void Process(string path, HttpCall call)
        {
            WebUI.WriteTemplate("sunfish-header", call, null);
            if (thing == "konosuba")
            {
                call.Write("<p>God's blessing on this wonderful world!</p>");
            }
            else
            {
                call.Write("<p>Hello world!</p>");
                call.Write("<p>");
                call.Write("The thing is: " + thing);
                call.Write("</p>");
                if (!string.IsNullOrEmpty(pass))
                {
                    string ppass;
                    if (call.Parameters.TryGetValue("pp", out ppass) && ppass == pass)
                        call.Write("<p>it IS the passowrd</p>");
                    else
                    {
                        call.Write("<p><form action=''>");
                        call.Write("Enter:<input type='password' name='pp'" + (ronly ? " DISABLED" : "") + "><br/><input type='submit' value='Go!'>");
                        call.Write("</form></p>");
                    }
                }
                call.Write("<p>");
                call.Write(ronly ? "Read only!" : "You can write (not read only)");
                call.Write("</p>");
            }
            WebUI.WriteTemplate("sunfish-footer", call, null);
        }

        protected override void Start()
        {
            //Notihng to load when service starts
        }

        protected override void Stop()
        {
            //Nothing to unload when service stops
        }
    }
}
