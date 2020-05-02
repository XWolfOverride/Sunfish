using DolphinWebXplorer2.Services;
using DolphinWebXplorer2.Services.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2
{
    abstract class SunfishService
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

        public static SunfishService Instance(SunfishServiceConfiguration ssc)
        {
            Type stype = null;
            foreach (Type t in ServiceTypes)
                if (t.Name == ssc.Name)
                    stype = t;
            if (stype == null)
                stype = typeof(ErrorService);
            try
            {
                return (SunfishService)stype.GetConstructor(new Type[] { typeof(SunfishServiceConfiguration) }).Invoke(new Object[] { ssc });
            }
            catch { return new ErrorService(ssc); }
        }

        #endregion

        protected SunfishServiceConfiguration ssc;

        public SunfishService(SunfishServiceConfiguration ssc)
        {
            Configuration = ssc;
        }

        protected abstract ConfigurationScreen GetConfigurationScreen();

        public SunfishServiceConfiguration Configuration { get; }
        public abstract string Description { get; }
        public ConfigurationScreen ConfigurationScreen => GetConfigurationScreen();
    }
}
