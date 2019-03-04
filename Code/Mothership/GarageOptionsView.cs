using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal class GarageOptionsView : MonoBehaviour, IInitialize
	{
		public GameObject garageSlotOptions;

		public GameObject readOnlyGarageSlotOptionsGO;

		public UIButton copyButtonGO;

		[Inject]
		internal GarageOptionsPresenter garageOptionsPresenter
		{
			private get;
			set;
		}

		public GarageOptionsView()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			copyButtonGO.set_isEnabled(false);
			garageSlotOptions.SetActive(false);
			readOnlyGarageSlotOptionsGO.SetActive(false);
			garageOptionsPresenter.SetView(this);
		}

		public void DisplayGarageButtons()
		{
			garageSlotOptions.SetActive(true);
			readOnlyGarageSlotOptionsGO.SetActive(false);
		}

		public void DisplayReadOnlyGarageButtons()
		{
			readOnlyGarageSlotOptionsGO.SetActive(true);
			garageSlotOptions.SetActive(false);
		}

		public void EnableCopyButton(bool enable)
		{
			copyButtonGO.set_isEnabled(enable);
		}
	}
}
