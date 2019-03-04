using Svelto.IoC;
using System;
using UnityEngine;
using Utility;

namespace Mothership.GUI
{
	internal class ReconnectScreenView : MonoBehaviour, IInitialize
	{
		[SerializeField]
		private UIButton reconnectButton;

		[SerializeField]
		private UIButton cancelButton;

		[SerializeField]
		private GameObject gameOpenState;

		[SerializeField]
		private GameObject gameClosedState;

		[Inject]
		internal ReconnectPresenter _presenter
		{
			get;
			set;
		}

		public ReconnectScreenView()
			: this()
		{
		}

		public unsafe void OnDependenciesInjected()
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected O, but got Unknown
			Console.Log("OnDependenciesInjected ReconnectScreenView");
			_presenter.SetView(this);
			EventDelegate.Add(reconnectButton.onClick, new Callback((object)_presenter, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			EventDelegate.Add(cancelButton.onClick, new Callback((object)_presenter, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			Hide();
		}

		public void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		internal void GameEnded()
		{
			gameOpenState.SetActive(false);
			gameClosedState.SetActive(true);
		}
	}
}
