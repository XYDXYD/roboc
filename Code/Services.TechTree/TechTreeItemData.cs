namespace Services.TechTree
{
	internal class TechTreeItemData
	{
		public uint mainCubeId;

		public uint techPointsCost;

		public int positionX;

		public int positionY;

		public bool isUnlocked;

		public bool isUnlockable;

		public string[] neighbours;

		public TechTreeItemData(uint mainCubeId, int positionX, int positionY, bool isUnlocked, bool isUnlockable, uint techPointsCost, string[] neighbours)
		{
			this.positionX = positionX;
			this.positionY = positionY;
			this.isUnlocked = isUnlocked;
			this.isUnlockable = isUnlockable;
			this.mainCubeId = mainCubeId;
			this.techPointsCost = techPointsCost;
			this.neighbours = neighbours;
		}
	}
}
