using Fabric;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class WeaponOrderButton : MonoBehaviour
	{
		public UISprite weaponUISprite;

		public UILabel hotkeyLabel;

		public GameObject defaultBackground;

		public GameObject destroyedBackground;

		public Animation animator;

		public UISprite cooldownSprite;

		public string notEnoughPowerAnimationName = "WeaponSwitchHUD_NoFire";

		private AnimatedAlpha _defaultBackgroundAlpha;

		public AnimatedColor weaponSpriteAnimatedColor;

		private int _itemDescriptorKey;

		private int _itemCategoryInt;

		private bool _isSelected;

		private bool _initialised;

		private const string WEAPON_SELECTED_ANIMATION = "WeaponSwitchHUD_SlotOn";

		private const string WEAPON_DEFAULT_ANIMATION = "WeaponSwitchHUD_SlotOff";

		private const string WEAPON_AVAILABLE_ANIMATION = "WeaponSwitchHUD_WeaponAvailable_Small";

		private const string SELECTED_WEAPON_AVAILABLE_ANIMATION = "WeaponSwitchHUD_WeaponAvailable";

		private const string WEAPON_DESTROYED_ANIMATION = "WeaponSwitchHUD_WeaponDestroyed_Small";

		private const string SELECTED_WEAPON_DESTROYED_ANIMATION = "WeaponSwitchHUD_WeaponDestroyed";

		private Color _defaultSpriteColour = new Color(1f, 1f, 1f, 1f);

		private Queue<string> _animationQueue = new Queue<string>();

		private float _totalCooldownTime;

		public int itemDescriptorKey
		{
			get
			{
				return _itemDescriptorKey;
			}
			set
			{
				_itemDescriptorKey = value;
				ItemDescriptorKey.GetItemCategoryFromKey(itemDescriptorKey, out _itemCategoryInt);
			}
		}

		public int itemCategoryInt => _itemCategoryInt;

		public WeaponOrderButton()
			: this()
		{
		}//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)


		private void Start()
		{
			_defaultBackgroundAlpha = defaultBackground.GetComponent<AnimatedAlpha>();
		}

		public void ShowSelected(bool selected)
		{
			if (!_initialised)
			{
				string animationName = (!selected) ? "WeaponSwitchHUD_SlotOff" : "WeaponSwitchHUD_SlotOn";
				PlayAnimationQueued(animationName);
				_isSelected = selected;
				_initialised = true;
			}
			else if (selected && !_isSelected)
			{
				PlayAnimationNotQueued("WeaponSwitchHUD_SlotOn");
				_isSelected = true;
			}
			else if (!selected && _isSelected)
			{
				PlayAnimationNotQueued("WeaponSwitchHUD_SlotOff");
				_isSelected = false;
			}
		}

		internal void Reset()
		{
			_isSelected = false;
			_initialised = false;
			this.get_gameObject().SetActive(true);
		}

		public void StartCooldown(float cooldownTime)
		{
			cooldownSprite.get_gameObject().SetActive(true);
			_totalCooldownTime = cooldownTime;
		}

		public void ResetCooldown()
		{
			cooldownSprite.get_gameObject().SetActive(false);
		}

		public void ShowInactive()
		{
			this.get_gameObject().SetActive(false);
		}

		public void ShowDestroyed(bool playSound = true)
		{
			if (!(animator == null))
			{
				if (playSound)
				{
					string text = AudioFabricEvent.Name(AudioFabricGameEvents.GUI_WeaponDestroyed);
					EventManager.get_Instance().PostEvent(text);
				}
				if (_isSelected)
				{
					PlayAnimationQueued("WeaponSwitchHUD_WeaponDestroyed");
				}
				else
				{
					PlayAnimationQueued("WeaponSwitchHUD_WeaponDestroyed_Small");
				}
				destroyedBackground.SetActive(true);
				defaultBackground.SetActive(false);
			}
		}

		public void ShowActive()
		{
			if (!(animator == null))
			{
				if (_isSelected)
				{
					PlayAnimationNotQueued("WeaponSwitchHUD_WeaponAvailable");
				}
				else
				{
					PlayAnimationQueued("WeaponSwitchHUD_WeaponAvailable_Small");
				}
				destroyedBackground.SetActive(false);
				defaultBackground.SetActive(true);
			}
		}

		public void ResetAlpha()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			weaponSpriteAnimatedColor.color = _defaultSpriteColour;
			_defaultBackgroundAlpha.alpha = 1f;
		}

		public void PlayNotEnoughPowerAnimation()
		{
			PlayAnimationNotQueued(notEnoughPowerAnimationName);
		}

		private void PlayAnimationQueued(string animationName)
		{
			if (this.get_gameObject().get_activeInHierarchy())
			{
				if (!animator.get_isPlaying())
				{
					animator.Play(animationName);
				}
				else
				{
					_animationQueue.Enqueue(animationName);
				}
			}
		}

		private void PlayAnimationNotQueued(string animationName)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			if (!this.get_gameObject().get_activeInHierarchy())
			{
				return;
			}
			if (animator.get_isPlaying())
			{
				IEnumerator enumerator = animator.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AnimationState val = enumerator.Current;
					if (animator.IsPlaying(val.get_name()))
					{
						val.set_time(val.get_length());
						break;
					}
				}
			}
			animator.PlayQueued(animationName);
		}

		private void Update()
		{
			UpdateAnimationQueue();
		}

		private void UpdateAnimationQueue()
		{
			if (_animationQueue.Count > 0 && !animator.get_isPlaying())
			{
				PlayAnimationQueued(_animationQueue.Dequeue());
			}
		}

		public void SetTime(float time)
		{
			if (time > 0f)
			{
				cooldownSprite.set_fillAmount(time / _totalCooldownTime);
			}
			else
			{
				cooldownSprite.get_gameObject().SetActive(false);
			}
		}
	}
}
