using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Utility;

namespace Services.Web.Photon
{
	internal abstract class WebServicesCachedRequest : WebServicesRequest
	{
		private static Dictionary<Type, OperationResponse> ResponseCache = new Dictionary<Type, OperationResponse>();

		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache0;

		public WebServicesCachedRequest(string strErrorTitleKey, string strErrorBodyKey, int autoAttempts)
			: base(strErrorTitleKey, strErrorBodyKey, autoAttempts)
		{
			_serviceBehaviour.SetAlternativeBehaviour((Action)Application.Quit, "strQuit");
		}

		public override void Execute()
		{
			if (ResponseCache.ContainsKey(GetType()))
			{
				base.OnOpResponse(ResponseCache[GetType()]);
			}
			else
			{
				PhotonWebServicesUtility.Instance.QueryWebServicesService(new PhotonRequestContainer(this));
			}
		}

		public override void OnOpResponse(OperationResponse response)
		{
			if (!ResponseCache.ContainsKey(GetType()))
			{
				ResponseCache.Add(GetType(), response);
			}
			else
			{
				Console.LogWarning(GetType() + " was triggered twice before the first completes");
			}
			base.OnOpResponse(response);
		}

		public void ClearCache()
		{
			ResponseCache.Remove(GetType());
		}
	}
	internal abstract class WebServicesCachedRequest<TData> : WebServicesRequest<TData>
	{
		private static Dictionary<Type, OperationResponse> ResponseCache = new Dictionary<Type, OperationResponse>();

		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache0;

		public WebServicesCachedRequest(string strErrorTitleKey, string strErrorBodyKey, int autoAttempts)
			: base(strErrorTitleKey, strErrorBodyKey, autoAttempts)
		{
			_serviceBehaviour.SetAlternativeBehaviour((Action)Application.Quit, "strQuit");
		}

		public override void Execute()
		{
			if (ResponseCache.ContainsKey(GetType()))
			{
				base.OnOpResponse(ResponseCache[GetType()]);
			}
			else
			{
				PhotonWebServicesUtility.Instance.QueryWebServicesService(new PhotonRequestContainer(this));
			}
		}

		public override void OnOpResponse(OperationResponse response)
		{
			if (!ResponseCache.ContainsKey(GetType()))
			{
				ResponseCache.Add(GetType(), response);
			}
			else
			{
				Console.LogWarning(GetType() + " was triggered twice before the first completes");
			}
			base.OnOpResponse(response);
		}

		public void ClearCache()
		{
			ResponseCache.Remove(GetType());
		}
	}
}
