using System;
using System.Collections;
using System.Collections.Generic;

internal class RoboPassSeasonData
{
	public DateTime endDateTimeUTC
	{
		get;
		private set;
	}

	public DateTime startDateTimeUTC
	{
		get;
		private set;
	}

	public RoboPassSeasonRewardData[][] gradesRewards
	{
		get;
		private set;
	}

	public int[] xpBetweenGrades
	{
		get;
		private set;
	}

	public string nameString
	{
		get;
		private set;
	}

	public RoboPassSeasonData(IDictionary data)
	{
		nameString = Decode.Get<string>(data, "NameString");
		string dateTimeString = Decode.Get<string>(data, "EndDateTimeUTC");
		string dateTimeString2 = Decode.Get<string>(data, "StartDateTimeUTC");
		Dictionary<string, object> dictionary = Decode.Get<Dictionary<string, object>>(data, "GradesRewards");
		int count = dictionary.Count;
		gradesRewards = new RoboPassSeasonRewardData[count][];
		xpBetweenGrades = new int[count];
		startDateTimeUTC = ParseDateTimeString(dateTimeString2);
		endDateTimeUTC = ParseDateTimeString(dateTimeString);
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			string key = item.Key;
			object value = item.Value;
			int num = int.Parse(key);
			IDictionary d = value as IDictionary;
			int num2 = Decode.Get<int>(d, "XpForNextGrade");
			object[] array = Decode.Get<object[]>(d, "Rewards");
			int num3 = array.Length;
			RoboPassSeasonRewardData[] array2 = new RoboPassSeasonRewardData[num3];
			for (int i = 0; i < num3; i++)
			{
				Dictionary<string, object> data2 = (Dictionary<string, object>)array[i];
				array2[i] = new RoboPassSeasonRewardData(data2);
			}
			gradesRewards[num] = array2;
			xpBetweenGrades[num] = num2;
		}
	}

	private static DateTime ParseDateTimeString(string dateTimeString)
	{
		try
		{
			string[] array = dateTimeString.Split('T');
			string text = array[0];
			string text2 = array[1];
			string[] array2 = text.Split('/');
			string[] array3 = text2.Split(':');
			return new DateTime(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), int.Parse(array3[0]), int.Parse(array3[1]), int.Parse(array3[2]));
		}
		catch (Exception)
		{
			throw new Exception("An error occurred in the RoboPassSeason json file while parsing this date: " + dateTimeString + ". The right format is yyyy/mm/ddThh:mm:ss");
		}
	}
}
