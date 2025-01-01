using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserSettings.ServerSpecific;

namespace Choas.SSSettings
{
    public abstract class CustomSettingsBase //Stole this from the CustomCommands project
    {
        private static CustomSettingsBase _instance;

        /// <summary>
        /// Name
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Quick description of the setting
        /// </summary>
        public abstract string Description { get; }

        public abstract ServerSpecificSettingBase[] SettingBases { get; }

        public abstract void Activate();
        public abstract void Deactivate();
        public abstract void ProcessUserInput(ReferenceHub hub, ServerSpecificSettingBase setting);
    }
}
