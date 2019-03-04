using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal class HUDBuildModeHintsView : MonoBehaviour, IInitialize
	{
		[SerializeField]
		public UIWidget hintUIWidget;

		[Inject]
		private HUDBuildModeHintsPresenter presenter
		{
			get;
			set;
		}

		public HUDBuildModeHintsView()
			: this()
		{
		}

		public void SetEnabled(bool setting)
		{
			if (hintUIWidget.get_gameObject().get_activeSelf() != setting)
			{
				hintUIWidget.get_gameObject().SetActive(setting);
			}
		}

		public void OnDependenciesInjected()
		{
			presenter.SetView(this);
		}
	}
}
