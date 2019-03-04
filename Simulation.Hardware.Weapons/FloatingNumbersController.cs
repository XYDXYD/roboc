using Fabric;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal class FloatingNumbersController
	{
		private const string POOL_NAME = "FloatingNumbers";

		private readonly HealthTracker _healthTracker;

		private readonly ObjectPool<FloatingNumber> _numberPool = new ObjectPool<FloatingNumber>();

		private readonly List<FloatingNumber> _numbers = new List<FloatingNumber>(10);

		private FloatingNumbersView _view;

		private HUDPlayerIDDisplay _hudPlayerIdDisplay;

		public FloatingNumbersController(HealthTracker healthTracker)
		{
			_healthTracker = healthTracker;
		}

		public void SetView(FloatingNumbersView view)
		{
			_view = view;
			_numberPool.Preallocate("FloatingNumbers", 20, (Func<FloatingNumber>)CreateFloatingNumberInstance);
			_healthTracker.HealthChanged += OnHealthChanged;
			TaskRunner.get_Instance().Run((Func<IEnumerator>)UpdateNumberPositions);
		}

		public void SetHUDPlayerIDDisplay(HUDPlayerIDDisplay display)
		{
			_hudPlayerIdDisplay = display;
		}

		private void OnHealthChanged(HealthChangeData data)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			if (data.shooterIsMe && !data.targetIsMe)
			{
				Transform floatingNumbersHolder = _hudPlayerIdDisplay.GetFloatingNumbersHolder(data.targetId);
				if (data.deltaHealth < 0)
				{
					AddNumber(-data.deltaHealth, _view.damageColor, floatingNumbersHolder, data.IsCriticalHit, data.isTargetDead);
				}
				else
				{
					AddNumber(data.deltaHealth, _view.healingColor, floatingNumbersHolder, isCrit: false, isLastHit: false);
				}
			}
		}

		private void AddNumber(float deltaHealth, Color color, Transform parent, bool isCrit, bool isLastHit)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			FloatingNumber floatingNumber = _numberPool.Use("FloatingNumbers", (Func<FloatingNumber>)CreateFloatingNumberInstance);
			if (isLastHit)
			{
				Reparent(_view.viewTransform, floatingNumber.numberTransform);
				floatingNumber.numberTransform.set_position(parent.get_position());
				Transform numberTransform = floatingNumber.numberTransform;
				Vector3 lossyScale = parent.get_lossyScale();
				Vector3 lossyScale2 = _view.viewTransform.get_lossyScale();
				numberTransform.set_localScale(lossyScale / lossyScale2.x);
			}
			else
			{
				Reparent(parent, floatingNumber.numberTransform);
			}
			floatingNumber.numberGameobject.SetActive(true);
			floatingNumber.text = deltaHealth.ToString("F0", CultureInfo.InvariantCulture);
			floatingNumber.textColor = color;
			string text = (!isCrit) ? "FloatingNumber_Normal" : "FloatingNumber_Critical";
			floatingNumber.labelAnimation.Stop();
			floatingNumber.labelAnimation.Play(text);
			_numbers.Add(floatingNumber);
			if (isCrit)
			{
				EventManager.get_Instance().PostEvent(_view.critAudioEvent, 0, (object)null, _view.get_gameObject());
			}
		}

		private IEnumerator UpdateNumberPositions()
		{
			while (true)
			{
				for (int num = _numbers.Count - 1; num >= 0; num--)
				{
					FloatingNumber floatingNumber = _numbers[num];
					if (!floatingNumber.labelAnimation.get_isPlaying())
					{
						_numbers.RemoveAt(num);
						floatingNumber.get_gameObject().SetActive(false);
						_numberPool.Recycle(floatingNumber, "FloatingNumbers");
					}
				}
				yield return null;
			}
		}

		private FloatingNumber CreateFloatingNumberInstance()
		{
			GameObject val = Object.Instantiate<GameObject>(_view.prefab);
			val.SetActive(false);
			Reparent(_view.viewTransform, val.get_transform());
			return val.GetComponent<FloatingNumber>();
		}

		private static void Reparent(Transform parent, Transform child)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			child.set_parent(parent);
			child.set_localPosition(Vector3.get_zero());
			child.set_localScale(Vector3.get_one());
		}
	}
}
