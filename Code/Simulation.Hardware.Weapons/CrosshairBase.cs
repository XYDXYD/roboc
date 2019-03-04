using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal class CrosshairBase
	{
		internal enum CrosshairState
		{
			Default,
			NoFire,
			Count
		}

		private const int NUM_SMOOTHING_FRAMES = 8;

		protected CrosshairController _crosshairController;

		protected float _hitDuration;

		protected float _hitTime;

		protected float _targetAccuracy;

		protected float _offsetAtMinAccuracy;

		protected List<SpriteInfo> _moveableSprites = new List<SpriteInfo>();

		private readonly Queue<float> _smoothedTargetAccuracy = new Queue<float>(9);

		private readonly GameObject _defaultGO;

		private readonly Transform _damageTransform;

		private readonly Animation _damageAnimation;

		private readonly Transform _critTransform;

		private readonly Animation _criticalAnimation;

		private readonly GameObject _noFireGO;

		private readonly GameObject _parentGO;

		protected CrosshairState _currentState;

		private readonly float _minScale;

		private readonly float _maxScale;

		private readonly float _scaleIncreasePerHit;

		private readonly float _scaleDecreasePerSecond;

		public float accuracy
		{
			set
			{
				_targetAccuracy = value;
			}
		}

		public CrosshairBase(BaseCrosshairInfo info, GameObject moveParent, CrosshairController crosshairController, CrosshairWidget crosshairWidget, float offsetAtMinAccuracy)
		{
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			_parentGO = info.CrosshairParent;
			_defaultGO = info.Default;
			_damageTransform = info.Damage.get_transform();
			_critTransform = info.CriticalHit.get_transform();
			_noFireGO = info.NoFire;
			_crosshairController = crosshairController;
			_offsetAtMinAccuracy = offsetAtMinAccuracy;
			_hitDuration = crosshairWidget.HitDuration;
			_damageAnimation = _damageTransform.GetComponent<Animation>();
			_criticalAnimation = info.CriticalHit.GetComponent<Animation>();
			_minScale = info.DamageMinScale;
			_maxScale = info.DamageMaxScale;
			_scaleIncreasePerHit = info.DamageScaleIncreasePerHit;
			_scaleDecreasePerSecond = info.DamageScaleDecreasePerSecond;
			if (moveParent != null)
			{
				UISprite[] componentsInChildren = moveParent.GetComponentsInChildren<UISprite>(true);
				UISprite[] array = componentsInChildren;
				foreach (UISprite val in array)
				{
					_moveableSprites.Add(new SpriteInfo(val.get_transform(), val, val.get_transform().get_localPosition()));
				}
			}
		}

		public void Register()
		{
			_crosshairController.RegisterCrosshair(this);
		}

		public void Unregister()
		{
			_crosshairController.UnRegisterCrosshair(this);
		}

		public void UpdateAccuracyMotion()
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			float smoothedTargetAccuracy = GetSmoothedTargetAccuracy();
			foreach (SpriteInfo moveableSprite in _moveableSprites)
			{
				moveableSprite.T.set_localPosition(moveableSprite.StartingPos - moveableSprite.T.get_up() * smoothedTargetAccuracy * _offsetAtMinAccuracy);
			}
		}

		public void UpdateDamageIndicator()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			Transform damageTransform = _damageTransform;
			damageTransform.set_localScale(damageTransform.get_localScale() - Vector3.get_one() * _scaleDecreasePerSecond * Time.get_deltaTime());
			Vector3 localScale = _damageTransform.get_localScale();
			if (localScale.x < _minScale)
			{
				_damageTransform.set_localScale(Vector3.get_one() * _minScale);
			}
			_critTransform.set_localScale(_damageTransform.get_localScale());
		}

		public void ShowDamageEffect(int stackPercent)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			if (_parentGO.get_activeInHierarchy())
			{
				_hitTime = Time.get_time() + _hitDuration;
				Transform damageTransform = _damageTransform;
				damageTransform.set_localScale(damageTransform.get_localScale() + Vector3.get_one() * _scaleIncreasePerHit);
				Vector3 localScale = _damageTransform.get_localScale();
				if (localScale.x > _maxScale)
				{
					_damageTransform.set_localScale(Vector3.get_one() * _maxScale);
				}
				_critTransform.set_localScale(_damageTransform.get_localScale());
				_damageAnimation.Stop();
				_damageAnimation.Play();
				UpdateStackCount(stackPercent);
			}
		}

		public void ActivateNoFireState(bool active)
		{
			if (_noFireGO != null)
			{
				_noFireGO.SetActive(active);
			}
			_defaultGO.SetActive(!active);
		}

		public virtual void ChangeState(CrosshairState newState)
		{
			if (_currentState != newState)
			{
				if (_noFireGO == null || !_noFireGO.get_activeInHierarchy())
				{
					_defaultGO.SetActive(newState == CrosshairState.Default);
				}
				_currentState = newState;
			}
		}

		public void ShowCrit()
		{
			_criticalAnimation.Stop();
			_criticalAnimation.Play();
		}

		public void ResetAnimations()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Expected O, but got Unknown
			IEnumerator enumerator = _criticalAnimation.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AnimationState val = enumerator.Current;
					val.set_normalizedTime(1f);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			IEnumerator enumerator2 = _damageAnimation.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					AnimationState val2 = enumerator2.Current;
					val2.set_normalizedTime(1f);
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator2 as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
			_damageAnimation.Play();
			_criticalAnimation.Play();
		}

		internal virtual void ActivateGroundWarning(bool active)
		{
		}

		internal virtual void UpdateState()
		{
		}

		protected float GetSmoothedTargetAccuracy()
		{
			_smoothedTargetAccuracy.Enqueue(_targetAccuracy);
			while (_smoothedTargetAccuracy.Count > 8)
			{
				_smoothedTargetAccuracy.Dequeue();
			}
			float num = 0f;
			foreach (float item in _smoothedTargetAccuracy)
			{
				float num2 = item;
				num += num2;
			}
			return num / (float)_smoothedTargetAccuracy.Count;
		}

		protected virtual void UpdateStackCount(int stackPercent)
		{
		}
	}
}
