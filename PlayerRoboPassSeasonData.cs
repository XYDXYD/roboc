using System.Collections;

internal class PlayerRoboPassSeasonData
{
	public int deltaXpToShow
	{
		get;
		private set;
	}

	public int currentGradeIndex
	{
		get;
		private set;
	}

	public bool hasDeluxe
	{
		get;
		private set;
	}

	public float progressInGrade
	{
		get;
		private set;
	}

	public int xpFromSeasonStart
	{
		get;
		private set;
	}

	public PlayerRoboPassSeasonData()
	{
		deltaXpToShow = 0;
		currentGradeIndex = 0;
		hasDeluxe = false;
		progressInGrade = 0f;
		xpFromSeasonStart = 0;
	}

	public PlayerRoboPassSeasonData(IDictionary data)
	{
		deltaXpToShow = Decode.Get<int>(data, "deltaXpToShow");
		currentGradeIndex = Decode.Get<int>(data, "grade");
		hasDeluxe = Decode.Get<bool>(data, "hasDeluxe");
		progressInGrade = Decode.Get<float>(data, "progressInGrade");
		xpFromSeasonStart = Decode.Get<int>(data, "xpFromSeasonStart");
	}
}
