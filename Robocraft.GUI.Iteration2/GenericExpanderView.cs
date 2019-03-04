using System;
using UnityEngine;

namespace Robocraft.GUI.Iteration2
{
	internal class GenericExpanderView : MonoBehaviour, IView
	{
		[SerializeField]
		private bool _expanded = true;

		[SerializeField]
		private UIButton _expandButton;

		[SerializeField]
		private UIButton _foldButton;

		[SerializeField]
		private GameObject _elementToFold;

		[SerializeField]
		private UIWidgetContainer _layout;

		public GameObject elementToFold
		{
			get
			{
				return _elementToFold;
			}
			set
			{
				_elementToFold = value;
			}
		}

		public UIWidgetContainer layout
		{
			set
			{
				_layout = value;
			}
		}

		public GenericExpanderView()
			: this()
		{
		}

		private void Awake()
		{
			LayoutUtility.CheckIsLayout(_layout);
		}

		private unsafe void Start()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			UpdateVisibleExpanding();
			EventDelegate.Add(_expandButton.onClick, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			EventDelegate.Add(_foldButton.onClick, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleOnClick()
		{
			_expanded = !_expanded;
			UpdateVisibleExpanding();
		}

		private void UpdateVisibleExpanding()
		{
			_expandButton.get_gameObject().SetActive(!_expanded);
			_foldButton.get_gameObject().SetActive(_expanded);
			_elementToFold.SetActive(_expanded);
			Layout();
		}

		private void Layout()
		{
			LayoutUtility.ScheduleReposition(_layout);
		}
	}
}
