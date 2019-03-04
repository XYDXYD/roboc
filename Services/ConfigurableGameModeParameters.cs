namespace Services
{
	public class ConfigurableGameModeParameters
	{
		public GameModeType GameMode
		{
			get;
			set;
		}

		public string[] ExcludedCubesList
		{
			get;
			set;
		}

		public string[] ExcludedCubeTypesList
		{
			get;
			set;
		}

		public int MinCPU
		{
			get;
			set;
		}

		public int MaxCPU
		{
			get;
			set;
		}

		public string NameString
		{
			get;
			set;
		}

		public string NameDesc
		{
			get;
			set;
		}

		public string[] RuleStrings
		{
			get;
			set;
		}

		public string[][] RuleParameters
		{
			get;
			set;
		}
	}
}
