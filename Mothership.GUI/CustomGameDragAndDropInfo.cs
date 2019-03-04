namespace Mothership.GUI
{
	public struct CustomGameDragAndDropInfo
	{
		public readonly int IconIndex;

		public readonly bool IsTeamA;

		public CustomGameDragAndDropInfo(int iconIndex_, bool isTeamA_)
		{
			IconIndex = iconIndex_;
			IsTeamA = isTeamA_;
		}
	}
}
