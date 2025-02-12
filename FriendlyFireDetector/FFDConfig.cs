using RedRightHandCore.PluginCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendlyFireDetector
{
	public class FFDConfig : CustomConfig
	{
		public bool ReverseDamage { get; set; } = false;
		public float ReverseDamageModifier { get; set; } = 1f;
	}
}
