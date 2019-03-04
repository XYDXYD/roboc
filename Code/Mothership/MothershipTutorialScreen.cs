using Svelto.IoC;

namespace Mothership
{
	internal sealed class MothershipTutorialScreen : TutorialScreenBase, IInitialize
	{
		private bool _isActive;

		[Inject]
		internal ITutorialController tutorialController
		{
			private get;
			set;
		}

		[Inject]
		internal PremiumMembership premiumMembership
		{
			private get;
			set;
		}

		public override GuiScreens QueryScreenType()
		{
			return GuiScreens.MothershipTutorialScreen;
		}

		void IInitialize.OnDependenciesInjected()
		{
			tutorialController.SetDisplay(this);
		}

		public void Start()
		{
			this.get_gameObject().SetActive(false);
		}

		public bool IsActive()
		{
			return _isActive;
		}

		public void Show()
		{
			this.get_gameObject().SetActive(true);
			ShowAnimations();
			_isActive = true;
		}

		private void ShowAnimations()
		{
			ShowScreenBase();
			this.get_gameObject().SetActive(true);
		}

		public void HideScreen()
		{
			HideScreenBase();
			this.get_gameObject().SetActive(false);
			_isActive = false;
		}
	}
}
