using System;

namespace PlayMaker
{
	public class CheckHasFilledAllCubeLocationsRequest : PlayMakerDataRequestBase<PlaymakerRequestEmptyInputParameters, CheckHasFilledAllCubeLocationsRequestReturn>
	{
		public CheckHasFilledAllCubeLocationsRequest(Action<CheckHasFilledAllCubeLocationsRequestReturn> resultCallback)
			: base((PlaymakerRequestEmptyInputParameters)null, resultCallback)
		{
		}
	}
}
