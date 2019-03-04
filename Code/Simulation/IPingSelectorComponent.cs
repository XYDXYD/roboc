using Svelto.ES.Legacy;
using UnityEngine;

namespace Simulation
{
	internal interface IPingSelectorComponent : IComponent
	{
		void SetSelectedPingType(PingType type);

		PingType GetSelectedPingType();

		void SetStateButtonsOfType(PingType type, State state);

		void SetButtonsEnabledOfType(PingType type, bool enabled);

		void SetHoverButtonScalerOfType(PingType type, bool hover);

		bool GetButtonsEnabledOfType(PingType type);

		State GetStateButtonsOfType(PingType type);

		void SetProgressBarValue(float value);

		void SetPingSelectorActive(bool active);

		void SetPingSelectorScale(float scale);

		void SetPingSelectorPosition(Vector3 position);
	}
}
