using ExitGames.Client.Photon;
using Services.Photon;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using WebServices;

namespace Services.Web.Photon
{
	internal abstract class WebServicesRequest : PhotonRequest
	{
		protected override short UnexpectedErrorCode => 9;

		protected override string ServerName => ServicesServerNames.WebServices;

		protected override byte GuidParameterCode => 0;

		public WebServicesRequest(string strErrorTitleKey, string strErrorBodyKey, int autoAttempts)
			: base(strErrorTitleKey, strErrorBodyKey, autoAttempts)
		{
			_serviceBehaviour.SetAlternativeBehaviour(delegate
			{
				Application.Quit();
			}, "strQuit");
		}

		public override void Execute()
		{
			PhotonWebServicesUtility.Instance.QueryWebServicesService(new PhotonRequestContainer(this));
		}

		public override void OnOpResponse(OperationResponse response)
		{
			if (response.ReturnCode == 125)
			{
				throw new QuitDueToMaintenanceException(response.DebugMessage);
			}
			base.OnOpResponse(response);
		}
	}
	internal abstract class WebServicesRequest<TData> : PhotonRequest<TData>
	{
		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache0;

		protected override short UnexpectedErrorCode => 9;

		protected override string ServerName => ServicesServerNames.WebServices;

		protected override byte GuidParameterCode => 0;

		public WebServicesRequest(string strErrorTitleKey, string strErrorBodyKey, int autoAttempts)
			: base(strErrorTitleKey, strErrorBodyKey, autoAttempts)
		{
			_serviceBehaviour.SetAlternativeBehaviour((Action)Application.Quit, "strQuit");
		}

		public override void Execute()
		{
			PhotonWebServicesUtility.Instance.QueryWebServicesService(new PhotonRequestContainer(this));
		}

		public override void OnOpResponse(OperationResponse response)
		{
			if (response.ReturnCode == 125)
			{
				throw new QuitDueToMaintenanceException(response.DebugMessage);
			}
			base.OnOpResponse(response);
		}
	}
}
