using UnityEngine;

namespace Simulation.BattleArena.CapturePoint
{
	internal class CapturePointMonoBehaviour : MonoBehaviour, IAnimationComponent, IPropComponent, ICaptureRingsComponent, ICaptureZoneComponent, IColorComponent, IAudioComponent
	{
		public Animator _animator;

		public string _loopAudioEvent = "HUD_CaptureBar_Enemy_Loop";

		public string _loopAudioParameter = "Fill_Amount";

		public string _segmentCapturedTrigger = "segmentCaptured";

		public string _captureCompletedTrigger = "captureCompleted";

		public Material[] _bluMaterials;

		public Material[] _redMaterials;

		public Renderer _propRenderer;

		public Renderer _sphereRenderer;

		public Renderer _plateFxRenderer;

		public ParticleSystem _segmentParticleSystem;

		public ParticleSystem[] _captureParticleSystem;

		public Color _allyColor;

		public Color _enemyColor;

		public Animator animator => _animator;

		public string captureCompletedTrigger => _captureCompletedTrigger;

		public string segmentCapturedTrigger => _segmentCapturedTrigger;

		public Material[] bluMaterials => _bluMaterials;

		public Material[] redMaterials => _redMaterials;

		public Renderer propRenderer => _propRenderer;

		public Renderer sphereRenderer => _sphereRenderer;

		public Renderer plateFxRenderer => _plateFxRenderer;

		public ParticleSystem segmentParticleSystem => _segmentParticleSystem;

		public ParticleSystem[] captureParticleSystem => _captureParticleSystem;

		public Color teamColor => _allyColor;

		public Color enemyColor => _enemyColor;

		public string loopAudioEvent => _loopAudioEvent;

		public string loopAudioParameter => _loopAudioParameter;

		public CapturePointMonoBehaviour()
			: this()
		{
		}
	}
}
