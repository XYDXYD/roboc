using System.Globalization;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal sealed class CubeCellWidget : MonoBehaviour
	{
		public int sortingPosition;

		[SerializeField]
		private UILabel nameLabel;

		[SerializeField]
		private UILabel robitsCostLabel;

		[SerializeField]
		private UISprite robitsCostSprite;

		[SerializeField]
		private UILabel cosmeticCreditsCostLabel;

		[SerializeField]
		private UISprite cosmeticCreditsSprite;

		[SerializeField]
		private UISprite cubeSprite;

		[SerializeField]
		private UILabel cpuRating;

		[SerializeField]
		private GameObject cubeTutorialHighlightIndicator;

		[SerializeField]
		private GameObject shownWhenLocked;

		[SerializeField]
		private GameObject shownWhenUnLocked;

		[SerializeField]
		private GameObject mainOverlayButton;

		[SerializeField]
		private GameObject newCubeSpriteGobj;

		[SerializeField]
		private GameObject mirrorIsLockedGobj;

		public CubeTypeID type
		{
			get;
			set;
		}

		public string NameLabelText
		{
			set
			{
				nameLabel.set_text(value);
			}
		}

		public string CubeSpriteName
		{
			set
			{
				cubeSprite.set_spriteName(value);
			}
		}

		public GameObject MainOverlayButton => mainOverlayButton;

		public GameObject NewCubeSpriteGobj => newCubeSpriteGobj;

		public GameObject MirrorIsLockedGobj => mirrorIsLockedGobj;

		public CubeCellWidget()
			: this()
		{
		}

		public void Default()
		{
			newCubeSpriteGobj.SetActive(false);
			mirrorIsLockedGobj.SetActive(false);
			mainOverlayButton.SetActive(true);
		}

		public void SetCannotClick(bool cannotClick)
		{
			mainOverlayButton.SetActive(!cannotClick);
		}

		public void ToggleHighlighting(bool highlight)
		{
			cubeTutorialHighlightIndicator.SetActive(highlight);
		}

		public void ToggleLockedState(bool lockState)
		{
			shownWhenLocked.SetActive(lockState);
			if (!lockState)
			{
				robitsCostLabel.get_gameObject().SetActive(false);
				robitsCostSprite.get_gameObject().SetActive(false);
				cosmeticCreditsCostLabel.get_gameObject().SetActive(false);
				cosmeticCreditsSprite.get_gameObject().SetActive(false);
			}
		}

		public void SetCostToBuy(int cost, bool isCosmetic)
		{
			robitsCostLabel.get_gameObject().SetActive(!isCosmetic);
			robitsCostSprite.get_gameObject().SetActive(!isCosmetic);
			cosmeticCreditsCostLabel.get_gameObject().SetActive(isCosmetic);
			cosmeticCreditsSprite.get_gameObject().SetActive(isCosmetic);
			if (isCosmetic)
			{
				cosmeticCreditsCostLabel.set_text(cost.ToString("N0", CultureInfo.InvariantCulture));
			}
			else
			{
				robitsCostLabel.set_text(cost.ToString("N0", CultureInfo.InvariantCulture));
			}
		}

		public void SetCPURatingOfCube(int cpuCost)
		{
			cpuRating.set_text(cpuCost.ToString("N0", CultureInfo.InvariantCulture) + " " + StringTableBase<StringTable>.Instance.GetString("strCPU"));
		}

		public void SetCurrencyTextValue(CurrencyType currencyType, string value)
		{
			switch (currencyType)
			{
			case CurrencyType.Robits:
				robitsCostLabel.set_text(value);
				break;
			case CurrencyType.CosmeticCredits:
				SetCCTextLabelVisible(visible: false);
				cosmeticCreditsCostLabel.set_text(value);
				break;
			default:
				Console.LogError("CurrencyType '" + currencyType + "' is of unknown type!");
				break;
			}
		}

		public void SetCCTextLabelVisible(bool visible)
		{
			cosmeticCreditsCostLabel.get_gameObject().SetActive(visible);
			cosmeticCreditsSprite.get_gameObject().SetActive(visible);
		}

		public void SetLocked(bool locked)
		{
			shownWhenLocked.SetActive(locked);
			shownWhenUnLocked.SetActive(!locked);
		}

		private void Start()
		{
		}
	}
}
