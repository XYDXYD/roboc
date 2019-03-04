using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal class AvatarViewLocalPlayerIconOnly : MonoBehaviour, IChainListener, IInitialize
{
	public UITexture Texture;

	[Inject]
	internal AvatarPresenterLocalPlayerIconOnly avatarPresenterLocalPlayer
	{
		private get;
		set;
	}

	public AvatarViewLocalPlayerIconOnly()
		: this()
	{
	}

	void IChainListener.Listen(object message)
	{
		avatarPresenterLocalPlayer.ReceiveMessage(message);
	}

	private void Start()
	{
		avatarPresenterLocalPlayer.RegisterView(this);
	}

	public void OnDependenciesInjected()
	{
	}
}
