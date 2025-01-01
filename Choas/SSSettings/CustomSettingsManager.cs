using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserSettings.ServerSpecific;

namespace Choas.SSSettings
{
    public static class CustomSettingsManager //Stole this from the CustomCommands project
    {
        //An enum containing every single custom setting ID that we will be using.
        //To keep it simple and easier to track, IDs need to be prefixed with what they are being used for.
        //SCP is used for CustomSCPSettings
        //Human is used for CustomHumanSettings
        public enum SettingsIDs
        {
            SpecialButton = 55
        }

        /// <summary>
        /// A <see cref="CustomSettingsBase"/> array containing all the custom settings that we are wanting to register. If you don't want a setting to be registered, simply don't add it to this array.
        /// </summary>
        public static readonly CustomSettingsBase[] CustomSettings =
        {
            new CustomGlobalSettings(),
        };

        public static ServerSpecificSettingBase[] GetAllSettings(bool deactivate = false)
        {
            List<ServerSpecificSettingBase> ssSettingBases = new List<ServerSpecificSettingBase>();

            foreach (var customSettings in CustomSettings)
            {
                if (deactivate)
                    customSettings.Deactivate();
                else
                    customSettings.Activate();

                ssSettingBases.AddRange(customSettings.SettingBases);
            }

            return ssSettingBases.ToArray();
        }
    }
}
