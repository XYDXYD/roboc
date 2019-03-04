using System;

namespace PlayMaker
{
	public class CheckCurrentSelectedCategoryRequest : PlayMakerDataRequestBase<PlaymakerRequestEmptyInputParameters, CheckCurrentSelectedCategoryRequestReturn>
	{
		public CheckCurrentSelectedCategoryRequest(Action<CheckCurrentSelectedCategoryRequestReturn> resultCallback)
			: base((PlaymakerRequestEmptyInputParameters)null, resultCallback)
		{
		}
	}
}
