using UnityEngine;

namespace Robocraft.GUI
{
	[RequireComponent(typeof(UITexture))]
	public class UIImageAdapter : MonoBehaviour
	{
		private UITexture _textureSource;

		public UIImageAdapter()
			: this()
		{
		}

		public void Setup()
		{
			_textureSource = this.GetComponent<UITexture>();
		}

		public void SetTexture(Texture2D textureSource)
		{
			_textureSource.set_mainTexture(textureSource);
		}

		public void Clear()
		{
			_textureSource = null;
		}
	}
}
