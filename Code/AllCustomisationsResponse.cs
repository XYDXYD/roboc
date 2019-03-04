using System.Collections.Generic;

public class AllCustomisationsResponse
{
	public string[] OwnedBaySkinCustomisations
	{
		get;
		private set;
	}

	public string[] OwnedSpawnFXCustomisations
	{
		get;
		private set;
	}

	public string[] OwnedDeathFXCustomisations
	{
		get;
		private set;
	}

	public List<CustomisationsEntry> AllSkinCustomisations
	{
		get;
		private set;
	}

	public List<CustomisationsEntry> AllSpawnCustomisations
	{
		get;
		private set;
	}

	public List<CustomisationsEntry> AllDeathCustomisations
	{
		get;
		private set;
	}

	public AllCustomisationsResponse(string[] OwnedBaySkinCustomisations_, string[] OwnedSpawnFXCustomisations_, string[] OwnedDeathFXCustomisations_, List<CustomisationsEntry> allSkinCustomisations_, List<CustomisationsEntry> allSpawnCustomisations_, List<CustomisationsEntry> allDeathCustomisations_)
	{
		OwnedBaySkinCustomisations = OwnedBaySkinCustomisations_;
		OwnedSpawnFXCustomisations = OwnedSpawnFXCustomisations_;
		OwnedDeathFXCustomisations = OwnedDeathFXCustomisations_;
		AllSkinCustomisations = allSkinCustomisations_;
		AllSpawnCustomisations = allSpawnCustomisations_;
		AllDeathCustomisations = allDeathCustomisations_;
	}
}
