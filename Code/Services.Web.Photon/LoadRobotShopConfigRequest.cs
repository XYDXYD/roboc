using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadRobotShopConfigRequest : WebServicesRequest<LoadRobotShopConfigResponse>, ILoadRobotShopConfigRequest, ITask, IStaticDataService, IServiceRequest, IAnswerOnComplete<LoadRobotShopConfigResponse>, IAbstractTask
	{
		protected override byte OperationCode => 92;

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

		public LoadRobotShopConfigRequest()
			: base("strGenericError", "strGenericErrorQuit", 0)
		{
		}

		public override void Execute()
		{
			if (CacheDTO.robotShopConfig != null)
			{
				if (base.answer != null && base.answer.succeed != null)
				{
					base.answer.succeed(new LoadRobotShopConfigResponse(CacheDTO.robotShopConfig));
				}
				TaskDone();
			}
			else
			{
				base.Execute();
			}
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override LoadRobotShopConfigResponse ProcessResponse(OperationResponse response)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Expected O, but got Unknown
			Hashtable val = response.get_Item((byte)110);
			int[] robotCpuRanges = (int[])val.get_Item((object)"robotShopPriceRanges");
			float submissionMultiplier = (float)val.get_Item((object)"submissionMultiplier");
			float earningMultiplier = (float)val.get_Item((object)"earningsMultiplier");
			CacheDTO.robotShopConfig = new LoadRobotShopConfigResponse(robotCpuRanges, submissionMultiplier, earningMultiplier);
			TaskDone();
			return new LoadRobotShopConfigResponse(CacheDTO.robotShopConfig);
		}

		private void TaskDone()
		{
			isDone = true;
			progress = 1f;
		}

		public IAbstractTask OnComplete(Action<bool> action)
		{
			return null;
		}

		public Dictionary<byte, string> GetJsonParameterCode()
		{
			Dictionary<byte, string> dictionary = new Dictionary<byte, string>();
			dictionary.Add(110, WebServicesParameterCode.PurchaseCrfRobotData.ToString());
			return dictionary;
		}
	}
}
