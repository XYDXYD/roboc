using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	internal class DefaultAvatarButton : MonoBehaviour, IChainListener
	{
		[SerializeField]
		private UITexture Texture;

		[SerializeField]
		private GameObject HighlightState;

		private BubbleSignal<IChainRoot> _bubbleSignal;

		private int _index;

		public DefaultAvatarButton()
			: this()
		{
		}

		public void SetTexture(Texture2D newTexture)
		{
			Texture.set_mainTexture(newTexture);
		}

		private void SetHighlightState(bool selected)
		{
			HighlightState.SetActive(selected);
		}

		public void Listen(object message)
		{
			if (message is AvatarSelectionChangedData)
			{
				AvatarSelectionChangedData avatarSelectionChangedData = message as AvatarSelectionChangedData;
				if (!avatarSelectionChangedData.avatarInfo.UseCustomAvatar && avatarSelectionChangedData.avatarInfo.AvatarId == _index)
				{
					SetHighlightState(selected: true);
				}
				else
				{
					SetHighlightState(selected: false);
				}
			}
		}

		public void Initialise()
		{
			_bubbleSignal = new BubbleSignal<IChainRoot>(this.get_transform());
		}

		public void OnClick()
		{
			_bubbleSignal.Dispatch<SelectDefaultAvatarButtonData>(new SelectDefaultAvatarButtonData(_index));
		}

		public void SetSelfIndex(int index)
		{
			_index = index;
		}
	}
}
