using Fabric;
using Mothership;
using Svelto.ES.Legacy;
using Svelto.IoC;
using UnityEngine;

internal sealed class CubeRotation : MonoBehaviour, IHandleEditingInput, IInputComponent, IComponent
{
	[Inject]
	public ICubeHolder selectedCube
	{
		private get;
		set;
	}

	[Inject]
	public ICubeList cubeTypeInventory
	{
		private get;
		set;
	}

	[Inject]
	internal IGUIInputControllerMothership guiController
	{
		private get;
		set;
	}

	public CubeRotation()
		: this()
	{
	}

	private void Start()
	{
		selectedCube.OnCubeSelectedChanged += SelectedCubeChanged;
		PersistentCubeData cubeData = cubeTypeInventory.CubeTypeDataOf(selectedCube.selectedCubeID).cubeData;
		if (!cubeData.canRotate)
		{
			selectedCube.currentRotation = cubeData.rotationWhenFixed;
		}
	}

	private void SelectedCubeChanged(CubeTypeID cubeID)
	{
		PersistentCubeData cubeData = cubeTypeInventory.CubeTypeDataOf(cubeID).cubeData;
		if (!cubeData.canRotate)
		{
			selectedCube.currentRotation = cubeData.rotationWhenFixed;
		}
	}

	public void HandleEditingInput(InputEditingData data)
	{
		if (data[EditingInputAxis.ROTATE_CUBE] != 0f && this.get_isActiveAndEnabled() && guiController.GetActiveScreen() == GuiScreens.BuildMode)
		{
			HandleWheel(data[EditingInputAxis.ROTATE_CUBE]);
		}
	}

	public void HandleWheel(float val)
	{
		PersistentCubeData cubeData = cubeTypeInventory.CubeTypeDataOf(selectedCube.selectedCubeID).cubeData;
		if (cubeData.canRotate)
		{
			if (val > 0f)
			{
				selectedCube.currentRotation += 90;
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.CubeRotate));
			}
			else if (val < 0f)
			{
				selectedCube.currentRotation -= 90;
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.CubeRotate));
			}
		}
	}
}
