using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;

namespace Choas
{
    public class Plugin
    {
        [PluginEntryPoint("Choas", "1.0.0", "brings the choas to SL", "Dragon Inn Tech Team")]
        public void OnPluginStart()
        {
            Log.Info($"Let the choas begin");
        }
    }
}
