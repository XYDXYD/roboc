using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadLeagueBattleParametersRequest : WebServicesRequest<uint[]>, ILoadLeagueBattleParametersRequest, IServiceRequest, IAnswerOnComplete<uint[]>, ITask, IAbstractTask
	{
		private uint _levelRequired;

		private uint _cpuRequired;

		private bool _isDone;

		protected override byte OperationCode => 57;

		public bool isDone => _isDone;

		public LoadLeagueBattleParametersRequest()
			: base("strGenericError", "strFetchNormalModeBattleParameters", 0)
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

		public override void Execute()
		{
			uint? levelRequiredForLeagueGame = CacheDTO.levelRequiredForLeagueGame;
			if (levelRequiredForLeagueGame.HasValue)
			{
				uint? cpuRequiredForLeagueGame = CacheDTO.cpuRequiredForLeagueGame;
				if (cpuRequiredForLeagueGame.HasValue)
				{
					_isDone = true;
					if (base.answer != null && base.answer.succeed != null)
					{
						uint[] obj = new uint[2]
						{
							CacheDTO.levelRequiredForLeagueGame.Value,
							CacheDTO.cpuRequiredForLeagueGame.Value
						};
						base.answer.succeed(obj);
					}
					return;
				}
			}
			base.Execute();
		}

		protected override uint[] ProcessResponse(OperationResponse response)
		{
			_isDone = true;
			Dictionary<string, object> dictionary = (Dictionary<string, object>)response.Parameters[1];
			_levelRequired = Convert.ToUInt32(dictionary["playerLevelRequired"]);
			_cpuRequired = Convert.ToUInt32(dictionary["minCpu"]);
			CacheDTO.levelRequiredForLeagueGame = _levelRequired;
			CacheDTO.cpuRequiredForLeagueGame = _cpuRequired;
			return new uint[2]
			{
				_levelRequired,
				_cpuRequired
			};
		}
	}
}
