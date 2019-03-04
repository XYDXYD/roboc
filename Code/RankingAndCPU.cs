internal struct RankingAndCPU
{
	public readonly int Ranking;

	public readonly int TotalCPU;

	public readonly int TotalCosmeticCPU;

	public RankingAndCPU(int ranking, int totalCPU, int totalCosmeticCPU)
	{
		Ranking = ranking;
		TotalCPU = totalCPU;
		TotalCosmeticCPU = totalCosmeticCPU;
	}
}
