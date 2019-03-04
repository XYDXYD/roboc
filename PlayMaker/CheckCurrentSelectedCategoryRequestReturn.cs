namespace PlayMaker
{
	public class CheckCurrentSelectedCategoryRequestReturn : IPlaymakerRequestReturnResults
	{
		public CubeCategory cubeCategorySelected;

		public CheckCurrentSelectedCategoryRequestReturn()
		{
			cubeCategorySelected = CubeCategory.None;
		}

		public CheckCurrentSelectedCategoryRequestReturn(CubeCategory category)
		{
			cubeCategorySelected = category;
		}

		public void SetDefaultReturnResults()
		{
			cubeCategorySelected = CubeCategory.None;
		}
	}
}
