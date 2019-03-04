using Svelto.ECS;
using Svelto.Ticker.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Modules
{
	internal sealed class TeleportModuleCameraEffectsEngine : MultiEntityViewsEngine<TeleportModuleEffectsNode, BlinkCameraEffectsNode>, ITickable, ITickableBase
	{
		private bool _playCameraEffect;

		private bool _teleportStarted;

		private BlinkCameraEffectsNode _blinkCameraEffectsNode;

		protected override void Add(TeleportModuleEffectsNode effectsNode)
		{
			effectsNode.teleporterComponent.teleportStarted.subscribers += PlayCameraEffect;
			effectsNode.teleporterComponent.teleportEnded.subscribers += PlayCameraEffect;
		}

		protected override void Remove(TeleportModuleEffectsNode effectsNode)
		{
			effectsNode.teleporterComponent.teleportStarted.subscribers -= PlayCameraEffect;
			effectsNode.teleporterComponent.teleportEnded.subscribers -= PlayCameraEffect;
		}

		protected override void Add(BlinkCameraEffectsNode entityView)
		{
			_blinkCameraEffectsNode = entityView;
		}

		protected override void Remove(BlinkCameraEffectsNode entityView)
		{
			_blinkCameraEffectsNode = null;
		}

		public void Tick(float deltaSec)
		{
			if (_playCameraEffect)
			{
				IBlinkCameraEffectsComponent blinkCameraEffectsComponent = _blinkCameraEffectsNode.blinkCameraEffectsComponent;
				float @float = blinkCameraEffectsComponent.blinkCameraEffectRenderer.get_material().GetFloat("_Range");
				float intensity = blinkCameraEffectsComponent.blinkAberrationEffect.intensity;
				float chromaticAberration = blinkCameraEffectsComponent.blinkAberrationEffect.chromaticAberration;
				if (_teleportStarted)
				{
					CameraEffectTransitionUp(blinkCameraEffectsComponent, @float, intensity, chromaticAberration);
				}
				else
				{
					CameraEffectTransitionDown(blinkCameraEffectsComponent, @float, intensity, chromaticAberration);
				}
			}
		}

		private void CameraEffectTransitionDown(IBlinkCameraEffectsComponent blinkCameraEffectsComponent, float currentValueCameraEffect, float currentValueVignettingEffect, float currentValueAberrationEffect)
		{
			if (currentValueCameraEffect > 0f)
			{
				float num = Mathf.Lerp(1f, 0f, blinkCameraEffectsComponent.mainCameraEffectTimer / blinkCameraEffectsComponent.closeTime);
				blinkCameraEffectsComponent.blinkCameraEffectRenderer.get_material().SetFloat("_Range", num);
				blinkCameraEffectsComponent.mainCameraEffectTimer += Time.get_deltaTime();
			}
			if (currentValueVignettingEffect > blinkCameraEffectsComponent.startValueVignetting)
			{
				float intensity = Mathf.Lerp(blinkCameraEffectsComponent.endValueVignetting, blinkCameraEffectsComponent.startValueVignetting, blinkCameraEffectsComponent.vignettingEffectTimer / blinkCameraEffectsComponent.durationDownTransitionVignetting);
				blinkCameraEffectsComponent.blinkAberrationEffect.intensity = intensity;
				blinkCameraEffectsComponent.vignettingEffectTimer += Time.get_deltaTime();
			}
			if (currentValueAberrationEffect > blinkCameraEffectsComponent.startValueAberration)
			{
				float chromaticAberration = Mathf.Lerp(blinkCameraEffectsComponent.endValueAberration, blinkCameraEffectsComponent.startValueAberration, blinkCameraEffectsComponent.aberrationEffectTimer / blinkCameraEffectsComponent.durationDownTransitionAberration);
				blinkCameraEffectsComponent.blinkAberrationEffect.chromaticAberration = chromaticAberration;
				blinkCameraEffectsComponent.aberrationEffectTimer += Time.get_deltaTime();
			}
			if (currentValueCameraEffect <= 0f && currentValueVignettingEffect <= blinkCameraEffectsComponent.startValueVignetting && currentValueAberrationEffect <= blinkCameraEffectsComponent.startValueAberration)
			{
				_playCameraEffect = false;
				blinkCameraEffectsComponent.blinkCameraEffect.SetActive(false);
			}
		}

		private static void CameraEffectTransitionUp(IBlinkCameraEffectsComponent blinkCameraEffectsComponent, float currentValueCameraEffect, float currentValueVignettingEffect, float currentValueAberrationEffect)
		{
			if (currentValueCameraEffect < 1f)
			{
				float num = Mathf.Lerp(0f, 1f, blinkCameraEffectsComponent.mainCameraEffectTimer / blinkCameraEffectsComponent.openTime);
				blinkCameraEffectsComponent.blinkCameraEffectRenderer.get_material().SetFloat("_Range", num);
				blinkCameraEffectsComponent.mainCameraEffectTimer += Time.get_deltaTime();
			}
			if (currentValueVignettingEffect < blinkCameraEffectsComponent.endValueVignetting)
			{
				float intensity = Mathf.Lerp(blinkCameraEffectsComponent.startValueVignetting, blinkCameraEffectsComponent.endValueVignetting, blinkCameraEffectsComponent.vignettingEffectTimer / blinkCameraEffectsComponent.durationUpTransitionVignetting);
				blinkCameraEffectsComponent.blinkAberrationEffect.intensity = intensity;
				blinkCameraEffectsComponent.vignettingEffectTimer += Time.get_deltaTime();
			}
			if (currentValueAberrationEffect < blinkCameraEffectsComponent.endValueAberration)
			{
				float chromaticAberration = Mathf.Lerp(blinkCameraEffectsComponent.startValueAberration, blinkCameraEffectsComponent.endValueAberration, blinkCameraEffectsComponent.aberrationEffectTimer / blinkCameraEffectsComponent.durationUpTransitionAberration);
				blinkCameraEffectsComponent.blinkAberrationEffect.chromaticAberration = chromaticAberration;
				blinkCameraEffectsComponent.aberrationEffectTimer += Time.get_deltaTime();
			}
		}

		private void PlayCameraEffect(ITeleporterComponent sender, int moduleId)
		{
			IBlinkCameraEffectsComponent blinkCameraEffectsComponent = _blinkCameraEffectsNode.blinkCameraEffectsComponent;
			_teleportStarted = sender.teleportActivated;
			if (_teleportStarted)
			{
				_playCameraEffect = true;
				blinkCameraEffectsComponent.blinkCameraEffect.SetActive(sender.teleportActivated);
			}
			blinkCameraEffectsComponent.mainCameraEffectTimer = 0f;
			blinkCameraEffectsComponent.aberrationEffectTimer = 0f;
			blinkCameraEffectsComponent.vignettingEffectTimer = 0f;
		}
	}
}
