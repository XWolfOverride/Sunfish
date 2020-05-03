using DolphinWebXplorer2.Configurator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Services
{
    class WebServiceConfigurator : SunfishServiceConfigurator
    {
        internal const string CFG_PATH = "path";
        internal const string CFG_SHARE = "folderShare";
        internal const string CFG_NAVIGATION = "allowSubfolderNavigation";
        internal const string CFG_UPLOAD = "allowUploads";
        internal const string CFG_RENAME = "allowRenaming";
        internal const string CFG_DELETE = "allowDeletion";
        internal const string CFG_EXECUTE = "allowServerExecution";

        private void AdvancedEditing(SunfishServiceConfiguration ssc)
        {

        }

        internal protected override ConfigurationScreen GetConfigurationScreen()
        {
            return new ConfigurationScreen()
            {
                Elements = new ConfigurationElement[]
                {
                    new ConfigurationString(CFG_PATH)
                    {
                        Tooltip = "Folder, zip or web-file path of site document root",
                        Label = "Web path",
                        IsDirectoiryPath = true,
                        IsFilePath = true,
                    }
                },
                Advanced = AdvancedEditing
            };
        }

    }
}
