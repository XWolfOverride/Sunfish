using DolphinWebXplorer2.Configurator;
using DolphinWebXplorer2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2
{
    public abstract class SunfishService
    {
        #region Static / Type management

        private static Type[] ServiceTypes;

        static SunfishService()
        {
            ScanForServices();
        }

        private static void ScanForServices()
        {
            List<Type> st = new List<Type>();
            ScanForServices(Assembly.GetExecutingAssembly(), st);
            ServiceTypes = st.ToArray();
        }

        private static void ScanForServices(Assembly a, List<Type> st)
        {
            Type tme = typeof(SunfishService);
            foreach (Type t in a.GetTypes())
            {
                if (tme.IsAssignableFrom(t) && !t.IsAbstract && t != typeof(ErrorService))
                    st.Add(t);
            }
        }

        private static Type GetServiceTypeOf(string type)
        {
            Type stype = null;
            foreach (Type t in ServiceTypes)
                if (t.Name == type)
                    stype = t;
            if (stype == null)
                stype = typeof(ErrorService);
            return stype;
        }

        internal static SunfishService Instance(SunfishServiceConfiguration ssc)
        {
            Type stype = GetServiceTypeOf(ssc.Type);
            try
            {
                return (SunfishService)stype.GetConstructor(new Type[] { typeof(SunfishServiceConfiguration) }).Invoke(new Object[] { ssc });
            }
            catch { return new ErrorService(ssc); }
        }

        public static SunfishServiceConfigurator GetConfigurator(string type)
        {
            return SunfishServiceConfigurator.GetConfiguratorForService(GetServiceTypeOf(type));
        }

        public static string[] GetTypes()
        {
            List<string> lresult = new List<string>();
            foreach (Type t in ServiceTypes)
                lresult.Add(t.Name);
            return lresult.ToArray();
        }

        #endregion

        private SunfishServiceConfigurator configurator;

        public SunfishService(SunfishServiceConfiguration ssc)
        {
            Configuration = ssc;
            configurator = SunfishServiceConfigurator.GetConfiguratorForService(GetType());
        }

        protected abstract void Start();
        protected abstract void Stop();

        #region GET/SET

        private void SetEnabled(bool enabled)
        {
            if (Enabled == enabled)
                return;
            if (enabled)
                Start();
            else
                Stop();
            Configuration.Enabled = enabled;
        }

        #endregion

        public SunfishServiceConfiguration Configuration { get; }
        public abstract string Description { get; }
        internal protected SunfishServiceConfigurator Configurator => configurator;
        public bool Enabled { get => Configuration.Enabled; set => SetEnabled(value); }
    }
}
