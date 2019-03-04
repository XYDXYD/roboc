using UnityEngine;

public abstract class AutoRecycleMonoBehaviour : AutoRecycleBehaviour<MonoBehaviour>
{
	protected override void OnDisable()
	{
		base.OnDisable();
		_object.get_gameObject().SetActive(false);
	}
}
