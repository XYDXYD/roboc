namespace Services.Analytics
{
	internal class LogSettingsChangedDependency
	{
		public float? musicVolume
		{
			get;
			private set;
		}

		public float? sfxVolume
		{
			get;
			private set;
		}

		public float? speechVolume
		{
			get;
			private set;
		}

		public float? buildMouseSpeed
		{
			get;
			private set;
		}

		public float? fightMouseSpeed
		{
			get;
			private set;
		}

		public string language
		{
			get;
			private set;
		}

		public bool? buildHintsEnabled
		{
			get;
			private set;
		}

		public LogSettingsChangedDependency(float? musicVolume_, float? sfxVolume_, float? speechVolume_, float? buildMouseSpeed_, float? fightMouseSpeed_, string language_, bool? buildHintsEnabled_)
		{
			musicVolume = musicVolume_;
			sfxVolume = sfxVolume_;
			speechVolume = speechVolume_;
			buildMouseSpeed = buildMouseSpeed_;
			fightMouseSpeed = fightMouseSpeed_;
			language = language_;
			buildHintsEnabled = buildHintsEnabled_;
		}
	}
}
