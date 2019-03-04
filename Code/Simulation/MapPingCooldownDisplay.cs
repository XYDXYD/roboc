using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal class MapPingCooldownDisplay : MonoBehaviour, IHudElement
	{
		public GameObject AnimationRoot;

		public string CooldownAnimation = string.Empty;

		public UILabel TimeLabel;

		private Animation _animation;

		private float _cooldownTimeRemaining;

		[Inject]
		internal MapPingCooldownObserver mapPingCooldownObserver
		{
			private get;
			set;
		}

		[Inject]
		internal IHudStyleController battleHudStyleController
		{
			private get;
			set;
		}

		public MapPingCooldownDisplay()
			: this()
		{
		}

		private void Start()
		{
			_animation = AnimationRoot.GetComponent<Animation>();
			Register();
			battleHudStyleController.AddHud(this);
		}

		private void OnDestroy()
		{
			Unregister();
			battleHudStyleController.RemoveHud(this);
		}

		private void Update()
		{
			if (_cooldownTimeRemaining > 0f)
			{
				_cooldownTimeRemaining -= Time.get_deltaTime();
				if (_cooldownTimeRemaining < 0f)
				{
					TimeLabel.set_text(string.Empty);
				}
				else
				{
					TimeLabel.set_text(_cooldownTimeRemaining.ToString("N"));
				}
			}
		}

		private void StartCooldown(float cooldownTime)
		{
			_cooldownTimeRemaining = cooldownTime;
			_animation.Play(CooldownAnimation);
			_animation.Rewind();
		}

		private void Register()
		{
			mapPingCooldownObserver.StartCooldown += StartCooldown;
		}

		private void Unregister()
		{
			mapPingCooldownObserver.StartCooldown -= StartCooldown;
		}

		public void SetStyle(HudStyle style)
		{
			switch (style)
			{
			case HudStyle.HideAllButChat:
				this.get_gameObject().SetActive(false);
				break;
			case HudStyle.HideAll:
				this.get_gameObject().SetActive(false);
				break;
			case HudStyle.Full:
				this.get_gameObject().SetActive(true);
				break;
			}
		}
	}
}
