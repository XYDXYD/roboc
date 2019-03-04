using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Utility;

namespace Services.Photon
{
	internal abstract class PhotonRequestBase : IPhotonRequest, IAnswerOnFail, IServiceRequest
	{
		protected IServiceFailedAnswer _answer;

		protected ServiceBehaviour _serviceBehaviour;

		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache0;

		protected abstract byte OperationCode
		{
			get;
		}

		protected abstract byte GuidParameterCode
		{
			get;
		}

		protected abstract short UnexpectedErrorCode
		{
			get;
		}

		protected abstract string ServerName
		{
			get;
		}

		public virtual bool isEncrypted => false;

		protected PhotonRequestBase(string strErrorTitleKey, string strErrorBodyKey, int autoAttempts)
		{
			_serviceBehaviour = new ServiceBehaviour(strErrorTitleKey, strErrorBodyKey, autoAttempts)
			{
				MainBehaviour = Execute
			};
			_serviceBehaviour.SetAlternativeBehaviour((Action)Application.Quit, "strQuit");
		}

		public IServiceRequest SetAnswer(IServiceFailedAnswer answer)
		{
			_answer = answer;
			return this;
		}

		public virtual void OnOpResponse(OperationResponse response)
		{
			short returnCode = response.ReturnCode;
			if (returnCode == -2 || returnCode == -3)
			{
				throw new Exception(response.DebugMessage);
			}
			if (!response.Parameters.ContainsKey(GuidParameterCode))
			{
				throw new Exception("Operation response did not contain a guid");
			}
			if (response.ReturnCode != 0)
			{
				OnErrorCode(response);
			}
			else
			{
				SuccessfulResponse(response);
			}
		}

		public OperationRequest GetOperationRequest()
		{
			OperationRequest val = BuildOpRequest();
			val.OperationCode = OperationCode;
			if (val.Parameters == null)
			{
				val.Parameters = new Dictionary<byte, object>();
			}
			return val;
		}

		public void OnSendOperationFailed(Exception obj)
		{
			_serviceBehaviour.reasonID = ReasonCode.STR_REASON_CODE_IMPOSSIBLE_TO_SEND_MESSAGE;
			OnFailed(new Exception("Photon Request failed to send"));
		}

		public void OnClientDisconnected(bool isUnexpected, Exception managedException)
		{
			_serviceBehaviour.serverReason = "Could not connect to server";
			_serviceBehaviour.reasonID = ((!isUnexpected) ? ReasonCode.STR_REASON_CODE_CLIENT_HAS_BEEN_DISCONNECTED : ReasonCode.STR_REASON_CODE_IMPOSSIBLE_TO_CONNECT_TO_THE_SERVER);
			OnFailed(managedException ?? new Exception("Connection to server lost"));
		}

		public abstract void Execute();

		protected virtual void OnFailed(Exception exception)
		{
			if (_answer != null && _answer.failed != null)
			{
				_serviceBehaviour.Failed(exception, _answer, retry: true);
			}
			else
			{
				Console.LogError($"Photon request {GetType()} failed: {exception}");
			}
		}

		protected void OnErrorCode(OperationResponse response)
		{
			_serviceBehaviour.reasonID = ReasonCode.STR_REASON_CODE_SERVER_ERROR;
			_serviceBehaviour.errorCode = response.ReturnCode;
			_serviceBehaviour.guidParameterCode = string.Empty;
			if (!string.IsNullOrEmpty(response.DebugMessage))
			{
				string text = $"Server error received for {GetType().Name}: {response.DebugMessage}";
				if (response.ReturnCode == UnexpectedErrorCode)
				{
					_serviceBehaviour.guidParameterCode = response.DebugMessage;
					RemoteLogger.Error(text, "Server name: " + ServerName, null);
				}
				else if (response.ReturnCode > 0)
				{
					Console.Log(text);
				}
			}
			OnFailed(new Exception("Photon request raised an error code: operation code=" + OperationCode + "class=" + GetType().ToString()));
		}

		protected virtual void OnSuccess()
		{
		}

		protected abstract OperationRequest BuildOpRequest();

		protected abstract void SuccessfulResponse(OperationResponse response);
	}
}
