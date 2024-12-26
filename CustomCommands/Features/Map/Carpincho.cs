using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.Map
{
    public class Carpincho
    {
        [PluginEvent()]
        public void DesovarCarpincho(WaitingForPlayersEvent argumentos)
        {
            if (Scp956Pinata.TryGetInstance(out var capy))
            {
                capy.Network_carpincho = 69;
            }
        }
    }
}
