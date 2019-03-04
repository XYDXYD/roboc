using Svelto.Context;
using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal sealed class CubeLayoutAdjuster : MonoBehaviour, IInitialize, IWaitForFrameworkDestruction
	{
		[Inject]
		public IGUIInputControllerMothership guiInputController
		{
			get;
			private set;
		}

		[Inject]
		public GaragePresenter garage
		{
			private get;
			set;
		}

		[Inject]
		public IMachineMap machineMap
		{
			private get;
			set;
		}

		public CubeLayoutAdjuster()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			guiInputController.OnScreenStateChange += Recalculate;
			garage.OnCurrentGarageSlotChanged += Recalculate;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			guiInputController.OnScreenStateChange -= Recalculate;
			garage.OnCurrentGarageSlotChanged -= Recalculate;
		}

		private void Recalculate(GarageSlotDependency slot)
		{
			Recalculate();
		}

		public void Recalculate()
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = GameObject.FindGameObjectWithTag("BayCentre");
			if (!(val != null))
			{
				return;
			}
			this.get_transform().SetParent(val.get_transform(), false);
			float num = 4.9f;
			float num2 = 4.9f;
			this.get_transform().set_localPosition(new Vector3(0f - num, 0f, 0f - num2));
			if (!WorldSwitching.IsInBuildMode() && guiInputController != null && guiInputController.GetActiveScreen() != GuiScreens.PrebuiltRobotScreen && guiInputController.GetActiveScreen() != GuiScreens.RobotShop)
			{
				HUDAdvancedEdit hUDAdvancedEdit = Resources.FindObjectsOfTypeAll<HUDAdvancedEdit>()[0];
				if (hUDAdvancedEdit != null && hUDAdvancedEdit.ComIndicator != null)
				{
					Vector3 val2 = val.get_transform().get_position() - hUDAdvancedEdit.ComIndicator.get_transform().get_position();
					Vector3 position = this.get_transform().get_position();
					val2.y = position.y;
					this.get_transform().set_position(this.get_transform().get_position() + val2);
				}
			}
		}
	}
}
