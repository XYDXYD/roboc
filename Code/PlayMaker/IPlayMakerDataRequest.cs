namespace PlayMaker
{
	public interface IPlayMakerDataRequest
	{
		void Execute();

		void AssignResults<T>(T resultsObj) where T : class, IPlaymakerRequestReturnResults;

		IPlaymakerRequestInputParameters GetInputParameters();

		T GetInputParameters<T>() where T : class, IPlaymakerRequestInputParameters;
	}
}
