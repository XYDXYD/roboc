using System;

[Serializable]
internal class CubeFaces
{
	public bool up;

	public bool down;

	public bool left;

	public bool right;

	public bool back;

	public bool front;

	public CubeFaces(bool up_, bool down_, bool left_, bool right_, bool back_, bool front_)
	{
		up = up_;
		down = down_;
		left = left_;
		right = right_;
		back = back_;
		front = front_;
	}
}
