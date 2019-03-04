using UnityEngine;

internal sealed class CubeCounter
{
	private int _cubeCount;

	private MeshRenderer _renderer;

	public CubeCounter(int cubeTotal, MeshRenderer renderer)
	{
		_cubeCount = cubeTotal;
		_renderer = renderer;
	}

	public void RemoveCube()
	{
		if (_cubeCount > 0)
		{
			_cubeCount--;
			if (_cubeCount == 0 && _renderer != null)
			{
				_renderer.set_enabled(false);
			}
		}
	}
}
