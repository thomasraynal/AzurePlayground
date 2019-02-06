﻿using AzurePlayground.Service.Shared;
using Dasein.Core.Lite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service
{
    public class MarketServiceConfiguration : ServiceHubConfigurationBase, ICanRegister
    {
        public string Id { get; set; }
        public override string Name { get; set; }
        public override int Version { get; set; }
        public string Identity { get; set; }
        public string Consult { get; set; }
        public string EventStore { get; set; }
    }
}