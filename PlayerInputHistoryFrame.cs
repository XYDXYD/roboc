internal sealed class PlayerInputHistoryFrame : PlayerHistoryFrame, IMachineControl
{
	public float horizontalAxis
	{
		get;
		set;
	}

	public float forwardAxis
	{
		get;
		set;
	}

	public bool moveUpwards
	{
		get;
		set;
	}

	public bool moveDown
	{
		get;
		set;
	}

	public float mouseX
	{
		get;
		set;
	}

	public float mouseY
	{
		get;
		set;
	}

	public float fire1
	{
		get;
		set;
	}

	public float fire2
	{
		get;
		set;
	}

	public bool pulseAR
	{
		get;
		set;
	}

	public bool toggleLight
	{
		get;
		set;
	}

	public bool strafeLeft
	{
		get;
		set;
	}

	public bool strafeRight
	{
		get;
		set;
	}

	public bool taunt
	{
		get;
		set;
	}
}
