using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal sealed class RobotShopMainView : MonoBehaviour, IInitialize
	{
		public RobotShopShowroomView showroomView;

		public RobotShopModelView modelView;

		[Inject]
		internal RobotShopController robotShop
		{
			private get;
			set;
		}

		public bool IsActive => this.get_gameObject().get_activeSelf();

		public RobotShopMainView()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			if (robotShop != null)
			{
				robotShop.SetupMainView(this);
			}
		}

		private void Start()
		{
			Hide();
		}

		public void Show()
		{
			SwitchToShowroomView();
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public void SwitchToModelView()
		{
			showroomView.Hide();
			modelView.Show();
		}

		public void SwitchToShowroomView()
		{
			modelView.Hide();
			showroomView.Show();
		}
	}
}
