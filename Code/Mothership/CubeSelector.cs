using Fabric;
using InputMask;
using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal class CubeSelector : MonoBehaviour
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
		internal ICubeHolder selectedCube
		{
			private get;
			set;
		}

		[Inject]
		internal IInputActionMask inputActionMask
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

		public CubeSelector()
			: this()
		{
		}

		private void Update()
		{
			bool buttonDown = InputRemapper.Instance.GetButtonDown("Pick Cube");
			if (inputActionMask.InputIsAvailable(UserInputCategory.BuildModeInputAxis, 2))
			{
				SelectCube(buttonDown);
			}
		}

		private void SelectCube(bool selectCube)
		{
			if (selectCube && raycastInfo.hitCube != null)
			{
				paintToolPresenter.TrySetCurrentColor(raycastInfo.hitCube.paletteIndex);
				selectedCube.selectedCubeID = raycastInfo.hitCube.persistentCubeData.cubeType;
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.BUILD_KubeCopy));
			}
		}
	}
}
