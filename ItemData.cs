using System;
using System.Collections;

public class ItemData
{
	public string id
	{
		get;
		private set;
	}

	public RoboPassSeasonRewardCategory category
	{
		get;
		private set;
	}

	public int count
	{
		get;
		private set;
	}

	public ItemData(IDictionary data)
	{
		id = Decode.Get<string>(data, "id");
		count = Decode.Get<int>(data, "count");
		string text = Decode.Get<string>(data, "category");
		try
		{
			category = (RoboPassSeasonRewardCategory)Enum.Parse(typeof(RoboPassSeasonRewardCategory), text);
		}
		catch (Exception ex)
		{
			throw new Exception($"The category {text} is not valid. {ex.Message} {ex.StackTrace}");
		}
	}
}
