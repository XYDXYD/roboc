namespace Mothership.GUI
{
	internal interface ITierProgressionWidgetComponent
	{
		int rank
		{
			set;
		}

		string rankString
		{
			set;
		}

		float progressInRank
		{
			set;
		}

		string tierString
		{
			set;
		}
	}
}
