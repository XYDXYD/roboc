internal struct CPUSettingsDependency
{
	public readonly uint maxRegularCpu;

	public readonly uint maxMegabotCpu;

	public readonly uint premiumForLifeCosmeticCPU;

	public readonly uint premiumCosmeticCPU;

	public readonly uint noPremiumCosmeticCPU;

	public readonly uint maxRegularHealth;

	public readonly uint maxMegabotHealth;

	public CPUSettingsDependency(uint maxRegularCPU_, uint maxMegabotCPU_, uint premiumForLifeCosmeticCPU_, uint premiumCosmeticCPU_, uint noPremiumCosmeticCPU_, uint maxRegularHealth_, uint maxMegabotHealth_)
	{
		maxRegularCpu = maxRegularCPU_;
		maxMegabotCpu = maxMegabotCPU_;
		premiumForLifeCosmeticCPU = premiumForLifeCosmeticCPU_;
		premiumCosmeticCPU = premiumCosmeticCPU_;
		noPremiumCosmeticCPU = noPremiumCosmeticCPU_;
		maxRegularHealth = maxRegularHealth_;
		maxMegabotHealth = maxMegabotHealth_;
	}
}
