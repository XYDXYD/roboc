using Avatars;
using SocialServiceLayer;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using System;

internal class AvatarPresenterLocalPlayerIconOnly : IInitialize, IWaitForFrameworkDestruction
{
	private AvatarViewLocalPlayerIconOnly _avatarView;

	private bool _localAvatarIsCustom;

	[Inject]
	internal IServiceRequestFactory ServiceRequestFactory
	{
		private get;
		set;
	}

	[Inject]
	internal ISocialRequestFactory socialRequestFactory
	{
		private get;
		set;
	}

	[Inject]
	internal ICommandFactory CommandFactory
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

	[Inject]
	internal LoadingIconPresenter loadingPresenter
	{
		private get;
		set;
	}

	[Inject]
	internal AvatarAvailableObserver AvatarAvailableObserver
	{
		private get;
		set;
	}

	[Inject]
	internal IMultiAvatarLoader MultiAvatarLoader
	{
		private get;
		set;
	}

	unsafe void IInitialize.OnDependenciesInjected()
	{
		_localAvatarIsCustom = false;
		AvatarAvailableObserver.AddAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
	{
		AvatarAvailableObserver.RemoveAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	public void SetCustomAvatarExpected()
	{
		_avatarView.Texture.set_mainTexture(AvatarUtils.StillLoadingTexture);
		_localAvatarIsCustom = true;
	}

	public void ReceiveMessage(object message)
	{
	}

	public void RegisterView(AvatarViewLocalPlayerIconOnly avatarView)
	{
		_avatarView = avatarView;
		Hide();
		ServiceRequestFactory.Create<IGetAvatarInfoRequest>().SetAnswer(new ServiceAnswer<AvatarInfo>(ApplyAvatarAndShow, GetAvatarInfoFailed)).Execute();
	}

	public void ApplyAvatarAndShow(AvatarInfo avatarInfo)
	{
		_localAvatarIsCustom = avatarInfo.UseCustomAvatar;
		if (avatarInfo.UseCustomAvatar)
		{
			MultiAvatarLoader.RequestAvatar(AvatarType.PlayerAvatar, AvatarUtils.LocalPlayerAvatarName);
		}
		else
		{
			_avatarView.Texture.set_mainTexture(PresetAvatarMapProvider.GetPresetAvatar(avatarInfo.AvatarId));
		}
		Show();
	}

	private void HandleOnAvatarAvailable(ref AvatarAvailableData avatarData)
	{
		if (avatarData.avatarName == AvatarUtils.LocalPlayerAvatarName && avatarData.avatarType == AvatarType.PlayerAvatar && _localAvatarIsCustom)
		{
			_avatarView.Texture.set_mainTexture(avatarData.texture);
		}
	}

	private void GetAvatarInfoFailed(ServiceBehaviour serviceBehaviour)
	{
		ErrorWindow.ShowServiceErrorWindow(serviceBehaviour);
	}

	private void Show()
	{
		_avatarView.get_gameObject().SetActive(true);
	}

	private void Hide()
	{
		_avatarView.get_gameObject().SetActive(false);
	}
}
