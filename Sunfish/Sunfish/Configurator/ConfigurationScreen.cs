﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Configurator
{
    public class ConfigurationScreen
    {

        public delegate void AdvancedEditing(SunfishServiceConfiguration ssc);
        //public delegate void AdvancedWebEditing(SunfishServiceConfiguration ssc, <path or data for web calls prcessing> );

        public ConfigurationElement[] Elements;

        public AdvancedEditing Advanced { get; set; }
        //public AdvancedWebEditing AdvancedWeb{get;set;}
    }
}
