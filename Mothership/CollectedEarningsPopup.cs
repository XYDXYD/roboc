using UnityEngine;

namespace Mothership
{
	internal class CollectedEarningsPopup : MonoBehaviour
	{
		public UILabel buyCountLabel;

		public UILabel earningsLabel;

		public CollectedEarningsPopup()
			: this()
		{
		}

		public void Show(int buyCount, int earnings)
		{
			buyCountLabel.set_text(buyCount.ToString("N0"));
			earningsLabel.set_text(earnings.ToString("N0"));
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}
	}
}
