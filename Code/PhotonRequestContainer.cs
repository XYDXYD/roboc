using ExitGames.Client.Photon;
using Services.Photon;
using System;

internal class PhotonRequestContainer
{
	public const byte GUIDParameterCode = 0;

	protected PhotonClient _client;

	private IPhotonRequest _request;

	private string _guid;

	public string GUID => _guid;

	public string name => _request.GetType().Name;

	public PhotonRequestContainer(IPhotonRequest request)
	{
		string text = _guid = Guid.NewGuid().ToString();
		_request = request;
	}

	public void SetClient(PhotonClient client)
	{
		_client = client;
	}

	public void ClientDisconnected(bool isUnexpected, Exception managedException)
	{
		_request.OnClientDisconnected(isUnexpected, managedException);
	}

	public void SendRequest(object sender, object e)
	{
		SendRequest();
	}

	public void SendRequest()
	{
		OperationRequest operationRequest = _request.GetOperationRequest();
		operationRequest.Parameters.Add(0, _guid);
		if (!_client.SendOp(operationRequest, _request.isEncrypted))
		{
			_request.OnSendOperationFailed(new Exception("No connection to the server"));
		}
	}

	public void OnOperationResponse(OperationResponse operationResponse)
	{
		string b = (string)operationResponse.Parameters[0];
		if (_guid == b)
		{
			_request.OnOpResponse(operationResponse);
			_request = null;
		}
	}
}
