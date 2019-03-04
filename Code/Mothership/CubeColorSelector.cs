using Fabric;
using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal class CubeColorSelector : MonoBehaviour
	{
		[Inject]
		internal CubeRaycastInfo raycastInfo
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeInventory cubeInventory
		{
			private get;
			set;
		}

		[Inject]
		internal PaintToolPresenter paintToolPresenter
		{
			private get;
			set;
		}

		public CubeColorSelector()
			: this()
		{
		}

		private void Update()
		{
			bool buttonDown = InputRemapper.Instance.GetButtonDown("Pick Cube");
			SelectColor(buttonDown);
		}

		public void HandleCharacterInput(InputCharacterData data)
		{
			SelectColor(data.data[13] > 0.1f);
		}

		private void SelectColor(bool selectColor)
		{
			if (selectColor && raycastInfo.hitCube != null)
			{
				paintToolPresenter.TrySetCurrentColor(raycastInfo.hitCube.paletteIndex);
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.BUILD_KubeCopy));
			}
		}
	}
}
