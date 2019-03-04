using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Services.Photon
{
	internal abstract class PhotonRequest<TData> : PhotonRequestBase, IAnswerOnComplete<TData>
	{
		public IServiceAnswer<TData> answer => _answer as IServiceAnswer<TData>;

		public PhotonRequest(string strErrorTitleKey, string strErrorBodyKey, int autoAttempts)
			: base(strErrorTitleKey, strErrorBodyKey, autoAttempts)
		{
		}

		public IServiceRequest SetAnswer(IServiceAnswer<TData> answer)
		{
			_answer = answer;
			return this;
		}

		protected abstract TData ProcessResponse(OperationResponse response);

		protected override void SuccessfulResponse(OperationResponse response)
		{
			TData val = default(TData);
			try
			{
				val = ProcessResponse(response);
				OnSuccess();
			}
			catch (Exception exception)
			{
				OnFailed(exception);
				return;
			}
			if (answer != null && answer.succeed != null)
			{
				answer.succeed(val);
			}
		}
	}
	internal abstract class PhotonRequest : PhotonRequestBase, IAnswerOnComplete
	{
		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache0;

		public IServiceAnswer answer => _answer as IServiceAnswer;

		public PhotonRequest(string strErrorTitleKey, string strErrorBodyKey, int autoAttempts)
			: base(strErrorTitleKey, strErrorBodyKey, autoAttempts)
		{
		}

		public IServiceRequest SetAnswer(IServiceAnswer answer)
		{
			_answer = answer;
			return this;
		}

		protected virtual void ProcessResponse(OperationResponse response)
		{
		}

		protected override void SuccessfulResponse(OperationResponse response)
		{
			try
			{
				ProcessResponse(response);
				OnSuccess();
			}
			catch (Exception exception)
			{
				_serviceBehaviour.reasonID = ReasonCode.STR_REASON_CODE_DATA_CORRUPTED;
				_serviceBehaviour.SetAlternativeBehaviour((Action)Application.Quit, "strQuit");
				OnFailed(exception);
				return;
			}
			if (answer != null && answer.succeed != null)
			{
				answer.succeed();
			}
		}
	}
}
