using rail;
using Utility;

internal class TGPEventManager
{
	private string sessionId;

	public bool SessionResponseReceived
	{
		get;
		private set;
	}

	public string railId
	{
		get;
		private set;
	}

	public TGPEventManager(RailID railID)
	{
		railId = railID.id_.ToString();
		SessionResponseReceived = false;
	}

	public bool GetSession(out string sessionIDresult)
	{
		if (!SessionResponseReceived)
		{
			sessionIDresult = string.Empty;
			return false;
		}
		sessionIDresult = sessionId;
		return true;
	}

	public void RailCallBackGetSession(RAILEventID id, EventBase data)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		if ((int)id == 13001)
		{
			AcquireSessionTicketResponse val = data;
			sessionId = val.session_ticket.ticket;
			SessionResponseReceived = true;
			Console.Log("TGP Rail callback get session: returns session id:" + sessionId);
		}
		else
		{
			Console.LogError("new event!");
		}
	}
}
