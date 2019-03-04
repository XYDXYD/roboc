using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadTutorialEnemyMachineRequest : WebServicesRequest<TutorialEnemyMachineData>, ILoadTutorialEnemyMachineRequest, IServiceRequest, IAnswerOnComplete<TutorialEnemyMachineData>
	{
		private Action<bool> _onComplete;

		protected override byte OperationCode => 130;

		public LoadTutorialEnemyMachineRequest()
			: base("strRobocloudError", "strUnableLoadTutorialEnemyRobot", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.OperationCode = OperationCode;
			return val;
		}

		public void Inject()
		{
		}

		public override void Execute()
		{
			if (CacheDTO.tutorialEnemyMachineData == null)
			{
				base.Execute();
			}
			else
			{
				OnParseSuccess(base.answer);
			}
		}

		private void OnParseSuccess(IServiceAnswer<TutorialEnemyMachineData> answer)
		{
			if (answer != null && answer.succeed != null)
			{
				TutorialEnemyMachineData loadMachineResultFromCacheDTO = GetLoadMachineResultFromCacheDTO();
				answer.succeed(loadMachineResultFromCacheDTO);
			}
		}

		private TutorialEnemyMachineData GetLoadMachineResultFromCacheDTO()
		{
			return new TutorialEnemyMachineData(CacheDTO.tutorialEnemyMachineData);
		}

		protected override TutorialEnemyMachineData ProcessResponse(OperationResponse operationResponse)
		{
			byte[] data = (byte[])operationResponse.Parameters[49];
			byte[] colourData = (byte[])operationResponse.Parameters[33];
			CacheDTO.tutorialEnemyMachineData = new TutorialEnemyMachineData(new MachineModel(data), colourData);
			return GetLoadMachineResultFromCacheDTO();
		}
	}
}
