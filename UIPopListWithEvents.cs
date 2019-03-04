using System.Collections.Generic;
using UnityEngine;

public class UIPopListWithEvents : UIPopupList
{
	public List<EventDelegate> onListOpen = new List<EventDelegate>();

	public List<EventDelegate> onListClose = new List<EventDelegate>();

	public List<EventDelegate> onItemSelected = new List<EventDelegate>();

	public UIPopListWithEvents()
		: this()
	{
	}

	public override void CloseSelf()
	{
		this.CloseSelf();
		if (EventDelegate.IsValid(onListClose))
		{
			EventDelegate.Execute(onListClose);
		}
	}

	public override void Show()
	{
		this.Show();
		if (EventDelegate.IsValid(onListOpen))
		{
			EventDelegate.Execute(onListOpen);
		}
	}

	protected override void OnItemPress(GameObject go, bool isPressed)
	{
		this.OnItemPress(go, isPressed);
		if (EventDelegate.IsValid(onItemSelected))
		{
			EventDelegate.Execute(onItemSelected);
		}
	}
}
