using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class TeamBaseStatsView : MonoBehaviour, IInitialize
	{
		private const float NGUI_FRIENDLY_ZERO = 1E-06f;

		public Transform[] friendProgressBar;

		public Transform[] enemyProgressBar;

		public int screenEdgeOffset = 20;

		public int screenCentreOffset = 24;

		public UISprite[] friendSliderForeground;

		public UISprite[] friendSliderBackground;

		public UISprite[] enemySliderForeground;

		public UISprite[] enemySliderBackground;

		public Transform[] friendSliderScale;

		public Transform[] enemySliderScale;

		public UILabel friendWarning;

		public UILabel enemyWarning;

		public float barInterpolateSpeed = 5f;

		public Color friendColour;

		public Color enemyColour;

		private int _lastScreenWidth;

		private float _friendProgress;

		private float _enemyProgress;

		private float _displayedOffsetFriend;

		private float _displayedOffsetEnemy;

		private bool _gameStarted;

		[Inject]
		internal TeamBaseStatsPresenter teamBaseStatsPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal LobbyGameStartPresenter lobbyPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal NetworkMachineManager networkMachineManager
		{
			private get;
			set;
		}

		public TeamBaseStatsView()
			: this()
		{
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		public void UpdateStats(bool isMyTeam, float teamProgress)
		{
			if (isMyTeam)
			{
				_friendProgress = teamProgress;
				if (_gameStarted)
				{
					SetFriendWidgetsVisible();
				}
			}
			else
			{
				_enemyProgress = teamProgress;
				if (_gameStarted)
				{
					SetEnemyWidgetsVisible();
				}
			}
		}

		private void Start()
		{
			SetFriendWidgetsInvisible(includeBackground: false);
			SetEnemyWidgetsInvisible(includeBackground: false);
			UpdateBarScaleForScreenWidth();
			_lastScreenWidth = Screen.get_width();
			if (WorldSwitching.GetGameModeType() != GameModeType.TeamDeathmatch && WorldSwitching.GetGameModeType() != GameModeType.Pit)
			{
				teamBaseStatsPresenter.SetView(this);
			}
			UIPanel component = this.GetComponent<UIPanel>();
			component.widgetsAreStatic = false;
		}

		void IInitialize.OnDependenciesInjected()
		{
			LobbyGameStartPresenter lobbyPresenter = this.lobbyPresenter;
			lobbyPresenter.OnInitialLobbyGuiClose = (Action)Delegate.Combine(lobbyPresenter.OnInitialLobbyGuiClose, new Action(GameStarted));
			SetAllProgressBarsProgress(friendSliderScale, 0f);
			SetAllProgressBarsProgress(enemySliderScale, 0f);
		}

		private void GameStarted()
		{
			if (WorldSwitching.GetGameModeType() != GameModeType.Pit)
			{
				_gameStarted = true;
			}
		}

		private void Update()
		{
			UpdateBarSizeForScreenResolution();
			SmoothlyUpdateDisplayedValues();
		}

		private void SetEnemyWidgetsInvisible(bool includeBackground)
		{
			SetWidgetInvisible(enemyWarning);
			for (int i = 0; i < enemySliderForeground.Length; i++)
			{
				SetWidgetInvisible(enemySliderForeground[i]);
			}
			if (includeBackground)
			{
				for (int j = 0; j < enemySliderBackground.Length; j++)
				{
					SetWidgetInvisible(enemySliderBackground[j]);
				}
			}
		}

		private void SetFriendWidgetsInvisible(bool includeBackground)
		{
			SetWidgetInvisible(friendWarning);
			for (int i = 0; i < friendSliderForeground.Length; i++)
			{
				SetWidgetInvisible(friendSliderForeground[i]);
			}
			if (includeBackground)
			{
				for (int j = 0; j < friendSliderBackground.Length; j++)
				{
					SetWidgetInvisible(friendSliderBackground[j]);
				}
			}
		}

		private void SmoothlyUpdateDisplayedValues()
		{
			_displayedOffsetFriend += (_friendProgress - _displayedOffsetFriend) * barInterpolateSpeed * Time.get_deltaTime();
			_displayedOffsetEnemy += (_enemyProgress - _displayedOffsetEnemy) * barInterpolateSpeed * Time.get_deltaTime();
			SetAllProgressBarsProgress(friendSliderScale, _displayedOffsetFriend);
			SetAllProgressBarsProgress(enemySliderScale, _displayedOffsetEnemy);
		}

		private void UpdateBarScaleForScreenWidth()
		{
			float num = Screen.get_width() / 2 - (screenCentreOffset + screenEdgeOffset);
			int num2 = 4;
			int num3 = friendProgressBar.Length;
			float num4 = num - ((float)num3 - 1f) * (float)num2;
			num4 /= (float)num3;
			float num5 = screenCentreOffset;
			for (int i = 0; i < num3; i++)
			{
				SetProgressBarSectionWidth(friendProgressBar[i], num5, num4);
				SetProgressBarSectionWidth(enemyProgressBar[i], num5, num4);
				num5 += num4 + (float)num2;
			}
		}

		private void SetProgressBarSectionWidth(Transform progressBarSection, float offset, float scale)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			Vector3 localScale = progressBarSection.get_localScale();
			localScale.x = Mathf.Max(scale, 1E-06f);
			progressBarSection.set_localScale(localScale);
			Vector3 localPosition = progressBarSection.get_localPosition();
			localPosition.x = offset;
			progressBarSection.set_localPosition(localPosition);
		}

		private void SetAllProgressBarsProgress(Transform[] progressBarForegrounds, float progress)
		{
			int num = (int)progress;
			float progress2 = progress - (float)num;
			for (int i = 0; i < progressBarForegrounds.Length; i++)
			{
				if (num > i)
				{
					SetProgressBarProgress(progressBarForegrounds[i], 1f);
				}
				else if (i == num)
				{
					SetProgressBarProgress(progressBarForegrounds[i], progress2);
				}
				else
				{
					SetProgressBarProgress(progressBarForegrounds[i], 1E-06f);
				}
			}
		}

		private void SetProgressBarProgress(Transform progressBarForeground, float progress)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			Vector3 localScale = progressBarForeground.get_localScale();
			localScale.x = Mathf.Clamp(progress, 1E-06f, 1f);
			progressBarForeground.set_localScale(localScale);
		}

		private void UpdateBarSizeForScreenResolution()
		{
			if (Screen.get_width() != _lastScreenWidth)
			{
				UpdateBarScaleForScreenWidth();
				_lastScreenWidth = screenEdgeOffset;
			}
		}

		private void SetEnemyWidgetsVisible()
		{
			SetWidgetVisible(enemyWarning);
			for (int i = 0; i < enemySliderForeground.Length; i++)
			{
				SetWidgetVisible(enemySliderForeground[i]);
			}
			for (int j = 0; j < enemySliderBackground.Length; j++)
			{
				SetWidgetVisible(enemySliderBackground[j]);
			}
		}

		private void SetFriendWidgetsVisible()
		{
			SetWidgetVisible(friendWarning);
			for (int i = 0; i < friendSliderForeground.Length; i++)
			{
				SetWidgetVisible(friendSliderForeground[i]);
			}
			for (int j = 0; j < friendSliderBackground.Length; j++)
			{
				SetWidgetVisible(friendSliderBackground[j]);
			}
		}

		private void SetWidgetVisible(UIWidget widget)
		{
			widget.set_enabled(true);
		}

		private void SetWidgetInvisible(UIWidget widget)
		{
			widget.set_enabled(false);
		}
	}
}
