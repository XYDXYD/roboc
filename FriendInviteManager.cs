using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System;

internal sealed class FriendInviteManager : ITickable, ITickableBase
{
	private enum State
	{
		None,
		Queued,
		Visible
	}

	private int _inviteCount;

	private Action<bool> _callback;

	private State _state;

	private bool _mothershipFlowCompleted;

	[Inject]
	public GenericInfoDisplay infoDisplay
	{
		private get;
		set;
	}

	internal void MothershipFlowCompleted()
	{
		_mothershipFlowCompleted = true;
	}

	internal void Set(int count, Action<bool> callback)
	{
		_inviteCount = count;
		_callback = callback;
		_state = State.Queued;
	}

	public void Tick(float delta)
	{
		if (_mothershipFlowCompleted && _state == State.Queued)
		{
			DisplayInvitiation();
			_state = State.Visible;
		}
	}

	private void DisplayInvitiation()
	{
		GenericErrorData data = new GenericErrorData(bodyText: (_inviteCount != 1) ? StringTableBase<StringTable>.Instance.GetReplaceString("strPendingInvitesCount", "[INVITE_COUNT]", _inviteCount.ToString()) : StringTableBase<StringTable>.Instance.GetReplaceString("strPendingInviteCount", "[INVITE_COUNT]", _inviteCount.ToString()), headerText: StringTableBase<StringTable>.Instance.GetString("strFriends"), okText: StringTableBase<StringTable>.Instance.GetString("strOK"), okClicked: delegate
		{
			Callback(accept: true);
		});
		infoDisplay.ShowInfoDialogue(data);
	}

	private void Callback(bool accept)
	{
		_state = State.None;
		_callback(accept);
	}
}
