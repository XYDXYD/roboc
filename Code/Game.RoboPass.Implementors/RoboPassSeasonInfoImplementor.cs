using Game.ECS.GUI.Components;
using Game.ECS.GUI.Implementors;
using Game.RoboPass.Components;
using Svelto.ECS;
using System;
using System.Diagnostics;
using UnityEngine;

namespace Game.RoboPass.Implementors
{
	internal class RoboPassSeasonInfoImplementor : MonoBehaviour, IRoboPassSeasonInfoComponent, IButtonComponent, IGUIDisplayComponent
	{
		[SerializeField]
		private GameObject seasonNotAvailablePopup;

		[SerializeField]
		private UIButton seasonNotAvailableConfirmButton;

		[SerializeField]
		private GameObject roboPassScreen;

		public TimeSpan timeRemaining
		{
			get;
			set;
		}

		public int gradesHighestIndex
		{
			get;
			set;
		}

		public int[] xpBetweenGrades
		{
			get;
			set;
		}

		public string robopassSeasonName
		{
			get;
			set;
		}

		public string robopassSeasonNameKey
		{
			get;
			set;
		}

		public RoboPassSeasonRewardData[][] gradesRewards
		{
			get;
			set;
		}

		public bool isValidSeason
		{
			get;
			set;
		}

		public DispatchOnChange<bool> buttonPressed
		{
			get;
			private set;
		}

		public DispatchOnChange<bool> IsShown
		{
			get;
			private set;
		}

		public DispatchOnSet<bool> dataUpdated
		{
			get;
			private set;
		}

		public RoboPassSeasonInfoImplementor()
			: this()
		{
		}

		internal unsafe void Initialize(int entityId)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Expected O, but got Unknown
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected O, but got Unknown
			dataUpdated = new DispatchOnSet<bool>(entityId);
			IsShown = new DispatchOnChange<bool>(entityId);
			IsShown.NotifyOnValueSet((Action<int, bool>)ShowRoboPassScreen);
			buttonPressed = new DispatchOnChange<bool>(entityId);
			seasonNotAvailableConfirmButton.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
		}

		private unsafe void OnDestroy()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			IsShown.StopNotify((Action<int, bool>)ShowRoboPassScreen);
			seasonNotAvailableConfirmButton.onClick.Remove(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
		}

		private void ShowRoboPassScreen(int entityId, bool show)
		{
			if (show)
			{
				bool flag = timeRemaining <= TimeSpan.Zero;
				roboPassScreen.SetActive(!flag);
				seasonNotAvailablePopup.SetActive(flag);
			}
			else
			{
				roboPassScreen.SetActive(false);
				seasonNotAvailablePopup.SetActive(false);
			}
		}

		private void HidePopupClicked()
		{
			buttonPressed.set_value(true);
			buttonPressed.set_value(false);
		}

		[Conditional("DEBUG")]
		private void CheckExposedFields()
		{
		}
	}
}
