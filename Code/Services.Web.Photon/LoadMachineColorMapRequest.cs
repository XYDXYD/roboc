using Authentication;
using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class LoadMachineColorMapRequest : WebServicesRequest<byte[]>, ILoadMachineColorMapRequest, IServiceRequest<LoadMachineColorMapDependancy>, IAnswerOnComplete<byte[]>, IServiceRequest
	{
		private bool _forceRefresh;

		private string _userName;

		private uint _garageSlot;

		protected override byte OperationCode => 33;

		public LoadMachineColorMapRequest()
			: base("strRobocloudError", "strUnableToLoadMachineColorMap", 3)
		{
		}

		public void Inject(LoadMachineColorMapDependancy dependancy)
		{
			_userName = dependancy.userName;
			_garageSlot = dependancy.garageSlot;
		}

		public override void Execute()
		{
			if (_forceRefresh || User.Username != _userName || CacheDTO.garageSlots[_garageSlot].colorMap == null)
			{
				base.Execute();
			}
			else if (base.answer != null && base.answer.succeed != null)
			{
				base.answer.succeed(CacheDTO.garageSlots[_garageSlot].colorMap);
			}
		}

		protected override byte[] ProcessResponse(OperationResponse response)
		{
			if (User.Username == _userName)
			{
				CacheDTO.garageSlots[_garageSlot].colorMap = (byte[])response.Parameters[33];
			}
			return (byte[])response.Parameters[33];
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[30] = _userName;
			dictionary[31] = Convert.ToInt32(_garageSlot);
			val.Parameters = dictionary;
			return val;
		}
	}
}
