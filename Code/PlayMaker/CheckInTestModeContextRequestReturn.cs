namespace PlayMaker
{
	public class CheckInTestModeContextRequestReturn : IPlaymakerRequestReturnResults
	{
		public bool isInTestContext;

		public CheckInTestModeContextRequestReturn()
		{
			isInTestContext = false;
		}

		public CheckInTestModeContextRequestReturn(bool param_)
		{
			isInTestContext = param_;
		}

		public void SetDefaultReturnResults()
		{
			isInTestContext = false;
		}
	}
}
