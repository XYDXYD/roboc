using UnityEngine;

namespace Mothership
{
	internal class NotEnoughRobitsDialog : MonoBehaviour
	{
		[SerializeField]
		private UILabel robitsLabel;

		[SerializeField]
		private UILabel cosmeticCreditsLabel;

		[SerializeField]
		private GameObject okButton;

		private const string FMT = "{0:#,#}";

		public NotEnoughRobitsDialog()
			: this()
		{
		}

		public void Show(Wallet missingMoney)
		{
			long robitsBalance = missingMoney.RobitsBalance;
			if (robitsBalance > 0)
			{
				robitsLabel.set_text($"{robitsBalance:#,#}");
			}
			else
			{
				robitsLabel.set_text(robitsBalance.ToString());
			}
			long cosmeticCreditsBalance = missingMoney.CosmeticCreditsBalance;
			if (cosmeticCreditsBalance > 0)
			{
				cosmeticCreditsLabel.set_text($"{cosmeticCreditsBalance:#,#}");
			}
			else
			{
				cosmeticCreditsLabel.set_text(cosmeticCreditsBalance.ToString());
			}
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}
	}
}
