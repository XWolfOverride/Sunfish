using DolphinWebXplorer2.Configurator;
using System;

namespace DolphinWebXplorer2.Services
{
    class WebServiceConfigurator : SunfishServiceConfigurator
    {
        internal const string CFG_PATH = "path";
        internal const string CFG_INDEX = "defaultFile";
        internal const string CFG_SHARE = "folderShare";
        internal const string CFG_NAVIGATION = "allowSubfolderNavigation";
        internal const string CFG_UPLOAD = "allowUploads";
        internal const string CFG_EDITOR = "allowEditor";
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
                    new ConfigurationString(CFG_PATH,"Web path")
                    {
                        Tooltip = "Folder, zip or web-file path of site document root",
                        IsDirectoiryPath = true,
                        IsFilePath = true,
                        Mandatory = true
                    },
                    new ConfigurationString(CFG_INDEX,"Default document")
                    {
                        Tooltip = "Default document for directory path",
                        DefaultValue = "index.html"
                    },
                    new ConfigurationBool(CFG_SHARE,"Folder share")
                    {
                        Tooltip = "Allow directory listings, switched off the server will report a forbidden message if the directory has no default file.",
                        DefaultValue = true,
                    },
                    new ConfigurationBool(CFG_NAVIGATION,"Subfolder navigation")
                    {
                        Tooltip = "Allow subfolder navigation.",
                        DefaultValue = true,
                    },
                    new ConfigurationBool(CFG_UPLOAD,"Allow upload")
                    {
                        Tooltip = "Allow upload of files and folders, also enables the create folder option if subfolder navigation is enabled."
                    },
                    new ConfigurationBool(CFG_EDITOR,"Allow text editor")
                    {
                        Tooltip = "Allow text file creation and edition, also enables the create folder option if subfolder navigation is enabled."
                    },
                    new ConfigurationBool(CFG_RENAME,"Allow rename")
                    {
                        Tooltip = "Allow raname of server files and folders."
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
