using Simulation;
using Svelto.IoC;
using UnityEngine;

public class AutoRecycleIntroBeamsOnDisable : MonoBehaviour
{
	private EmpTargetingLocatorMonoBehaviour _objectToRecycle;

	private bool _canBeRecycled;

	[Inject]
	internal EmpTargetingLocatorPool pool
	{
		get;
		set;
	}

	public AutoRecycleIntroBeamsOnDisable()
		: this()
	{
	}

	private void Awake()
	{
		_objectToRecycle = this.GetComponent<EmpTargetingLocatorMonoBehaviour>();
	}

	private void Start()
	{
		_canBeRecycled = true;
	}

	private void OnDisable()
	{
		if (_canBeRecycled)
		{
			pool.Recycle(_objectToRecycle, this.get_name());
			_canBeRecycled = false;
		}
	}
}
