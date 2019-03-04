using UnityEngine;

public class OnSpawn_Unparent : MonoBehaviour
{
	public OnSpawn_Unparent()
		: this()
	{
	}

	private void Start()
	{
		this.get_transform().set_parent(null);
	}
}
