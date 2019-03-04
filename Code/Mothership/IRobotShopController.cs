using System;

namespace Mothership
{
	internal interface IRobotShopController
	{
		event Action<Byte3, uint, byte> OnPreviewAddCubeAt;

		event Action<Byte3, uint> OnPreviewCubeRemovedAt;

		bool IsActive();

		GUIShowResult Show();

		bool Hide();

		void EnableBackground(bool enable);

		void SetupMotherShipPropActivator(MothershipPropActivator motherShipPropActivator);
	}
}
