using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

public class HoverItemOnEnable : MonoBehaviour
{
	[SerializeField]
	private GameObject itemToHover;

	[Header("Optional Field")]
	[SerializeField]
	private GameObject itemToHoverOnDisable;

	public HoverItemOnEnable()
		: this()
	{
	}

	private void Awake()
	{
		if (itemToHover == null)
		{
			RemoteLogger.Error(new Exception("This item does not have a button to hover will make the dialogue loose focus OnEnable! " + this.get_transform().get_name()));
		}
	}

	private void OnEnable()
	{
		TaskRunner.get_Instance().Run(HoverOnEnable(itemToHover));
	}

	private void OnDisable()
	{
		TaskRunner.get_Instance().Run(HoverOnEnable(itemToHoverOnDisable));
	}

	private IEnumerator HoverOnEnable(GameObject item)
	{
		yield return null;
		if (item != null)
		{
			UICamera.set_currentScheme(2);
			UICamera.set_controllerNavigationObject(item);
		}
	}
}
