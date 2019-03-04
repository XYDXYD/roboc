using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal class BundleAwardDialog : MonoBehaviour, IInitialize
	{
		public UIButton Submit;

		public UILabel Bundles;

		[Inject]
		public BundleAwardController Controller
		{
			get;
			set;
		}

		public BundleAwardDialog()
			: this()
		{
		}

		public void Show()
		{
			this.set_enabled(true);
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.set_enabled(false);
			this.get_gameObject().SetActive(false);
		}

		public void OnDependenciesInjected()
		{
			Controller.SetDialog(this);
		}
	}
}
