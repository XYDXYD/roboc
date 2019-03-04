using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Simulation
{
	internal interface IBlinkCameraEffectsComponent
	{
		GameObject blinkCameraEffect
		{
			get;
		}

		VignetteAndChromaticAberration blinkAberrationEffect
		{
			get;
		}

		Renderer blinkCameraEffectRenderer
		{
			get;
		}

		float openTime
		{
			get;
		}

		float closeTime
		{
			get;
		}

		float startValueVignetting
		{
			get;
		}

		float endValueVignetting
		{
			get;
		}

		float startValueAberration
		{
			get;
		}

		float endValueAberration
		{
			get;
		}

		float durationUpTransitionVignetting
		{
			get;
		}

		float durationDownTransitionVignetting
		{
			get;
		}

		float durationUpTransitionAberration
		{
			get;
		}

		float durationDownTransitionAberration
		{
			get;
		}

		float mainCameraEffectTimer
		{
			get;
			set;
		}

		float vignettingEffectTimer
		{
			get;
			set;
		}

		float aberrationEffectTimer
		{
			get;
			set;
		}
	}
}
