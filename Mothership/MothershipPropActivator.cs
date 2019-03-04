using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal sealed class MothershipPropActivator : MonoBehaviour, IInitialize
	{
		[SerializeField]
		private GameObject EditModeShipPremium;

		[SerializeField]
		private GameObject EditModeShipNonPremium;

		[SerializeField]
		private GameObject EditModeShared;

		[SerializeField]
		private GameObject CRFViewShip;

		[SerializeField]
		private GameObject PreviewRobotShipPremium;

		[SerializeField]
		private GameObject PreviewRobotShipNonPremium;

		[SerializeField]
		private GameObject ThumbnailPreview;

		[SerializeField]
		private GameObject MegabotDecoration;

		[SerializeField]
		private GameObject NonMegabotDecoration;

		[SerializeField]
		private Transform FloorCentre;

		[SerializeField]
		private UILabel[] BayNumberLabels;

		[SerializeField]
		private UILabel[] RobotNameLabels;

		[Inject]
		internal IMothershipPropPresenter presenter
		{
			private get;
			set;
		}

		public MothershipPropActivator()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			presenter.SetView(this);
		}

		public void SetPropType(MothershipPropType propType)
		{
			EditModeShipNonPremium.SetActive(propType == MothershipPropType.PropTypeEditModeNonPremium);
			EditModeShipPremium.SetActive(propType == MothershipPropType.PropTypeEditModePremium);
			EditModeShared.SetActive(propType == MothershipPropType.PropTypeEditModePremium || propType == MothershipPropType.PropTypeEditModeGarageSkin || propType == MothershipPropType.PropTypeEditModeNonPremium);
			CRFViewShip.SetActive(propType == MothershipPropType.PropTypeCRF);
			PreviewRobotShipNonPremium.SetActive(propType == MothershipPropType.PropTypePreviewRobotsNonPremium);
			PreviewRobotShipPremium.SetActive(propType == MothershipPropType.PropTypePreviewRobotsPremium);
			ThumbnailPreview.SetActive(propType == MothershipPropType.ThumbnailGaragePreview);
			if (propType == MothershipPropType.PropTypeEditModeNonPremium || propType == MothershipPropType.PropTypeEditModePremium || propType == MothershipPropType.PropTypeEditModeGarageSkin)
			{
				FixCameraPosition();
			}
		}

		public void SetMegaBayState(bool megabotState)
		{
			MegabotDecoration.SetActive(megabotState);
		}

		public void SetNonMegabotDectoration(bool activate)
		{
			NonMegabotDecoration.SetActive(activate);
		}

		private void FixCameraPosition()
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (Camera.get_main() != null)
			{
				Camera.get_main().get_transform().set_localPosition(Vector3.get_zero());
			}
		}

		public void SetGarageNameText(string label)
		{
			UILabel[] robotNameLabels = RobotNameLabels;
			foreach (UILabel val in robotNameLabels)
			{
				val.set_text(label);
			}
		}

		public void SetBayLabelText(uint garageId)
		{
			UILabel[] bayNumberLabels = BayNumberLabels;
			foreach (UILabel val in bayNumberLabels)
			{
				string format = "D2";
				val.set_text(StringTableBase<StringTable>.Instance.GetString("strBay") + " " + (garageId + 1).ToString(format));
			}
		}

		public Vector3 GetFloorCentre()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return FloorCentre.get_position();
		}
	}
}
