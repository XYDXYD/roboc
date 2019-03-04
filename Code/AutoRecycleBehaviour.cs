using UnityEngine;

public abstract class AutoRecycleBehaviour<Recycled> : MonoBehaviour
{
	protected Recycled _object;

	protected IObjectPool<Recycled> _pool;

	protected int _id = -1;

	protected string _poolName;

	protected AutoRecycleBehaviour()
		: this()
	{
	}

	protected virtual void Awake()
	{
	}

	public void SetPool(IObjectPool<Recycled> pool, int poolID)
	{
		_pool = pool;
		_id = poolID;
	}

	public void SetPool(IObjectPool<Recycled> pool, string poolName)
	{
		_pool = pool;
		_poolName = poolName;
	}

	public void SetRecycledComponent(Recycled componentToRecycle)
	{
		_object = componentToRecycle;
	}

	protected virtual void OnDisable()
	{
		if (_id != -1)
		{
			_pool.Recycle(_object, _id);
		}
		else
		{
			_pool.Recycle(_object, _poolName);
		}
	}
}
internal class AutoRecycleBehaviour : AutoRecycleBehaviour<GameObject>
{
	private new void OnDisable()
	{
		if (_id != -1)
		{
			_pool.Recycle(this.get_gameObject(), _id);
		}
		else
		{
			_pool.Recycle(this.get_gameObject(), this.get_gameObject().get_name());
		}
		this.get_gameObject().SetActive(false);
	}
}
