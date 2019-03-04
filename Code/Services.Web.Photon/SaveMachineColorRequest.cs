using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class SaveMachineColorRequest : WebServicesRequest, ISaveMachineColorRequest, ITask, IServiceRequest<SaveMachineColorDependency>, IAnswerOnComplete, IServiceRequest, IAbstractTask
	{
		private uint _garageSlot;

		private byte[] _colorMap;

		private Action<bool> _onComplete;

		protected override byte OperationCode => 32;

		public bool isDone
		{
			get;
			private set;
		}

		public float progress
		{
			get;
			private set;
		}

		public SaveMachineColorRequest()
			: base("strRobocloudError", "strUnableToSaveMachineColor", 0)
		{
		}

		public void Inject(SaveMachineColorDependency dependency)
		{
			_garageSlot = dependency.garageSlot;
			_colorMap = dependency.colorMap;
		}

		public IAbstractTask OnComplete(Action<bool> action)
		{
			_onComplete = (Action<bool>)Delegate.Combine(_onComplete, action);
			return this;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[31] = Convert.ToInt32(_garageSlot);
			dictionary[33] = _colorMap;
			val.Parameters = dictionary;
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
			CacheDTO.garageSlots[_garageSlot].colorMap = _colorMap;
			progress = 1f;
			if (_onComplete != null)
			{
				_onComplete(obj: true);
			}
			isDone = true;
		}
	}
}
