namespace Services.Web.Photon
{
	internal struct RoboPassPreviewItemDisplayData
	{
		public string SpriteName;

		public bool SpriteFullSize;

		public string Name;

		public string Category;

		public RoboPassPreviewItemDisplayData(string spriteName_, bool spriteFullSize_, string category_, string name_)
		{
			SpriteName = spriteName_;
			SpriteFullSize = spriteFullSize_;
			Category = category_;
			Name = name_;
		}
	}
}
