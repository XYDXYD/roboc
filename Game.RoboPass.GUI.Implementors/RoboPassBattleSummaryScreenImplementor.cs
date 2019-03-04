using Game.ECS.GUI.Implementors;
using Game.RoboPass.GUI.Components;
using Svelto.ECS;
using System;
using UnityEngine;

namespace Game.RoboPass.GUI.Implementors
{
	public class RoboPassBattleSummaryScreenImplementor : MonoBehaviour, IBattleSummaryScreenComponent, IBattleSummaryAnimationComponent, IGUIDisplayComponent
	{
		[SerializeField]
		private UILabel _currentSeasonLabel;

		[SerializeField]
		private UIButton _continueButton;

		[SerializeField]
		private Animation _levelUpAnimation;

		[SerializeField]
		private UILabel _currentGradeLabel;

		[SerializeField]
		private UISprite _currentProgress;

		[SerializeField]
		private UISprite _newProgress;

		[SerializeField]
		private float _newProgressTweenDuration = 3f;

		public const string LEVELUP_ANIMATION_PART_1 = "RoboPass_Gradeup_Part1";

		public const string LEVELUP_ANIMATION_PART_2 = "RoboPass_Gradeup_Part2";

		private DispatchOnSet<bool> _continueClicked;

		public DispatchOnSet<bool> ContinueClicked => _continueClicked;

		public string CurrentSeasonLabel
		{
			set
			{
				_currentSeasonLabel.set_text(value);
			}
		}

		public string CurrentGradeLabel
		{
			set
			{
				_currentGradeLabel.set_text(value);
			}
		}

		public float CurrentProgress
		{
			set
			{
				_currentProgress.set_fillAmount(value);
			}
		}

		public float NewProgress
		{
			get
			{
				return _newProgress.get_fillAmount();
			}
			set
			{
				_newProgress.set_fillAmount(value);
			}
		}

		public float NewProgressTweenDuration => _newProgressTweenDuration;

		public GameObject GameObject => this.get_gameObject();

		public string LevelUpAnimationPart1 => "RoboPass_Gradeup_Part1";

		public string LevelUpAnimationPart2 => "RoboPass_Gradeup_Part2";

		public Animation Animation => _levelUpAnimation;

		public bool ScreenActive
		{
			set
			{
				this.get_gameObject().SetActive(value);
			}
		}

		public bool ContinueButtonActive
		{
			set
			{
				_continueButton.get_gameObject().SetActive(value);
			}
		}

		public DispatchOnChange<bool> IsShown
		{
			get;
			private set;
		}

		public DispatchOnChange<bool> OnScreenDisplayChange
		{
			get;
			private set;
		}

		public RoboPassBattleSummaryScreenImplementor()
			: this()
		{
		}

		internal unsafe void Initialize(int id)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			_continueClicked = new DispatchOnSet<bool>(id);
			_continueButton.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			OnScreenDisplayChange = new DispatchOnChange<bool>(id);
			IsShown = new DispatchOnChange<bool>(id);
			IsShown.NotifyOnValueSet((Action<int, bool>)OnShowScreen);
			this.get_gameObject().SetActive(false);
		}

		private void OnShowScreen(int id, bool show)
		{
			OnScreenDisplayChange.set_value(show);
		}

		private void OnContinueClick()
		{
			_continueClicked.set_value(true);
		}

		private void OnDestroy()
		{
			IsShown.StopNotify((Action<int, bool>)OnShowScreen);
		}
	}
}
