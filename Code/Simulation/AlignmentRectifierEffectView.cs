using Fabric;
using UnityEngine;

namespace Simulation
{
	internal sealed class AlignmentRectifierEffectView : MonoBehaviour
	{
		private enum SoundState
		{
			Idle,
			PreLoopSequence,
			Loop,
			EndSequence,
			WaitForEnd
		}

		private float _outroStartTime;

		private float _loopSoundStopTime;

		private float _loopSoundStartDelay;

		private string _audioIntroEffect;

		private string _audioLoopEffect;

		private string _audioOutroEffect;

		private float _size;

		private AlignmentRectifierParticleScaler[] _particleScalers;

		private ParticleSystem _rootParticle;

		private GameObjectPool _gameObjectPool;

		private float _timer;

		private SoundState _soundState;

		public AlignmentRectifierEffectView()
			: this()
		{
		}

		private void Awake()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			MeshRenderer[] componentsInChildren = this.GetComponentsInChildren<MeshRenderer>(true);
			_rootParticle = this.GetComponent<ParticleSystem>();
			Bounds bounds = componentsInChildren[0].get_bounds();
			Vector3 extents = bounds.get_extents();
			_size = extents.x;
			_particleScalers = this.GetComponentsInChildren<AlignmentRectifierParticleScaler>(true);
			this.get_gameObject().SetActive(false);
		}

		private void OnDestroy()
		{
			_gameObjectPool = null;
		}

		private void Update()
		{
			_timer += Time.get_deltaTime();
			switch (_soundState)
			{
			case SoundState.Idle:
				break;
			case SoundState.PreLoopSequence:
				if (_timer >= _loopSoundStartDelay)
				{
					EventManager.get_Instance().PostEvent(_audioLoopEffect, 0, (object)null, this.get_gameObject());
					_soundState = SoundState.Loop;
				}
				break;
			case SoundState.Loop:
				if (_timer >= _outroStartTime)
				{
					EventManager.get_Instance().PostEvent(_audioOutroEffect, 0, (object)null, this.get_gameObject());
					_soundState = SoundState.EndSequence;
				}
				break;
			case SoundState.EndSequence:
				if (_timer >= _loopSoundStopTime)
				{
					EventManager.get_Instance().PostEvent(_audioLoopEffect, 1, (object)null, this.get_gameObject());
					_soundState = SoundState.WaitForEnd;
				}
				break;
			case SoundState.WaitForEnd:
				if (!EventManager.get_Instance().IsEventActive(_audioOutroEffect, this.get_gameObject()))
				{
					_soundState = SoundState.Idle;
					RecycleGobj();
				}
				break;
			}
		}

		public void SetSize(float size)
		{
			float scaleFactor = size / _size;
			for (int i = 0; i < _particleScalers.Length; i++)
			{
				_particleScalers[i].ScaleSize(scaleFactor);
			}
			_size = size;
		}

		public void Stop()
		{
			if (_soundState != 0)
			{
				EventManager.get_Instance().PostEvent(_audioLoopEffect, 1, (object)null, this.get_gameObject());
				EventManager.get_Instance().PostEvent(_audioOutroEffect, 1, (object)null, this.get_gameObject());
				EventManager.get_Instance().PostEvent(_audioIntroEffect, 1, (object)null, this.get_gameObject());
				_soundState = SoundState.Idle;
				RecycleGobj();
			}
		}

		public void Play(GameObjectPool gameObjectPool, AlignmentRectifierData alignmentRectifierData)
		{
			_outroStartTime = alignmentRectifierData.outroStartTime;
			_loopSoundStopTime = alignmentRectifierData.loopSoundStopTime;
			_loopSoundStartDelay = alignmentRectifierData.loopSoundStartDelay;
			_audioIntroEffect = alignmentRectifierData.audioIntroEffect;
			_audioLoopEffect = alignmentRectifierData.audioLoopEffect;
			_audioOutroEffect = alignmentRectifierData.audioOutroEffect;
			_gameObjectPool = gameObjectPool;
			_rootParticle.Play();
			_timer = 0f;
			EventManager.get_Instance().PostEvent(_audioIntroEffect, 0, (object)null, this.get_gameObject());
			_soundState = SoundState.PreLoopSequence;
		}

		private void RecycleGobj()
		{
			this.get_gameObject().get_transform().set_parent(null);
			_gameObjectPool.Recycle(this.get_gameObject(), this.get_gameObject().get_name());
			this.get_gameObject().SetActive(false);
		}
	}
}
