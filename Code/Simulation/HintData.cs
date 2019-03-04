using UnityEngine;

namespace Simulation
{
	internal struct HintData
	{
		public readonly Texture2D texture;

		public readonly string text;

		public HintData(Texture2D texture, string text)
		{
			this.texture = texture;
			this.text = text;
		}
	}
}
