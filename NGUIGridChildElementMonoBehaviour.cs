using UnityEngine;

internal class NGUIGridChildElementMonoBehaviour : MonoBehaviour
{
	private UIGrid _grid;

	public NGUIGridChildElementMonoBehaviour()
		: this()
	{
	}

	public void Awake()
	{
		if (_grid == null)
		{
			Transform parent = this.get_gameObject().get_transform().get_parent();
			_grid = parent.get_gameObject().GetComponent<UIGrid>();
		}
		_grid.set_repositionNow(true);
	}
}
