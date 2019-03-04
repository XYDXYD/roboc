namespace PlayMaker
{
	public class CheckHasFilledAllCubeLocationsRequestReturn : IPlaymakerRequestReturnResults
	{
		public bool HasFilledAllLocations;

		public CheckHasFilledAllCubeLocationsRequestReturn()
		{
			HasFilledAllLocations = false;
		}

		public CheckHasFilledAllCubeLocationsRequestReturn(bool hasFilledAllLocations_)
		{
			HasFilledAllLocations = hasFilledAllLocations_;
		}

		public void SetDefaultReturnResults()
		{
			HasFilledAllLocations = false;
		}
	}
}
