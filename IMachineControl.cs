internal interface IMachineControl
{
	float horizontalAxis
	{
		get;
	}

	float forwardAxis
	{
		get;
	}

	bool moveUpwards
	{
		get;
	}

	bool moveDown
	{
		get;
	}

	float mouseX
	{
		get;
	}

	float mouseY
	{
		get;
	}

	float fire1
	{
		get;
	}

	float fire2
	{
		get;
	}

	bool pulseAR
	{
		get;
		set;
	}

	bool strafeLeft
	{
		get;
	}

	bool strafeRight
	{
		get;
	}

	bool taunt
	{
		get;
	}
}
