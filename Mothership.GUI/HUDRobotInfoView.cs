using Svelto.IoC;
using UnityEngine;

namespace Mothership.GUI
{
	internal class HUDRobotInfoView : MonoBehaviour, IInitialize
	{
		public enum StyleVersion
		{
			Garage,
			EditMode,
			GarageCustomGame,
			EditCustomGame,
			AllOtherLocations
		}

		[SerializeField]
		private UIWidget GarageStyleWidget;

		[SerializeField]
		private UIWidget EditStyleWidget;

		[SerializeField]
		private UIWidget GarageCustomGameStyleWidget;

		[SerializeField]
		private UIWidget EditCustomGameStyleWidget;

		private UIWidget ThisUIWidget;

		[Inject]
		internal HUDRobotInfoPresenter presenter
		{
			private get;
			set;
		}

		public HUDRobotInfoView()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			presenter.RegisterView(this);
			ThisUIWidget = this.GetComponent<UIWidget>();
		}

		internal void Show(StyleVersion style)
		{
			UpdateWidgetAnchors(style);
			this.get_gameObject().SetActive(true);
		}

		internal void UpdateWidgetAnchors(StyleVersion style)
		{
			switch (style)
			{
			case StyleVersion.EditMode:
				UIAnchorUtility.CopyAnchors(EditStyleWidget, ThisUIWidget, 12);
				break;
			case StyleVersion.Garage:
				UIAnchorUtility.CopyAnchors(GarageStyleWidget, ThisUIWidget, 12);
				break;
			case StyleVersion.EditCustomGame:
				UIAnchorUtility.CopyAnchors(EditCustomGameStyleWidget, ThisUIWidget, 12);
				break;
			case StyleVersion.GarageCustomGame:
				UIAnchorUtility.CopyAnchors(GarageCustomGameStyleWidget, ThisUIWidget, 12);
				break;
			default:
				Hide();
				break;
			}
		}

		internal void Hide()
		{
			this.get_gameObject().SetActive(false);
		}
	}
}
