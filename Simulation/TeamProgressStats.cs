namespace Simulation
{
	internal struct TeamProgressStats
	{
		public float currentProgress;

		public float maxProgress;

		public TeamProgressStats(float _currentProgress, float _maxProgress)
		{
			currentProgress = _currentProgress;
			maxProgress = _maxProgress;
		}
	}
}
