namespace PlayMaker.Tutorial
{
	public class CheckActiveScreensRequestReturn : IPlaymakerRequestReturnResults
	{
		public bool InventoryScreenActive;

		public CheckActiveScreensRequestReturn()
		{
			InventoryScreenActive = false;
		}

		public CheckActiveScreensRequestReturn(bool inventoryScreen_)
		{
			InventoryScreenActive = inventoryScreen_;
		}

		public void SetDefaultReturnResults()
		{
			InventoryScreenActive = false;
		}
	}
}
