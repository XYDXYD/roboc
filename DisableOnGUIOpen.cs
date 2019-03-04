using Svelto.IoC;
using UnityEngine;

internal sealed class DisableOnGUIOpen : MonoBehaviour
{
	[Inject]
	public ICursorMode cursorMode
	{
		private get;
		set;
	}

	public DisableOnGUIOpen()
		: this()
	{
	}

	private void Start()
	{
		cursorMode.OnSwitch += OnCursorModeChanged;
	}

	private void OnDestroy()
	{
		if (cursorMode != null)
		{
			cursorMode.OnSwitch -= OnCursorModeChanged;
		}
	}

	private void OnCursorModeChanged(Mode mode)
	{
		MonoBehaviour[] componentsInChildren = this.GetComponentsInChildren<MonoBehaviour>(true);
		MonoBehaviour[] array = componentsInChildren;
		foreach (MonoBehaviour val in array)
		{
			if (val is IHandleGUIDisabler)
			{
				IHandleGUIDisabler handleGUIDisabler = val as IHandleGUIDisabler;
				if (mode == Mode.Lock)
				{
					handleGUIDisabler.OnGUIEnable();
				}
				else
				{
					handleGUIDisabler.OnGUIDisable();
				}
			}
			if (val.get_tag() != "AvatarCamera" && val.get_tag() != "MainCamera")
			{
				val.set_enabled(mode == Mode.Lock);
			}
		}
	}
}
