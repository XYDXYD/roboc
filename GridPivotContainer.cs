using UnityEngine;

internal class GridPivotContainer : MonoBehaviour
{
	[SerializeField]
	private Transform gridPivot;

	[SerializeField]
	private bool isLateral;

	public GridPivotContainer()
		: this()
	{
	}

	public Transform GetGridPivot()
	{
		return gridPivot;
	}

	public bool GetIsLateral()
	{
		return isLateral;
	}
}
