using Authentication;
using ChatServiceLayer;
using SocialServiceLayer;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Utility;

internal abstract class IgnoreList : IInitialize, IWaitForFrameworkDestruction
{
	private HashSet<string> _ignoreList = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

	[CompilerGenerated]
	private static Action<ServiceBehaviour> _003C_003Ef__mg_0024cache0;

	[Inject]
	internal IServiceRequestFactory requestFactory
	{
		get;
		private set;
	}

	[Inject]
	internal ChatCommands chatCommands
	{
		get;
		private set;
	}

	[Inject]
	internal IChatRequestFactory chatRequestFactory
	{
		get;
		private set;
	}

	[Inject]
	internal ChatPresenter chatPresenter
	{
		get;
		private set;
	}

	[Inject]
	internal ISocialRequestFactory SocialRequestFactory
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

	protected abstract void BlockFriend(string user);

	void IInitialize.OnDependenciesInjected()
	{
		chatCommands.RegisterCommand("block", ChatCommands.CommandData.AccessLevel.All, StringTableBase<StringTable>.Instance.GetString("strBlockCommandUsage"), IgnoreUser);
		chatCommands.RegisterCommand("unblock", ChatCommands.CommandData.AccessLevel.All, StringTableBase<StringTable>.Instance.GetString("strUnblockCommandUsage"), UnignoreUser);
		chatCommands.RegisterCommand("listBlocks", ChatCommands.CommandData.AccessLevel.All, StringTableBase<StringTable>.Instance.GetString("strListBlocksCommandUsage"), ListIgnoresCommand);
		chatRequestFactory.Create<IGetChatIgnoresRequest>().SetAnswer(new ServiceAnswer<HashSet<string>>(OnIgnoresLoaded, OnLoadIgnoresFail)).Execute();
	}

	public void OnFrameworkDestroyed()
	{
		chatCommands.DeregisterCommand("block");
		chatCommands.DeregisterCommand("unblock");
		chatCommands.DeregisterCommand("listBlocks");
	}

	public bool ShouldIgnore(string user)
	{
		return _ignoreList.Contains(user);
	}

	protected void RemoveAndBlockFriend(string user)
	{
		IServiceRequest serviceRequest = SocialRequestFactory.Create<IRemoveFriendRequest, string>(user).SetAnswer(new ServiceAnswer<IList<Friend>>(delegate
		{
			BlockUser(user, isFriend: true);
		}, OnError));
		serviceRequest.Execute();
	}

	protected static bool IsFriend(IList<Friend> friends, string user)
	{
		bool result = false;
		for (int i = 0; i < friends.Count; i++)
		{
			Friend friend = friends[i];
			if (friend.Name.Equals(user, StringComparison.InvariantCultureIgnoreCase))
			{
				result = (friend.InviteStatus == FriendInviteStatus.Accepted);
				break;
			}
		}
		return result;
	}

	protected void BlockUser(string user, bool isFriend)
	{
		chatRequestFactory.Create<IIgnoreUserRequest, string>(user).SetAnswer(new ServiceAnswer<IgnoreUserResponse>(delegate(IgnoreUserResponse response)
		{
			OnUserIgnoreResponse(response, isFriend);
		}, OnIgnoreUserFailed)).Execute();
	}

	private void OnIgnoresLoaded(HashSet<string> ignoreList)
	{
		_ignoreList = ignoreList;
	}

	private void OnLoadIgnoresFail(ServiceBehaviour behaviour)
	{
		RemoteLogger.Error(behaviour.errorTitle, behaviour.errorBody, null);
		Console.LogError(behaviour.errorTitle + " " + behaviour.errorBody);
	}

	private bool IgnoreUser(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return false;
		}
		string user = input.Split(' ')[0];
		if (user.ToLower() == User.Username.ToLower() || user.ToLower() == User.DisplayName.ToLower())
		{
			chatCommands.CustomError("You cannot block yourself");
			return true;
		}
		if (_ignoreList.Contains(user))
		{
			chatCommands.CustomError("That user is already being blocked");
			return true;
		}
		SocialRequestFactory.Create<IGetFriendListRequest>().SetAnswer(new ServiceAnswer<GetFriendListResponse>(delegate(GetFriendListResponse friendsListResponse)
		{
			OnGotFriendListBlockUser(friendsListResponse.friendsList, user);
		}, OnIgnoreUserFailed)).Execute();
		return true;
	}

	private void OnGotFriendListBlockUser(IList<Friend> friends, string user)
	{
		if (IsFriend(friends, user))
		{
			BlockFriend(user);
		}
		else
		{
			BlockUser(user, isFriend: false);
		}
	}

	private void OnError(ServiceBehaviour serviceBehaviour)
	{
		serviceBehaviour.SetAlternativeBehaviour(delegate
		{
		}, StringTableBase<StringTable>.Instance.GetString("strCancel"));
		ErrorWindow.ShowServiceErrorWindow(serviceBehaviour);
	}

	private void OnUserIgnoreResponse(IgnoreUserResponse response, bool isFriend)
	{
		if (response.Successful)
		{
			_ignoreList = response.UpdatedIgnoreList;
			chatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString((!isFriend) ? "strUserBlocked" : "strUserBlockFriendRemoved"));
		}
		else
		{
			chatCommands.CustomError(response.Message);
		}
	}

	private void OnIgnoreUserFailed(ServiceBehaviour serviceBehaviour)
	{
		if (serviceBehaviour.errorCode == 5)
		{
			chatPresenter.SystemMessage("That user does not exist");
		}
		else
		{
			ErrorWindow.ShowServiceErrorWindow(serviceBehaviour);
		}
	}

	private bool UnignoreUser(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return false;
		}
		string text = input.Split(' ')[0];
		if (!_ignoreList.Contains(text))
		{
			chatCommands.CustomError("That user is not being blocked");
			return true;
		}
		chatRequestFactory.Create<IUnignoreUserRequest, string>(text).SetAnswer(new ServiceAnswer<IgnoreUserResponse>(OnUserUnignoreResponse, ErrorWindow.ShowServiceErrorWindow)).Execute();
		return true;
	}

	private void OnUserUnignoreResponse(IgnoreUserResponse response)
	{
		if (response.Successful)
		{
			_ignoreList = response.UpdatedIgnoreList;
			chatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strUserUnblocked"));
		}
		else
		{
			chatCommands.CustomError(response.Message);
		}
	}

	private bool ListIgnoresCommand(string input)
	{
		if (_ignoreList.Count > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(StringTableBase<StringTable>.Instance.GetString("strBlockedUsersTitle"));
			stringBuilder.Append(" ");
			using (HashSet<string>.Enumerator enumerator = _ignoreList.GetEnumerator())
			{
				bool flag = true;
				while (enumerator.MoveNext())
				{
					if (!flag)
					{
						stringBuilder.Append(", ");
					}
					else
					{
						flag = false;
					}
					stringBuilder.Append(enumerator.Current);
				}
			}
			chatPresenter.SystemMessage(stringBuilder.ToString());
		}
		else
		{
			chatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strNoBlockedUsers"));
		}
		return true;
	}
}
