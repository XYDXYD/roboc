using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class SaveRobotRequest : WebServicesRequest<SaveMachineResult>, ISaveMachineRequest, IServiceRequest<SaveMachineDependency>, IAnswerOnComplete<SaveMachineResult>, IServiceRequest
	{
		private SaveMachineDependency _dependency;

		private int _oldGarageSlot;

		protected override byte OperationCode => 41;

		public SaveRobotRequest()
			: base("strRobocloudError", "strUnableSaveRobot", 0)
		{
		}

		public void Inject(SaveMachineDependency dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Expected O, but got Unknown
			_oldGarageSlot = (int)CacheDTO.currentGarageSlot[0];
			string value = CacheDTO.garageSlots[(uint)_oldGarageSlot].uniqueId.ToString();
			byte[] compresesdRobotData = _dependency.model.GetCompresesdRobotData();
			byte[] compressedRobotColorData = _dependency.model.GetCompressedRobotColorData();
			int[] value2 = _dependency.weaponOrder.Serialise();
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[54] = value;
			val.Parameters[45] = _oldGarageSlot;
			val.Parameters[46] = compresesdRobotData;
			val.Parameters[33] = compressedRobotColorData;
			val.Parameters[52] = value2;
			val.OperationCode = OperationCode;
			return val;
		}

		protected override SaveMachineResult ProcessResponse(OperationResponse operationResponse)
		{
			CacheDTO.garageSlots[(uint)_oldGarageSlot].machineModel = new MachineModel(_dependency.model);
			int e = Convert.ToInt32(operationResponse.Parameters[47]);
			return new SaveMachineResult(e, (uint)_oldGarageSlot);
		}
	}
}
