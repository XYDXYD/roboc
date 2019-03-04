using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.GUI.Clan
{
	public class UploadAvatarView : MonoBehaviour, IChainRoot, IChainListener, IInitialize
	{
		private UITexture[] _avatarPreviews;

		private SocialMessage _message;

		private BubbleSignal<IChainRoot> _bubble;

		private BubbleSignal<IClanRoot> _bubbleToClanRoot;

		[Inject]
		internal UploadAvatarController uploadAvatarController
		{
			private get;
			set;
		}

		[Inject]
		internal PresetAvatarMapProvider PresetAvatarMapProvider
		{
			private get;
			set;
		}

		public UploadAvatarView()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			uploadAvatarController.SetView(this);
		}

		private void Start()
		{
			_avatarPreviews = this.GetComponentsInChildren<UITexture>();
			_message = new SocialMessage(SocialMessageType.CreateClanAvatarChanged, string.Empty);
			_bubble = new BubbleSignal<IChainRoot>(this.get_transform().get_parent());
			_bubbleToClanRoot = new BubbleSignal<IClanRoot>(this.get_transform());
		}

		public void Listen(object message)
		{
			if (message is SocialMessage)
			{
				uploadAvatarController.HandleMessage(message as SocialMessage);
			}
		}

		public void SetClanAvatarCustomPreview(Texture2D texture)
		{
			for (int i = 0; i < _avatarPreviews.Length; i++)
			{
				_avatarPreviews[i].set_mainTexture(texture);
			}
			_message.extraData = ImageConversion.EncodeToJPG(texture);
			_bubble.Dispatch<SocialMessage>(_message);
		}

		public void SetClanAvatarDefaultPreview(int avatarIndex)
		{
			Texture2D presetAvatar = PresetAvatarMapProvider.GetPresetAvatar(avatarIndex);
			for (int i = 0; i < _avatarPreviews.Length; i++)
			{
				_avatarPreviews[i].set_mainTexture(presetAvatar);
			}
			_message.extraData = avatarIndex;
			_bubble.Dispatch<SocialMessage>(_message);
		}

		public void BubbleUpToClanRoot(SocialMessage message)
		{
			_bubbleToClanRoot.TargetedDispatch<SocialMessage>(message);
		}
	}
}
