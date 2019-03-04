using BehaviorDesigner.Runtime;
using System;

namespace Simulation.SinglePlayer
{
	[Serializable]
	public class SharedAIWeaponShootingFeedbackComponent : SharedVariable<AIWeaponShootingFeedbackComponent>
	{
		public static implicit operator SharedAIWeaponShootingFeedbackComponent(AIWeaponShootingFeedbackComponent value)
		{
			SharedAIWeaponShootingFeedbackComponent sharedAIWeaponShootingFeedbackComponent = new SharedAIWeaponShootingFeedbackComponent();
			sharedAIWeaponShootingFeedbackComponent.set_Value(value);
			return sharedAIWeaponShootingFeedbackComponent;
		}
	}
}
