using System;

[Serializable]
internal class CubeOccludingFaces
{
	public bool up = true;

	public bool down = true;

	public bool left = true;

	public bool right = true;

	public bool back = true;

	public bool front = true;

	public CubeOccludingFaces(bool up_, bool down_, bool left_, bool right_, bool back_, bool front_)
	{
		up = up_;
		down = down_;
		left = left_;
		right = right_;
		back = back_;
		front = front_;
	}
}
