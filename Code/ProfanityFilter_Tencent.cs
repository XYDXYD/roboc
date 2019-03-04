using rail;

internal sealed class ProfanityFilter_Tencent : ProfanityFilter
{
	public override string FilterString(string inputString)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		IRailFactory val = rail_api.RailFactory();
		IRailUtils val2 = val.RailUtils();
		RailDirtyWordsCheckResult val3 = new RailDirtyWordsCheckResult();
		val2.DirtyWordsFilter(inputString, true, val3);
		if ((int)val3.dirty_type != 0)
		{
			return val3.replace_string;
		}
		return inputString;
	}
}
