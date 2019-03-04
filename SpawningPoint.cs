using UnityEngine;

public sealed class SpawningPoint : MonoBehaviour
{
	public SpawningPoint()
		: this()
	{
	}

	private void OnDrawGizmos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_color(Color.get_yellow());
		Gizmos.DrawWireSphere(this.get_transform().get_position(), 5.4f);
	}
}
