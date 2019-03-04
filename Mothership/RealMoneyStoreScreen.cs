using Svelto.ECS;
using System;
using UnityEngine;

namespace Mothership
{
	internal class RealMoneyStoreScreen : MonoBehaviour
	{
		public DispatchOnChange<bool> isShown;

		public GameObject PremiumStoreItemTemplate;

		public GameObject CosmeticCreditStoreItemTemplate;

		public GameObject RoboPassTemplate;

		public UIGrid ContainerPremiumItems;

		public UIGrid ContainerCosmeticCreditItems;

		public UIGrid ContainerRoboPass;

		public RealMoneyStoreScreen()
			: this()
		{
		}

		public void Initialize(int id)
		{
			isShown = new DispatchOnChange<bool>(id);
			isShown.NotifyOnValueSet((Action<int, bool>)OnShow);
		}

		private void OnShow(int id, bool show)
		{
			this.get_gameObject().SetActive(show);
		}
	}
}
