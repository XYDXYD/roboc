namespace PlayMaker
{
	public class GetCurrentCubeSelectedPlaymakerRequestResult : IPlaymakerRequestReturnResults
	{
		public string currentCubeSelected;

		public GetCurrentCubeSelectedPlaymakerRequestResult()
		{
			currentCubeSelected = string.Empty;
		}

		public GetCurrentCubeSelectedPlaymakerRequestResult(string selectedCube)
		{
			currentCubeSelected = selectedCube;
		}

		public void SetDefaultReturnResults()
		{
			currentCubeSelected = string.Empty;
		}
	}
}
