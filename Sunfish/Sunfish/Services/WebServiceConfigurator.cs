using DolphinWebXplorer2.Configurator;
using System;

namespace DolphinWebXplorer2.Services
{
    class WebServiceConfigurator : SunfishServiceConfigurator
    {
        internal const string CFG_PATH = "path";
        internal const string CFG_INDEX = "defaultFile";
        internal const string CFG_SHARE = "folderShare";
        internal const string CFG_RONLY = "readOnly";
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
                    new ConfigurationString(CFG_PATH,"Directory")
                    {
                        Tooltip = "Folder of site document root",
                        IsDirectoiryPath = true,
                        IsFilePath = true,
                        Mandatory = true
                    },
                    new ConfigurationString(CFG_INDEX,"Default document")
                    {
                        Tooltip = "Default document for directory path",
                        DefaultValue = "index.html"
                    },
                    new ConfigurationBool(CFG_SHARE,"Folder share (Web && DAV)")
                    {
                        Tooltip = "Allow directory listings, switched off the server will report a forbidden message if the directory has no default file. Also enables DAV protocol",
                        DefaultValue = true,
                    },
                    new ConfigurationBool(CFG_RONLY,"Read only")
                    {
                        Tooltip = "Avoids upload of files and folders, create folders, edit files, copy and move.",
                        DefaultValue = true,
                    },
                    new ConfigurationBool(CFG_DELETE,"Allow delete")
                    {
                        Tooltip = "Allow delete files and folders."
                    },
                    new ConfigurationBool(CFG_EXECUTE,"Allow execute")
                    {
                        Tooltip = "Allow application execution on server."
                    },
                    new ConfigurationMessage(ConfigurationMessage.MessageType.WARNING,"WARNING: Sunfish is executed as elevated process, any anction or process started by sunfish will be also elevated.")
                },
                Advanced = AdvancedEditing
            };
        }

    }
}
