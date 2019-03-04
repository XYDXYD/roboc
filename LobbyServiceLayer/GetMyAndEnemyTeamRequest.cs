using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace LobbyServiceLayer
{
	internal class GetMyAndEnemyTeamRequest : IGetMyAndEnemyTeamRequest, IServiceRequest, IAnswerOnComplete<int[]>
	{
		private IServiceAnswer<int[]> _answer;

		public void Execute()
		{
			int? localPlayerTeam = CacheDTO.LocalPlayerTeam;
			int value = localPlayerTeam.Value;
			int num = -1;
			HashSet<int> allTeams = CacheDTO.AllTeams;
			HashSet<int>.Enumerator enumerator = allTeams.GetEnumerator();
			while (enumerator.MoveNext() && num < 0)
			{
				if (value != enumerator.Current)
				{
					num = enumerator.Current;
				}
			}
			int[] obj = new int[2]
			{
				value,
				num
			};
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed(obj);
			}
		}

		public IServiceRequest SetAnswer(IServiceAnswer<int[]> answer)
		{
			_answer = answer;
			return this;
		}
	}
}
