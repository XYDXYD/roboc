internal sealed class LatestState
{
	public float horizontal;

	public float vertical;

	public bool jump;

	public bool crouch;

	public bool toggleLights;

	public bool strafeRight;

	public bool strafeLeft;

	public LatestState(float h, float v, bool j, bool c, bool sl, bool sr)
	{
		horizontal = h;
		vertical = v;
		jump = j;
		crouch = c;
		strafeLeft = sl;
		strafeRight = sr;
	}

	public LatestState()
	{
	}
}
