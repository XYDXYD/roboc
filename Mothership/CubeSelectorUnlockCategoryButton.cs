using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	internal sealed class CubeSelectorUnlockCategoryButton : MonoBehaviour
	{
		private CubeCellWidget _cubeButtonData;

		private BubbleSignal<IChainRoot> _bubbleSignal;

		public CubeSelectorUnlockCategoryButton()
			: this()
		{
		}

		private void Start()
		{
			_cubeButtonData = this.get_transform().get_parent().get_parent()
				.get_gameObject()
				.GetComponent<CubeCellWidget>();
			_bubbleSignal = new BubbleSignal<IChainRoot>(this.get_transform());
		}

		public void OnClick()
		{
			UnlockCubeCategoryButtonData unlockCubeCategoryButtonData = new UnlockCubeCategoryButtonData();
			unlockCubeCategoryButtonData.cubeType = _cubeButtonData.type;
			_bubbleSignal.Dispatch<UnlockCubeCategoryButtonData>(unlockCubeCategoryButtonData);
		}
	}
}
