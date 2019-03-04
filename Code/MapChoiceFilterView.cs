using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

[RequireComponent(typeof(UIPopupList))]
internal class MapChoiceFilterView : MonoBehaviour, IChainListener, IInitialize
{
	public UILabel popupListLabel;

	[SerializeField]
	private GameObject TargetWithButtonsToDisable;

	private UIButton[] _buttonsToDisable;

	private UIPopupList _popupList;

	private BubbleSignal<IChainRoot> _bubble;

	[Inject]
	internal MapChoiceFilterPresenter mapChoicePresenter
	{
		private get;
		set;
	}

	public UIPopupList popupList
	{
		get
		{
			if (_popupList == null)
			{
				_popupList = this.GetComponent<UIPopupList>();
			}
			return _popupList;
		}
	}

	public MapChoiceFilterView()
		: this()
	{
	}

	unsafe void IInitialize.OnDependenciesInjected()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		EventDelegate.Add(popupList.onChange, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		_bubble = new BubbleSignal<IChainRoot>(this.get_transform());
		_buttonsToDisable = TargetWithButtonsToDisable.GetComponents<UIButton>();
		mapChoicePresenter.SetView(this);
	}

	public void SetDropDownAvailable(bool setting)
	{
		popupList.set_enabled(setting);
		if (!setting)
		{
			UIButton[] buttonsToDisable = _buttonsToDisable;
			foreach (UIButton val in buttonsToDisable)
			{
				val.SetState(3, true);
				val.set_isEnabled(false);
			}
		}
		else
		{
			UIButton[] buttonsToDisable2 = _buttonsToDisable;
			foreach (UIButton val2 in buttonsToDisable2)
			{
				val2.set_isEnabled(true);
				val2.SetState(0, true);
			}
		}
	}

	public void Listen(object message)
	{
		if (message is CustomGameGUIEvent)
		{
			mapChoicePresenter.OnGUIEvent((CustomGameGUIEvent)message);
		}
	}

	public void Dispatch(CustomGameGUIEvent.Type eventType, object arg = null)
	{
		_bubble.Dispatch<CustomGameGUIEvent>(new CustomGameGUIEvent(eventType, arg));
	}

	private void OnItemSelected()
	{
		mapChoicePresenter.OnItemSelected();
	}
}
