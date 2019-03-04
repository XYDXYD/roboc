using Svelto.ECS;
using System;
using UnityEngine;

namespace Mothership
{
	internal class BuyPremiumAfterBattleScreen : MonoBehaviour, IBuyPremiumAfterBattleButtonComponents
	{
		public DispatchOnChange<bool> isShown;

		public GameObject PremiumStoreItemTemplate;

		public UIGrid ContainerPremiumItems;

		[SerializeField]
		private UIButton gobackButton;

		public DispatchOnChange<bool> goBackButtonPressed
		{
			get;
			private set;
		}

		public BuyPremiumAfterBattleScreen()
			: this()
		{
		}

		public unsafe void Initialize(int id)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Expected O, but got Unknown
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			isShown = new DispatchOnChange<bool>(id);
			isShown.NotifyOnValueSet((Action<int, bool>)OnShow);
			goBackButtonPressed = new DispatchOnChange<bool>(0);
			gobackButton.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
		}

		public void GoBackButtonClicked()
		{
			goBackButtonPressed.set_value(true);
			goBackButtonPressed.set_value(false);
		}

		private void OnShow(int id, bool show)
		{
			this.get_gameObject().SetActive(show);
		}
	}
}
