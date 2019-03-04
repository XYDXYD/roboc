using UnityEngine;

namespace Mothership
{
	internal class TooManyGarageSlotsDialog : MonoBehaviour
	{
		public UILabel Message;

		public TooManyGarageSlotsDialog()
			: this()
		{
		}

		public void Show(string errorStr)
		{
			Message.set_text(errorStr);
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}
	}
}
