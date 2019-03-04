using Svelto.IoC;
using UnityEngine;

internal sealed class CursorModeObjectToggler : MonoBehaviour, IInitialize
{
	[Inject]
	public ICursorMode cursorMode
	{
		private get;
		set;
	}

	public CursorModeObjectToggler()
		: this()
	{
	}

	void IInitialize.OnDependenciesInjected()
	{
		cursorMode.OnSwitch += ToggleObject;
	}

	private void OnDestroy()
	{
		cursorMode.OnSwitch -= ToggleObject;
	}

	private void OnEnable()
	{
		if (cursorMode != null && cursorMode.currentMode == Mode.Free)
		{
			this.get_gameObject().SetActive(false);
		}
	}

	public void ToggleObject(Mode cursorMode)
	{
		if (cursorMode == Mode.Lock)
		{
			this.get_gameObject().SetActive(true);
		}
		else
		{
			this.get_gameObject().SetActive(false);
		}
	}
}
