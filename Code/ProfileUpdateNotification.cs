using Svelto.IoC;

internal sealed class ProfileUpdateNotification
{
	private bool _notificationVisible;

	[Inject]
	public GenericInfoDisplay infoDisplay
	{
		private get;
		set;
	}

	public static bool removedObsoleteCubes
	{
		private get;
		set;
	}

	public static bool refundedObsoleteCubes
	{
		private get;
		set;
	}

	public static bool removedNotOwnedCubes
	{
		private get;
		set;
	}

	public static bool cubesHaveBeenReplaced
	{
		private get;
		set;
	}

	public static string specialRewardTitle
	{
		private get;
		set;
	}

	public static string specialRewardBody
	{
		private get;
		set;
	}

	public static int cratesRefundedTPAmount
	{
		private get;
		set;
	}

	public bool isReady => !_notificationVisible;

	public bool ShowPendingNotification()
	{
		if (ProfileUpdateNotification.removedObsoleteCubes || ProfileUpdateNotification.refundedObsoleteCubes)
		{
			_notificationVisible = true;
			bool refundedObsoleteCubes = ProfileUpdateNotification.refundedObsoleteCubes;
			bool removedObsoleteCubes = ProfileUpdateNotification.refundedObsoleteCubes = false;
			ProfileUpdateNotification.removedObsoleteCubes = removedObsoleteCubes;
			GenericErrorData data = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strObsoleteCubes"), StringTableBase<StringTable>.Instance.GetString((!refundedObsoleteCubes) ? "strObsoleteCubesError" : "strRefundObsoleteCubes"), StringTableBase<StringTable>.Instance.GetString("strOK"), delegate
			{
				_notificationVisible = false;
			});
			infoDisplay.ShowInfoDialogue(data);
			return true;
		}
		if (removedNotOwnedCubes)
		{
			_notificationVisible = true;
			removedNotOwnedCubes = false;
			GenericErrorData data2 = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strInvalidCubesFound"), StringTableBase<StringTable>.Instance.GetString("strInvalidCubesRemoved"), StringTableBase<StringTable>.Instance.GetString("strOK"), delegate
			{
				_notificationVisible = false;
			});
			infoDisplay.ShowInfoDialogue(data2);
			return true;
		}
		if (!string.IsNullOrEmpty(specialRewardBody))
		{
			_notificationVisible = true;
			specialRewardTitle = specialRewardTitle.Replace("&", "\n");
			specialRewardBody = specialRewardBody.Replace("&", "\n");
			GenericErrorData data3 = new GenericErrorData(specialRewardTitle, specialRewardBody, StringTableBase<StringTable>.Instance.GetString("strOK"), delegate
			{
				specialRewardTitle = string.Empty;
				specialRewardBody = string.Empty;
				_notificationVisible = false;
			});
			infoDisplay.ShowInfoDialogue(data3);
			return true;
		}
		if (cubesHaveBeenReplaced)
		{
			_notificationVisible = true;
			cubesHaveBeenReplaced = false;
			GenericErrorData data4 = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCubeReplacement"), StringTableBase<StringTable>.Instance.GetString("strUpdatedInventoryWithReplacements"), StringTableBase<StringTable>.Instance.GetString("strOK"), delegate
			{
				_notificationVisible = false;
			});
			infoDisplay.ShowInfoDialogue(data4);
			return true;
		}
		if (cratesRefundedTPAmount > 0)
		{
			_notificationVisible = true;
			GenericErrorData data5 = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCratesRefund"), StringTableBase<StringTable>.Instance.GetReplaceString("strRefundCratesWithTP", "[TP_AMOUNT]", cratesRefundedTPAmount.ToString()), StringTableBase<StringTable>.Instance.GetString("strOK"), delegate
			{
				_notificationVisible = false;
				cratesRefundedTPAmount = 0;
			});
			infoDisplay.ShowInfoDialogue(data5);
			return true;
		}
		return false;
	}
}
