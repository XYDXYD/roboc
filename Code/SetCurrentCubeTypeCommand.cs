using Fabric;
using Mothership;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class SetCurrentCubeTypeCommand : MonoBehaviour
{
	public CubeTypeID cube;

	private BubbleSignal<IChainRoot> _bubbleSignal;

	[Inject]
	internal ICubeHolder selectedCube
	{
		private get;
		set;
	}

	[Inject]
	internal ICubeInventory inventory
	{
		private get;
		set;
	}

	[Inject]
	internal ICubePrerequisites cubePrerequisites
	{
		private get;
		set;
	}

	[Inject]
	internal CubePrerequisitesFailedObservable cubePrerequisitesFailedObservable
	{
		private get;
		set;
	}

	public SetCurrentCubeTypeCommand()
		: this()
	{
	}

	private void Awake()
	{
		_bubbleSignal = new BubbleSignal<IChainRoot>(this.get_transform());
	}

	private void OnClick()
	{
		if (selectedCube != null)
		{
			if (!cubePrerequisites.CanUseCube(cube, out string errorStringKey))
			{
				cubePrerequisitesFailedObservable.Dispatch(ref errorStringKey);
			}
			else if (inventory.IsCubeOwned(cube))
			{
				selectedCube.selectedCubeID = cube;
				_bubbleSignal.Dispatch<CubeTypeID>(cube);
			}
			else
			{
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonError));
			}
		}
	}
}
