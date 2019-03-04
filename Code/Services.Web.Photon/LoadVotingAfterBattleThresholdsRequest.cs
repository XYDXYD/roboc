using ExitGames.Client.Photon;
using Simulation.GUI;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class LoadVotingAfterBattleThresholdsRequest : WebServicesCachedRequest<Dictionary<VoteType, ThresholdData[]>>, ILoadVotingAfterBattleThresholdsRequest, IServiceRequest, IAnswerOnComplete<Dictionary<VoteType, ThresholdData[]>>, ITask, IAbstractTask
	{
		protected override byte OperationCode => 169;

		public bool isDone
		{
			get;
			private set;
		}

		public LoadVotingAfterBattleThresholdsRequest()
			: base("strRobocloudError", "strUnableLoadVotingAfterBattleThresholds", 3)
		{
			_serviceBehaviour.SetAlternativeBehaviour(delegate
			{
			}, "strCancel");
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			return val;
		}

		protected override Dictionary<VoteType, ThresholdData[]> ProcessResponse(OperationResponse response)
		{
			Dictionary<VoteType, ThresholdData[]> dictionary = new Dictionary<VoteType, ThresholdData[]>();
			Dictionary<string, object> data = (Dictionary<string, object>)response.Parameters[203];
			VoteType[] array = Enum.GetValues(typeof(VoteType)) as VoteType[];
			for (int i = 0; i < array.Length; i++)
			{
				dictionary.Add(array[i], DeserialiseByType(data, array[i]));
			}
			isDone = true;
			return dictionary;
		}

		private ThresholdData[] DeserialiseByType(Dictionary<string, object> data, VoteType type)
		{
			object[] array = (object[])data[type.ToString()];
			ThresholdData[] array2 = new ThresholdData[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)array[i];
				ThresholdData thresholdData = array2[i] = new ThresholdData(Convert.ToString(dictionary["name"]), Convert.ToString(dictionary["localisedName"]), Convert.ToString(dictionary["color"]), Convert.ToInt32(dictionary["votesRequired"]));
			}
			return array2;
		}
	}
}
