using UnityEngine;

namespace Mothership
{
	internal class RobotShopShowroomView : MonoBehaviour
	{
		public RobotShopShowroomView()
			: this()
		{
		}

		public void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}
	}
}
