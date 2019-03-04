using Authentication;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using Svelto.WeakEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Utility;

internal abstract class PhotonClient : LoadBalancingClient, IPhotonClient
{
	public enum ClientCCUCheckStatus
	{
		NotNeeded,
		WaitingForMasterEvent,
		CCUCheckPassed,
		CCUCheckFailed,
		CCUCheckPassedAndClientConnected
	}

	public class PhotonConnectionMonobehavior : MonoBehaviour
	{
		public Action OnDestroyActions;

		public PhotonConnectionMonobehavior()
			: this()
		{
		}

		private void OnDestroy()
		{
			if (OnDestroyActions != null)
			{
				OnDestroyActions();
				OnDestroyActions = null;
			}
		}
	}

	private Dictionary<string, PhotonRequestContainer> requestContainers = new Dictionary<string, PhotonRequestContainer>();

	public Action<int, List<string>> OnCCUExceeded;

	public Action OnCCUPassed;

	private PhotonEventRegistry _eventRegistry;

	private ClientCCUCheckStatus _ccuCheckStatus;

	private readonly string _serverAddress;

	private readonly object _lockDateTime = new object();

	private MultiThreadRunner _whileLoadingThreadRunner;

	private int _connectionAttempts;

	private DateTime _lastService;

	private Exception _disconnectException;

	private ITaskRoutine _reconnectTask;

	private ClientState _lastState;

	private bool _isConnecting;

	private bool _isJoiningGameServer;

	private PhotonConnectionMonobehavior _photonConnectionBehaviour;

	public bool IsConnectedAndReady => (int)this.get_State() == 8;

	public bool IsConnecting => _isConnecting;

	public WeakEvent onConnected
	{
		get;
		set;
	}

	public Action onUnexpectedDisconnection
	{
		get;
		set;
	}

	public virtual string UserName => User.Username;

	protected bool mustDisconnect
	{
		get;
		set;
	}

	protected abstract string serverAddressS3Key
	{
		get;
	}

	protected abstract string serverAddressLocalOverrideKey
	{
		get;
	}

	protected abstract string applicationID
	{
		get;
	}

	protected abstract string appVersion
	{
		get;
	}

	protected abstract int serviceConnectionInterval
	{
		get;
	}

	protected abstract int pingInterval
	{
		get;
	}

	private int maxServiceDelay => pingInterval;

	protected abstract int connectAttempts
	{
		get;
	}

	protected abstract float connectAttemptDelay
	{
		get;
	}

	protected abstract byte duplicateLoginCode
	{
		get;
	}

	protected abstract byte ccuExceededCode
	{
		get;
	}

	protected abstract byte ccuCheckPassedCode
	{
		get;
	}

	protected abstract byte maxPlayerPerRoom
	{
		get;
	}

	protected abstract int emptyRoomTtl
	{
		get;
	}

	protected abstract bool CCUCheckRequired
	{
		get;
	}

	protected PhotonClient(ConnectionProtocol protocol = 0)
		: this(protocol)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		if ((!ServEnv.Exists() || !ServEnv.TryGetValue(serverAddressLocalOverrideKey, out _serverAddress)) && !ClientConfigData.TryGetValue(serverAddressS3Key, out _serverAddress))
		{
			throw new Exception("Failed to get address of server for " + applicationID);
		}
		onConnected = new WeakEvent();
		onUnexpectedDisconnection = null;
	}

	public void SetEventRegistry(PhotonEventRegistry eventRegistry)
	{
		_eventRegistry = eventRegistry;
	}

	public void AddCCUExceededEventHandler(Action<int, List<string>> OnCCuExceeded_)
	{
		OnCCUExceeded = (Action<int, List<string>>)Delegate.Combine(OnCCUExceeded, OnCCuExceeded_);
	}

	public void AddCCUPassedEventHandler(Action OnCCUPassed_)
	{
		OnCCUPassed = (Action)Delegate.Combine(OnCCUPassed, OnCCUPassed_);
	}

	public void RemoveCCUExceededEventHandler(Action<int, List<string>> OnCCuExceeded_)
	{
		OnCCUExceeded = (Action<int, List<string>>)Delegate.Remove(OnCCUExceeded, OnCCuExceeded_);
	}

	public void RemoveCCUPassedEventHandler(Action OnCCUPassed_)
	{
		OnCCUPassed = (Action)Delegate.Remove(OnCCUPassed, OnCCUPassed_);
	}

	public override void OnEvent(EventData eventData)
	{
		if ((eventData.Code == ccuCheckPassedCode || eventData.Code == ccuExceededCode) && !CCUCheckRequired)
		{
			Console.LogWarning("WARNING! photon master sent CCU check status information, but it was not expected for this type of photon server on the client.");
		}
		if (eventData.Code == ccuCheckPassedCode)
		{
			if (OnCCUPassed != null)
			{
				OnCCUPassed();
			}
			JoinRandomRoom();
		}
		else if (eventData.Code == ccuExceededCode)
		{
			int arg = 0;
			if (eventData.Parameters.ContainsKey(8))
			{
				arg = (int)eventData.Parameters[8];
			}
			List<string> list = new List<string>();
			if (eventData.Parameters.ContainsKey(2))
			{
				object[] array = (object[])eventData.Parameters[2];
				for (int i = 0; i < array.Length; i++)
				{
					list.Add((string)array[i]);
				}
			}
			_ccuCheckStatus = ClientCCUCheckStatus.CCUCheckFailed;
			if (OnCCUExceeded != null)
			{
				OnCCUExceeded(arg, list);
			}
		}
		else if (eventData.Code == duplicateLoginCode)
		{
			mustDisconnect = true;
			ForceDisconnect();
			throw new DuplicateLoginException(StringTableBase<StringTable>.Instance.GetString("strLoggedInOtherLocation"));
		}
		bool flag = false;
		if (_eventRegistry != null)
		{
			flag = _eventRegistry.OnEvent(eventData);
		}
		if (!flag)
		{
			this.OnEvent(eventData);
		}
	}

	public void RaiseInternalEvent<TEventListener, TEventData>(TEventData eventData) where TEventListener : IServiceEventListener<TEventData>
	{
		if (_eventRegistry != null)
		{
			_eventRegistry.InternalEvent<TEventListener, TEventData>(eventData);
		}
	}

	public override void DebugReturn(DebugLevel level, string message)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		if ((int)level != 2)
		{
			if ((int)level == 1 && (!message.Contains("32760") || !message.Contains("225")))
			{
				RemoteLogger.Error($"{applicationID}: {message}", null, null);
			}
		}
		else
		{
			Console.LogWarning("Server warning from " + applicationID + " : " + message);
		}
	}

	public override void OnOperationResponse(OperationResponse operationResponse)
	{
		if (operationResponse.OperationCode == 230 && operationResponse.Parameters.ContainsKey(245))
		{
			if (operationResponse.ReturnCode == 0)
			{
				_ccuCheckStatus = ClientCCUCheckStatus.CCUCheckPassedAndClientConnected;
				OnConnectedAndReady();
			}
			else
			{
				Console.LogException((Exception)new PhotonConnectionException("Failed initial authentication request."));
			}
			return;
		}
		if (operationResponse.Parameters.ContainsKey(0))
		{
			string key = (string)operationResponse.Parameters[0];
			if (requestContainers.TryGetValue(key, out PhotonRequestContainer value))
			{
				if (value != null)
				{
					try
					{
						value.OnOperationResponse(operationResponse);
					}
					catch (Exception ex)
					{
						Console.LogException(ex);
					}
				}
				requestContainers.Remove(key);
			}
			else
			{
				Console.LogException((Exception)new PhotonConnectionException($"Received operation request with ID not found in request containers. Operation code: {operationResponse.OperationCode}"));
			}
		}
		else
		{
			ProcessPhotonStandardCodes(operationResponse);
		}
		this.OnOperationResponse(operationResponse);
	}

	public bool SendOp(OperationRequest opRequest, bool isEncrypted = false)
	{
		if (IsConnectedAndReady)
		{
			return SendOpInternal(opRequest, isEncrypted, 0);
		}
		return false;
	}

	internal void CreateRequestWrapperAndExecuteRequest(PhotonRequestContainer requestContainer)
	{
		requestContainer.SetClient(this);
		requestContainers.Add(requestContainer.GUID, requestContainer);
		if (IsConnectedAndReady)
		{
			requestContainer.SendRequest();
		}
		else
		{
			ConnectToPhoton();
		}
	}

	private void ClearContainersOnDisconnect(bool isUnexpected, Exception managedException)
	{
		Dictionary<string, PhotonRequestContainer>.Enumerator enumerator = requestContainers.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Current.Value == null)
			{
				Console.LogWarning("Null request container found on disconnect (not fatal)");
			}
			else
			{
				enumerator.Current.Value.ClientDisconnected(isUnexpected, managedException);
			}
		}
		requestContainers.Clear();
	}

	private bool SendOpInternal(OperationRequest opRequest, bool isEncrypted, byte channelId = 0)
	{
		bool flag = false;
		if (isEncrypted)
		{
			return base.loadBalancingPeer.OpCustom(opRequest.OperationCode, opRequest.Parameters, true, channelId, isEncrypted);
		}
		return base.loadBalancingPeer.OpCustom(opRequest.OperationCode, opRequest.Parameters, true);
	}

	private IEnumerator CreateNewTaskWithTimer()
	{
		Console.Log("Connection task started");
		while ((int)base.loadBalancingPeer.get_PeerState() != 0)
		{
			yield return null;
		}
		yield return (object)new WaitForSecondsEnumerator(connectAttemptDelay);
		ConnectToPhoton();
	}

	public void TearDown()
	{
		Console.Log("Clear Containers for " + base.GetType());
		using (List<string>.Enumerator enumerator = new List<string>(requestContainers.Keys).GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				requestContainers[enumerator.Current] = null;
			}
		}
	}

	public void ForceDisconnect()
	{
		_isConnecting = false;
		_isJoiningGameServer = false;
		mustDisconnect = true;
		Disconnect();
		if (_whileLoadingThreadRunner != null)
		{
			CleanUpThreadRunner();
		}
		DestroyPhotonGameObject();
	}

	protected virtual void OnConnectedToMaster()
	{
		Console.Log("CLIENT IS CONNECTED TO MASTER");
		if (CCUCheckRequired)
		{
			_ccuCheckStatus = ClientCCUCheckStatus.WaitingForMasterEvent;
		}
		else
		{
			JoinRandomRoom();
		}
	}

	protected void DisconnectWithError(Exception exception)
	{
		_connectionAttempts = 0;
		mustDisconnect = true;
		_disconnectException = exception;
		ForceDisconnect();
	}

	protected virtual void OnConnectedAndAuthenticating()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		Console.Log("Connected and authenticating");
		OperationRequest val = new OperationRequest();
		val.OperationCode = 230;
		val.Parameters = new Dictionary<byte, object>();
		val.Parameters[245] = GetSlaveAuthenticationParametersTencent();
		if (!SendOp(val))
		{
			throw new Exception("No connection to " + applicationID);
		}
	}

	protected virtual void OnConnectedAndReady()
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		_isConnecting = false;
		_isJoiningGameServer = false;
		_connectionAttempts = 0;
		Console.Log(applicationID + " connected and ready");
		_connectionAttempts = 0;
		if (_eventRegistry != null)
		{
			_eventRegistry.Connected();
		}
		foreach (KeyValuePair<string, PhotonRequestContainer> requestContainer in requestContainers)
		{
			if (requestContainer.Value != null)
			{
				requestContainer.Value.SendRequest();
			}
		}
		onConnected.Invoke();
		onConnected = new WeakEvent();
	}

	protected virtual void OnDisconnected()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (!_isConnecting)
		{
			onConnected = new WeakEvent();
		}
		if (!CheckIfDisconnectedWhileConnecting())
		{
			if (!mustDisconnect && onUnexpectedDisconnection != null)
			{
				Console.Log("Invoking unexpected disconnect handler!!!!!!!");
				onUnexpectedDisconnection();
				onUnexpectedDisconnection = null;
			}
			if (_whileLoadingThreadRunner != null)
			{
				CleanUpThreadRunner();
			}
			if (_eventRegistry != null)
			{
				_eventRegistry.Disconnected();
			}
			ClearContainersOnDisconnect(!mustDisconnect, GetConnectionFailureException());
			if (!mustDisconnect)
			{
				Console.LogError($"Disconnected from / failed to connect to {applicationID}. Reason: {this.get_DisconnectedCause()}");
			}
			else
			{
				Console.Log("Expected disconnection happened");
			}
		}
	}

	private void CleanUpThreadRunner()
	{
		_whileLoadingThreadRunner.StopAllCoroutines();
		_whileLoadingThreadRunner.Dispose();
		_whileLoadingThreadRunner = null;
	}

	protected virtual string GetAuthenticationParameters()
	{
		return User.AuthToken + ";" + User.RefreshToken;
	}

	protected virtual string GetSlaveAuthenticationParameters()
	{
		return User.TGPID + ";" + User.AuthToken + ";" + User.RefreshToken;
	}

	protected virtual string GetSlaveAuthenticationParametersTencent()
	{
		return User.TGPID + ";" + User.AuthToken + ";" + User.RefreshToken;
	}

	protected void ConnectToPhoton()
	{
		Console.Log($"{applicationID} connecting to master server at {_serverAddress}");
		ConnectToPhoton(User.TGPID, _serverAddress, applicationID, appVersion);
	}

	private void JoinRandomRoom()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		if (!_isJoiningGameServer)
		{
			if (this.OpJoinRandomRoom(new Hashtable(), maxPlayerPerRoom, (string[])null))
			{
				Console.Log("Joining random room for " + applicationID);
			}
			_isJoiningGameServer = true;
		}
	}

	private void ConnectToPhoton(string userName, string serverAddress, string appId, string appVersion)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Expected O, but got Unknown
		if (IsConnecting)
		{
			Console.LogWarning("ConnectToPhoton called while already IsConnecting");
			return;
		}
		this.add_OnStateChangeAction((Action<ClientState>)OnStatusChanged);
		this.set_AutoJoinLobby(false);
		bool flag = false;
		try
		{
			AuthenticationValues val = new AuthenticationValues();
			val.set_AuthType(0);
			val.set_AuthGetParameters(GetAuthenticationParameters());
			val.set_UserId(userName);
			if (this.Connect(serverAddress, appId, appVersion, userName, val))
			{
				_reconnectTask = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)CreateNewTaskWithTimer);
				mustDisconnect = false;
				_isConnecting = true;
				base.loadBalancingPeer.DebugOut = 1;
				Console.Log("Connecting to " + applicationID);
				CreateServiceGameObject();
				if (_whileLoadingThreadRunner == null)
				{
					_whileLoadingThreadRunner = new MultiThreadRunner("PhotonClientThread", true);
					TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumerator(AcknowledgeIncomingCommands_Coroutine())
						.SetScheduler(_whileLoadingThreadRunner)
						.Start((Action<PausableTaskException>)null, (Action)null);
				}
				base.loadBalancingPeer.TimePingInterval = pingInterval;
			}
			else
			{
				if (!Disconnect())
				{
					flag = true;
				}
				Console.LogError("Unknown error while trying to connect");
			}
		}
		catch (Exception arg)
		{
			if (!Disconnect())
			{
				flag = true;
			}
			Console.LogError("Unknown exception while trying to connect " + arg);
		}
		if (!flag)
		{
			return;
		}
		throw new PhotonConnectionException("Connection Failed");
	}

	public virtual bool Disconnect()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Invalid comparison between Unknown and I4
		this.Disconnect();
		this.remove_OnStateChangeAction((Action<ClientState>)OnStatusChanged);
		if ((int)this.get_State() != 14)
		{
			return true;
		}
		return false;
	}

	private Exception GetConnectionFailureException()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (_disconnectException != null)
		{
			return _disconnectException;
		}
		if ((int)this.get_DisconnectedCause() != 0)
		{
			return new PhotonConnectionException($"Failed to connect to server {applicationID}: {this.get_DisconnectedCause()}");
		}
		if (!mustDisconnect)
		{
			return new PhotonConnectionException($"Failed to connect to server {applicationID}");
		}
		return null;
	}

	private bool CheckIfDisconnectedWhileConnecting()
	{
		if (_isConnecting && _connectionAttempts < connectAttempts)
		{
			_connectionAttempts++;
			Console.LogWarning("Reconnection attempt number " + _connectionAttempts);
			_isConnecting = false;
			_isJoiningGameServer = false;
			_reconnectTask.Start((Action<PausableTaskException>)null, (Action)null);
			return true;
		}
		_isConnecting = false;
		_isJoiningGameServer = false;
		return false;
	}

	private void OnStatusChanged(ClientState clientState)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected I4, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Invalid comparison between Unknown and I4
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Invalid comparison between Unknown and I4
		if (clientState == _lastState)
		{
			return;
		}
		_lastState = clientState;
		Console.Log(base.GetType().Name + " New client state: " + clientState);
		switch (clientState - 5)
		{
		case 3:
			OnConnectedAndAuthenticating();
			return;
		case 0:
			Console.Log($"{applicationID} connecting to slave server at {this.get_GameServerAddress()}");
			return;
		}
		if ((int)clientState != 14)
		{
			if ((int)clientState == 15)
			{
				OnConnectedToMaster();
			}
		}
		else
		{
			OnDisconnected();
		}
	}

	public override void OnStatusChanged(StatusCode statusCode)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Console.Log(base.GetType().Name + " New server state: " + statusCode);
		this.OnStatusChanged(statusCode);
	}

	private void ProcessPhotonStandardCodes(OperationResponse operationResponse)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		if (operationResponse.ReturnCode == 0)
		{
			return;
		}
		if (operationResponse.ReturnCode == 32760 || operationResponse.ReturnCode == 32765)
		{
			Console.Log("Joining random room failed - creating new room");
			RoomOptions val = new RoomOptions();
			val.MaxPlayers = maxPlayerPerRoom;
			val.EmptyRoomTtl = emptyRoomTtl;
			this.OpCreateRoom((string)null, val, null, (string[])null);
		}
		else
		{
			if (operationResponse.OperationCode == 226)
			{
				Console.LogError($"Photon client {applicationID} failed to join game. Reason: {operationResponse.ReturnCode}");
				Disconnect();
			}
			Console.LogException((Exception)new UnmanagedPhotonErrorException($"Photon client {applicationID} something went wrong. Reason: {operationResponse.ReturnCode} Code: {operationResponse.OperationCode}"));
		}
	}

	private IEnumerator ServiceConnection_Coroutine()
	{
		while (true)
		{
			if ((DateTime.UtcNow - _lastService).TotalMilliseconds > (double)serviceConnectionInterval)
			{
				try
				{
					this.Service();
					lock (_lockDateTime)
					{
						_lastService = DateTime.UtcNow;
					}
				}
				catch (DuplicateLoginException)
				{
					throw;
				}
				catch (PhotonConnectionException)
				{
					throw;
				}
				catch (Exception ex3)
				{
					Console.LogError(ex3.ToString());
				}
			}
			yield return null;
		}
	}

	private IEnumerator AcknowledgeIncomingCommands_Coroutine()
	{
		while (true)
		{
			DateTime lastService;
			lock (_lockDateTime)
			{
				lastService = _lastService;
			}
			if ((DateTime.UtcNow - lastService).TotalMilliseconds > (double)maxServiceDelay)
			{
				base.loadBalancingPeer.SendAcksOnly();
				lock (_lockDateTime)
				{
					_lastService = DateTime.UtcNow;
				}
			}
			Thread.Sleep(maxServiceDelay);
			yield return null;
		}
	}

	private void CreateServiceGameObject()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		if (!(_photonConnectionBehaviour != null) || !(_photonConnectionBehaviour.get_gameObject() != null))
		{
			GameObject val = new GameObject("PhotonServiceTaskRunner (" + base.GetType() + ")");
			Object.DontDestroyOnLoad(val);
			_photonConnectionBehaviour = val.AddComponent<PhotonConnectionMonobehavior>();
			_photonConnectionBehaviour.OnDestroyActions = ForceDisconnect;
			_photonConnectionBehaviour.StartCoroutine(ServiceConnection_Coroutine());
		}
	}

	private void DestroyPhotonGameObject()
	{
		if (_photonConnectionBehaviour != null)
		{
			Object.Destroy(_photonConnectionBehaviour.get_gameObject());
			_photonConnectionBehaviour = null;
		}
	}
}
