using Game.ECS.GUI.Components;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership.GUI
{
	internal class GameModePreferencesScreen : MonoBehaviour, IShowComponent, IDialogChoiceComponent, IGameModePreferencesWidgetComponent, IFormValidationComponent
	{
		private bool _isFormValid;

		[SerializeField]
		private UIButton _validateButton;

		[SerializeField]
		private UIToggle _battleArenaToggle;

		[SerializeField]
		private UIToggle _teamDeathmatchToggle;

		[SerializeField]
		private UIToggle _eliminationToggle;

		public DispatchOnChange<bool> cancelPressed
		{
			get;
			set;
		}

		public DispatchOnChange<bool> validatePressed
		{
			get;
			set;
		}

		public DispatchOnChange<bool> isShown
		{
			get;
			private set;
		}

		public bool isFormValid
		{
			get
			{
				return _isFormValid;
			}
			set
			{
				if (_isFormValid != value)
				{
					_isFormValid = value;
					_validateButton.set_isEnabled(_isFormValid);
				}
			}
		}

		public List<GameModeType> availableGameModeTypes
		{
			get;
			set;
		}

		public DispatchOnChange<GameModePreferences> preferences
		{
			get;
			set;
		}

		public GameModePreferencesScreen()
			: this()
		{
		}

		public unsafe void Initialize(int entityId)
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Expected O, but got Unknown
			cancelPressed = new DispatchOnChange<bool>(entityId);
			validatePressed = new DispatchOnChange<bool>(entityId);
			isShown = new DispatchOnChange<bool>(entityId);
			isShown.NotifyOnValueSet((Action<int, bool>)delegate(int _, bool v)
			{
				this.get_gameObject().SetActive(v);
			});
			preferences = new DispatchOnChange<GameModePreferences>(entityId);
			EventDelegate.Add(_validateButton.onClick, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			InitializeToggle(_battleArenaToggle, GameModeType.Normal);
			InitializeToggle(_teamDeathmatchToggle, GameModeType.TeamDeathmatch);
			InitializeToggle(_eliminationToggle, GameModeType.SuddenDeath);
			preferences.NotifyOnValueSet((Action<int, GameModePreferences>)SetToggles);
		}

		private unsafe void InitializeToggle(UIToggle toggle, GameModeType gameMode)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			_003CInitializeToggle_003Ec__AnonStorey0 _003CInitializeToggle_003Ec__AnonStorey;
			EventDelegate.Add(toggle.onChange, new Callback((object)_003CInitializeToggle_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void SetToggles(int _, GameModePreferences prefs)
		{
			_battleArenaToggle.Set(prefs.Contains(GameModeType.Normal), false);
			_teamDeathmatchToggle.Set(prefs.Contains(GameModeType.TeamDeathmatch), false);
			_eliminationToggle.Set(prefs.Contains(GameModeType.SuddenDeath), false);
		}
	}
}
