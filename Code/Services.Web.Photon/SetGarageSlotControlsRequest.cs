using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class SetGarageSlotControlsRequest : WebServicesRequest<ControlSettings>, ISetGarageSlotControlsRequest, IServiceRequest<GarageSlotControlsDependency>, IAnswerOnComplete<ControlSettings>, ITask, IServiceRequest, IAbstractTask
	{
		private GarageSlotControlsDependency _dependency;

		protected override byte OperationCode => 115;

		public bool isDone
		{
			get;
			private set;
		}

		public SetGarageSlotControlsRequest()
			: base("strRobocloudError", "strUnableSetGarageSlotControls", 0)
		{
		}

		public void Inject(GarageSlotControlsDependency dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[45] = Convert.ToInt32(_dependency.index);
			Dictionary<byte, object> parameters = val.Parameters;
			ControlSettings controlSetting = _dependency.controlSetting;
			parameters[59] = (int)controlSetting.controlType;
			val.Parameters[60] = _dependency.controlSetting.options();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override ControlSettings ProcessResponse(OperationResponse response)
		{
			CacheDTO.garageSlots[_dependency.index].controlSetting = _dependency.controlSetting;
			isDone = true;
			return _dependency.controlSetting;
		}
	}
}
