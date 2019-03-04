namespace Mothership
{
	public class PrebuiltRobotOption
	{
		public PrebuiltRobotType optionType;

		public string id;

		public string strKey;

		public PrebuiltRobotOption(string strKey_)
		{
			strKey = strKey_;
			optionType = PrebuiltRobotType.RobotClass;
		}

		public PrebuiltRobotOption(string id_, string strKey_)
		{
			id = id_;
			strKey = strKey_;
			optionType = PrebuiltRobotType.RobotCategory;
		}
	}
}
