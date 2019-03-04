public static class ButtonMappingUsedChecker
{
	public static bool IsMappingUsed(string action, bool isShopAvailable)
	{
		if (!IsMappingUsedForShop(action, isShopAvailable))
		{
			return false;
		}
		return true;
	}

	private static bool IsMappingUsedForShop(string action, bool isShopAvailable)
	{
		if (!isShopAvailable && action == "Premium")
		{
			return false;
		}
		return true;
	}
}
