using UnityEngine;

namespace Robocraft.GUI
{
	[RequireComponent(typeof(UIImageAdapter))]
	public class GenericImageComponentView : GenericComponentViewBase
	{
		private UIImageAdapter _imageAdapter;

		public override void Setup()
		{
			base.Setup();
			_imageAdapter = this.GetComponent<UIImageAdapter>();
			_imageAdapter.Setup();
		}

		public override void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public override void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		public void SetImage(Texture2D image)
		{
			_imageAdapter.SetTexture(image);
		}
	}
}
