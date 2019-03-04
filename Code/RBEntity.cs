using UnityEngine;

internal sealed class RBEntity
{
	public Rigidbody rigidBody;

	public GameObject board;

	public RBEntity(GameObject board)
	{
		this.board = board;
	}
}
