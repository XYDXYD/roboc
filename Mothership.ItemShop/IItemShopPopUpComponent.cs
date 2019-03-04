using UnityEngine;

namespace Mothership.ItemShop
{
	public interface IItemShopPopUpComponent
	{
		UILabel titleLabel
		{
			get;
		}

		UILabel infoLabel
		{
			get;
		}

		UILabel rightButtLabel
		{
			get;
		}

		UILabel leftButtLabel
		{
			get;
		}

		UILabel centerButtLabel
		{
			get;
		}

		GameObject doubleButtonsContainer
		{
			get;
		}

		GameObject singleButtonContainer
		{
			get;
		}

		ItemShopPopUpType popupType
		{
			get;
			set;
		}
	}
}
