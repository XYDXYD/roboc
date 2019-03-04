using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation
{
	internal interface IDiscShieldEffectsComponent
	{
		float openTime
		{
			get;
		}

		float openingTimer
		{
			get;
			set;
		}

		float closeTime
		{
			get;
		}

		float closingTimer
		{
			get;
			set;
		}

		Renderer discShieldRenderer
		{
			get;
		}

		Renderer ringEffectRenderer
		{
			get;
		}

		float ringOpenTime
		{
			get;
		}

		float ringOpeningTimer
		{
			get;
			set;
		}

		float ringCloseTime
		{
			get;
		}

		float ringClosingTimer
		{
			get;
			set;
		}

		float nearToCloseEffectStartTime
		{
			get;
		}

		float nearToCloseEffectsTimer
		{
			get;
			set;
		}

		Dispatcher<IDiscShieldEffectsComponent, int> startOpenEffect
		{
			get;
		}

		Dispatcher<IDiscShieldEffectsComponent, int> startNearToCloseEffect
		{
			get;
		}

		Dispatcher<IDiscShieldEffectsComponent, int> startCloseEffect
		{
			get;
		}
	}
}
