using System;

internal class UIInputWithFocusEvents : UIInput
{
	[NonSerialized]
	public bool disableTab;

	public event Action OnInputGetFocus = delegate
	{
	};

	public event Action OnInputLoseFocus = delegate
	{
	};

	public UIInputWithFocusEvents()
		: this()
	{
	}

	public void Select()
	{
		this.set_isSelected(true);
	}

	public void Deselect()
	{
		this.set_isSelected(false);
	}

	protected override void OnSelect(bool isSelected_)
	{
		bool flag = this.get_isSelected() != isSelected_;
		this.OnSelect(isSelected_);
		if (flag)
		{
			if (isSelected_)
			{
				this.OnInputGetFocus();
			}
			else
			{
				this.OnInputLoseFocus();
			}
		}
	}

	protected override void Update()
	{
		if (!disableTab || !UICamera.GetKeyUp.Invoke(9))
		{
			this.Update();
		}
	}
}
