using Svelto.IoC;
using System;
using UnityEngine;

internal class PopupMessageData : MonoBehaviour
{
	public PopupMessageInfo[] data;

	[Inject]
	internal PopupMessagePresenter presenter
	{
		private get;
		set;
	}

	public PopupMessageData()
		: this()
	{
	}

	private void Start()
	{
		if (presenter == null)
		{
			return;
		}
		PopupMessageInfo[] array = data;
		foreach (PopupMessageInfo popupMessageInfo in array)
		{
			for (int j = 0; j < popupMessageInfo.stringOverridesByCubeType.Length; j++)
			{
				popupMessageInfo.stringOverridesByCubeType[j].stringKeyAsCubeTypeID = Convert.ToUInt32(popupMessageInfo.stringOverridesByCubeType[j].cubeTypeInHexFormat, 16);
			}
		}
		presenter.RegisterData(data);
	}
}
