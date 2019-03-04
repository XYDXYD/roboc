using UnityEngine;

namespace Mothership
{
	internal class NotEnoughMoneyDialog : MonoBehaviour
	{
		public UILabel costLabel;

		public UILabel cosmeticCreditsCostLabel;

		public GameObject okButton;

		public UILabel bodyLabel;

		public UILabel costTitleLabel;

		public UILabel cosmeticCreditsCostTitleLabel;

		public NotEnoughMoneyDialog()
			: this()
		{
		}

		public void Show(string robitsTitleStr, string cosmeticCreditsTitleStr, string bodyStr, long robitsCost, long cosmeticCreditsCost)
		{
			cosmeticCreditsCostTitleLabel.set_text(cosmeticCreditsTitleStr);
			cosmeticCreditsCostLabel.set_text($"{cosmeticCreditsCost:#,#}");
			Show(robitsTitleStr, bodyStr, robitsCost);
		}

		public void Show(string robitsTitleStr, string bodyStr, long robitsCost)
		{
			costTitleLabel.set_text(robitsTitleStr);
			bodyLabel.set_text(bodyStr);
			costLabel.set_text($"{robitsCost:#,#}");
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}
	}
}
