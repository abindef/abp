﻿using Volo.Abp.AspNetCore.Mvc.UI.Bundling.Libraries.Bootstrap;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling.Libraries.DatatablesNet;
using Volo.Abp.Modularity;

namespace Volo.Abp.AspNetCore.Mvc.UI.Bundling.Libraries.DatatablesNetBs4
{
    [DependsOn(typeof(DatatablesNetScriptContributor))]
    [DependsOn(typeof(BootstrapScriptContributor))]
    public class DatatablesNetBs4ScriptContributor : BundleContributor
    {
        public override void ConfigureBundle(BundleConfigurationContext context)
        {
            context.Files.Add("/libs/datatables.net-bs4/js/dataTables.bootstrap4.js");
        }
    }
}