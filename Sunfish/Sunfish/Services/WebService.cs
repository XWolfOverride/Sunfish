﻿using DolphinWebXplorer2.Services.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Services
{
    class WebService : SunfishService
    {
        public WebService(SunfishServiceConfiguration ssc) : base(ssc)
        {
        }

        protected override ConfigurationScreen GetConfigurationScreen()
        {
            return new ConfigurationScreen()
            {
                elements = new ConfigurationElement[]
                {
                    new ConfigurationMessage(ConfigurationMessage.MessageType.ERROR,"Error loading or initializing this service")
                }
            };
        }

        public override string Description => "For Webpages or file sharing";
    }
}