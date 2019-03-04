using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Simulation;
using Simulation.Hardware.Weapons;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

internal class DamageIndicator : MonoBehaviour
{
	public GameObject Indicator;

	public float fadeInTime = 0.2f;

	public float fadeOutTime = 2.8f;

	private Dictionary<int, Sequence> _activeTweens = new Dictionary<int, Sequence>();

	private Transform _playerCamera;

	private Transform T;

	[Inject]
	internal DestructionReporter destructionReporter
	{
		private get;
		set;
	}

	[Inject]
	internal PlayerMachinesContainer playerMachinesContainer
	{
		private get;
		set;
	}

	[Inject]
	internal RigidbodyDataContainer rigidbodyDataContainer
	{
		private get;
		set;
	}

	[Inject]
	internal DamageIndicatorArrowPool arrowPool
	{
		private get;
		set;
	}

	public DamageIndicator()
		: this()
	{
	}

	private void Start()
	{
		T = this.get_transform();
		_playerCamera = Camera.get_main().get_transform();
		Indicator.get_gameObject().SetActive(false);
		destructionReporter.OnPlayerDamageApplied += HandleOnPlayerTakesDamage;
		arrowPool.Preallocate(Indicator.get_name(), 4, (Func<DamageIndicatorArrow>)OnFirstArrowInit);
	}

	private DamageIndicatorArrow OnFirstArrowInit()
	{
		GameObject val = Object.Instantiate<GameObject>(Indicator);
		val.set_name(Indicator.get_name());
		return val.GetComponent<DamageIndicatorArrow>();
	}

	private void OnDestroy()
	{
		destructionReporter.OnPlayerDamageApplied -= HandleOnPlayerTakesDamage;
		List<Sequence> list = new List<Sequence>(_activeTweens.Values);
		for (int i = 0; i < list.Count; i++)
		{
			TweenExtensions.Kill(list[i], false);
		}
	}

	private unsafe void HandleOnPlayerTakesDamage(DestructionData data)
	{
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Expected O, but got Unknown
		//IL_0116: Expected O, but got Unknown
		if (data.targetIsMe && !data.shooterIsMe)
		{
			int shooterId = data.shooterId;
			if (!_activeTweens.ContainsKey(shooterId))
			{
				int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, shooterId);
				Rigidbody shooterRB = rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, activeMachine);
				Rigidbody playerRB = data.hitRigidbody;
				DamageIndicatorArrow indicatorArrow = arrowPool.Use(Indicator.get_name(), (Func<DamageIndicatorArrow>)OnFirstArrowInit);
				indicatorArrow.get_gameObject().SetActive(true);
				indicatorArrow.get_transform().set_parent(T.get_parent());
				indicatorArrow.get_transform().set_localScale(Vector3.get_one());
				_003CHandleOnPlayerTakesDamage_003Ec__AnonStorey0 _003CHandleOnPlayerTakesDamage_003Ec__AnonStorey;
				Sequence value = StartAlphaTween(indicatorArrow.GetSprite(), new TweenCallback((object)_003CHandleOnPlayerTakesDamage_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new TweenCallback((object)_003CHandleOnPlayerTakesDamage_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
				_activeTweens.Add(shooterId, value);
			}
			else
			{
				TweenExtensions.Restart(_activeTweens[shooterId], true);
			}
		}
	}

	private void UpdatePosition(GameObject indicator, Rigidbody shooterRB, Rigidbody playerRB, int shooterId)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		if (shooterRB != null && playerRB != null)
		{
			Vector3 val = shooterRB.get_worldCenterOfMass() - playerRB.get_worldCenterOfMass();
			Vector3 forward = _playerCamera.get_forward();
			Vector3 right = _playerCamera.get_right();
			forward.y = 0f;
			forward.Normalize();
			right.y = 0f;
			val.y = 0f;
			val.Normalize();
			float num = Mathf.Clamp(Vector3.Dot(forward, val), -1f, 1f);
			float num2 = Mathf.Acos(num) * 57.29578f;
			if (Vector3.Dot(right, val) > 0f)
			{
				num2 *= -1f;
			}
			indicator.get_transform().set_rotation(Quaternion.Euler(0f, 0f, num2));
		}
		else
		{
			TweenExtensions.Goto(_activeTweens[shooterId], fadeInTime + fadeOutTime, false);
		}
	}

	private unsafe Sequence StartAlphaTween(UISprite sprite, TweenCallback onComplete, TweenCallback onUpdate)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.OnKill<Sequence>(val, onComplete);
		TweenSettingsExtensions.OnUpdate<Sequence>(val, onUpdate);
		if (sprite != null)
		{
			Color color = sprite.get_color();
			Color val2 = color;
			val2.a = 0f;
			sprite.set_color(val2);
			color.a = 1f;
			_003CStartAlphaTween_003Ec__AnonStorey2 _003CStartAlphaTween_003Ec__AnonStorey;
			TweenSettingsExtensions.Append(val, TweenSettingsExtensions.SetAs<TweenerCore<Color, Color, ColorOptions>>(DOTween.To(new DOGetter<Color>((object)_003CStartAlphaTween_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<Color>((object)_003CStartAlphaTween_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), color, fadeInTime), new TweenParams().SetEase(2, (float?)null, (float?)null)));
			TweenSettingsExtensions.Append(val, TweenSettingsExtensions.SetAs<TweenerCore<Color, Color, ColorOptions>>(DOTween.To(new DOGetter<Color>((object)_003CStartAlphaTween_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<Color>((object)_003CStartAlphaTween_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), val2, fadeOutTime), new TweenParams().SetEase(3, (float?)null, (float?)null)));
		}
		TweenExtensions.Play<Sequence>(val);
		return val;
	}
}
