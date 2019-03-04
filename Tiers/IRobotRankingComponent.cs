using Svelto.ECS;

namespace Tiers
{
	internal interface IRobotRankingComponent
	{
		DispatchOnSet<RankingAndCPU> CubeRanking
		{
			get;
		}
	}
}
