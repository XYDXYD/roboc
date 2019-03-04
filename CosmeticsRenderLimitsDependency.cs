using ExitGames.Client.Photon;
using System;

internal sealed class CosmeticsRenderLimitsDependency
{
	public readonly uint othersMaxNumberHoloAndTrails;

	public readonly uint othersMaxNumberHeadlamps;

	public readonly uint othersMaxCosmeticItemsWithParticleSystem;

	public CosmeticsRenderLimitsDependency(Hashtable data)
	{
		othersMaxNumberHoloAndTrails = Convert.ToUInt32(data.get_Item((object)"OthersMaxNumberHoloAndTrails"));
		othersMaxNumberHeadlamps = Convert.ToUInt32(data.get_Item((object)"OthersMaxNumberHeadlamps"));
		othersMaxCosmeticItemsWithParticleSystem = Convert.ToUInt32(data.get_Item((object)"OthersMaxCosmeticItemsWithParticleSystem"));
	}
}
