using Avatars;
using Svelto.Command;
using Svelto.IoC;

internal class SetLocalPlayerAvatarCommand : ICommand
{
	private AvatarInfo _avatarInfo;

	[Inject]
	internal AvatarPresenterLocalPlayer AvatarPresenterLocalPlayer
	{
		private get;
		set;
	}

	[Inject]
	internal IMultiAvatarLoader AvatarLoader
	{
		private get;
		set;
	}

	public ICommand Inject(AvatarInfo avatarInfo)
	{
		_avatarInfo = avatarInfo;
		return this;
	}

	public void Execute()
	{
		if (_avatarInfo.UseCustomAvatar)
		{
			AvatarPresenterLocalPlayer.SetCustomAvatarExpected();
			AvatarLoader.ForceRequestAvatar(AvatarType.PlayerAvatar, AvatarUtils.LocalPlayerAvatarName);
		}
		else
		{
			AvatarPresenterLocalPlayer.ApplyAvatarAndShow(_avatarInfo);
		}
	}
}
