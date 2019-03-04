using Fabric;
using Simulation;
using Svelto.IoC;
using UnityEngine;

internal sealed class TeleportCooldownDisplay : MonoBehaviour, IHudElement
{
	public GameObject animationRoot;

	public string minOpenAnim;

	public string minCloseAnim;

	public string maxOpenAnim;

	public string maxCloseAnim;

	public UILabel timeLabel;

	private float _displayCooldownTime;

	private bool _teleportIsInProgress;

	private TeleportAnimState _currentState = TeleportAnimState.minClose;

	private TeleportAnimState _desiredState;

	private Animation _animation;

	[Inject]
	public ITeleportCooldownController teleportCooldownController
	{
		private get;
		set;
	}

	[Inject]
	public IHudStyleController battleHudStyleController
	{
		private get;
		set;
	}

	public TeleportCooldownDisplay()
		: this()
	{
	}

	private void Start()
	{
		if (!WorldSwitching.IsMultiplayer())
		{
			this.get_gameObject().SetActive(false);
		}
		Register();
		_animation = animationRoot.GetComponent<Animation>();
		PlayState(TeleportAnimState.minClose);
		battleHudStyleController.AddHud(this);
	}

	private void OnDestroy()
	{
		Unregister();
		battleHudStyleController.RemoveHud(this);
	}

	private void Register()
	{
		teleportCooldownController.OnCooldownRestart += OnCooldownRestart;
		teleportCooldownController.OnTeleportAttemptStart += OnTeleportAttemptStart;
		teleportCooldownController.OnTeleportAttemptEnd += OnTeleportAttemptEnd;
		teleportCooldownController.OnCooldownDeactivated += OnDeactivated;
	}

	private void Unregister()
	{
		if (teleportCooldownController != null)
		{
			teleportCooldownController.OnCooldownRestart -= OnCooldownRestart;
			teleportCooldownController.OnTeleportAttemptStart -= OnTeleportAttemptStart;
			teleportCooldownController.OnTeleportAttemptEnd -= OnTeleportAttemptEnd;
			teleportCooldownController.OnCooldownDeactivated -= OnDeactivated;
		}
	}

	private void OnCooldownRestart()
	{
		_displayCooldownTime = teleportCooldownController.GetCooldownTime();
		if (!_teleportIsInProgress)
		{
			_desiredState = TeleportAnimState.minOpen;
		}
		else
		{
			_desiredState = TeleportAnimState.maxOpen;
		}
	}

	private void OnTeleportAttemptStart()
	{
		_teleportIsInProgress = true;
		if (_desiredState == TeleportAnimState.minOpen)
		{
			_desiredState = TeleportAnimState.maxOpen;
		}
	}

	private void OnTeleportAttemptEnd()
	{
		_teleportIsInProgress = false;
		_desiredState = TeleportAnimState.minOpen;
	}

	private void OnDeactivated()
	{
		_displayCooldownTime = 0f;
		_desiredState = TeleportAnimState.minClose;
		timeLabel.set_text(" ");
		AnimateWidget();
	}

	private void AnimateWidget()
	{
		if (_animation.get_isPlaying())
		{
			return;
		}
		if (_desiredState == TeleportAnimState.maxOpen)
		{
			if (_currentState == TeleportAnimState.maxClose)
			{
				PlayState(TeleportAnimState.maxOpen);
			}
			if (_currentState == TeleportAnimState.minOpen)
			{
				PlayState(TeleportAnimState.maxOpen);
			}
			if (_currentState == TeleportAnimState.minClose)
			{
				PlayState(TeleportAnimState.minOpen);
			}
		}
		if (_desiredState == TeleportAnimState.minOpen)
		{
			if (_currentState == TeleportAnimState.maxOpen)
			{
				PlayState(TeleportAnimState.maxClose);
			}
			if (_currentState == TeleportAnimState.minClose)
			{
				PlayState(TeleportAnimState.minOpen);
			}
		}
		if (_desiredState == TeleportAnimState.minClose)
		{
			if (_currentState == TeleportAnimState.maxClose)
			{
				PlayState(TeleportAnimState.minClose);
			}
			if (_currentState == TeleportAnimState.maxOpen)
			{
				PlayState(TeleportAnimState.maxClose);
			}
			if (_currentState == TeleportAnimState.minOpen)
			{
				PlayState(TeleportAnimState.minClose);
			}
		}
		if (_desiredState == TeleportAnimState.minOpen)
		{
			if (_currentState == TeleportAnimState.maxOpen)
			{
				PlayState(TeleportAnimState.maxClose);
			}
			if (_currentState == TeleportAnimState.minClose)
			{
				PlayState(TeleportAnimState.minOpen);
			}
		}
	}

	private void PlayState(TeleportAnimState state)
	{
		switch (state)
		{
		case TeleportAnimState.maxClose:
			_animation.Play(maxCloseAnim);
			_currentState = state;
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_TeleCoolDownContract", 0, (object)null, this.get_gameObject());
			break;
		case TeleportAnimState.maxOpen:
			_animation.Play(maxOpenAnim);
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_TeleCoolDownExpand", 0, (object)null, this.get_gameObject());
			break;
		case TeleportAnimState.minClose:
			_animation.Play(minCloseAnim);
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_TeleCoolDownClose", 0, (object)null, this.get_gameObject());
			break;
		case TeleportAnimState.minOpen:
			_animation.Play(minOpenAnim);
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_TeleCoolDownOpen", 0, (object)null, this.get_gameObject());
			break;
		}
		_currentState = state;
	}

	private void Update()
	{
		if (_displayCooldownTime <= 0f)
		{
			_desiredState = TeleportAnimState.minClose;
			timeLabel.set_text(" ");
		}
		else
		{
			timeLabel.set_text(_displayCooldownTime.ToString("N"));
		}
		_displayCooldownTime -= Time.get_deltaTime();
		AnimateWidget();
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
