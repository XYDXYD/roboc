using Mothership;
using Simulation.Hardware.Weapons;
using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class GhostCubeCollisionCheck : MonoBehaviour
{
	private IMachineMap _machineMap;

	private HashSet<GameObject> _collidingObjects = new HashSet<GameObject>();

	private int _gridFloorLayer;

	private int _ghostCubeLayer;

	private int _ghostCollisionCount;

	private bool _justEnabled;

	private Dictionary<GameObject, int> _collisionsPerObject = new Dictionary<GameObject, int>();

	internal CubeCaster cubeCaster
	{
		private get;
		set;
	}

	internal bool IsCollidingCubes => _collidingObjects.Count > 0 || _justEnabled;

	internal bool IsCollidingFloor
	{
		get;
		private set;
	}

	internal bool IsCollidingGhost => _ghostCollisionCount > 0;

	internal IMachineMap machineMap
	{
		set
		{
			_machineMap = value;
			_machineMap.OnRemoveCubeAt += HandleOnRemoveCubeAt;
		}
	}

	internal MachineColorUpdater colorUpdater
	{
		get;
		set;
	}

	public GhostCubeCollisionCheck()
		: this()
	{
	}

	private void Start()
	{
		_gridFloorLayer = GameLayers.CUBE_FLOOR_LAYER;
		_ghostCubeLayer = GameLayers.GHOST_CUBE;
		Collider[] componentsInChildren = this.GetComponentsInChildren<Collider>(true);
		Collider[] array = componentsInChildren;
		foreach (Collider val in array)
		{
			val.get_gameObject().set_layer(_ghostCubeLayer);
			if (val is MeshCollider)
			{
				(val as MeshCollider).set_convex(true);
			}
			val.set_isTrigger(true);
		}
		if (this.GetComponent<Rigidbody>() == null)
		{
			this.get_gameObject().AddComponent<Rigidbody>();
		}
		Rigidbody[] componentsInChildren2 = this.GetComponentsInChildren<Rigidbody>(true);
		Rigidbody[] array2 = componentsInChildren2;
		foreach (Rigidbody val2 in array2)
		{
			val2.set_isKinematic(true);
			val2.get_gameObject().set_layer(_ghostCubeLayer);
			val2.set_collisionDetectionMode(1);
		}
		ResetGhost();
	}

	public void ResetGhost()
	{
		_ghostCollisionCount = 0;
		cubeCaster.SetGhostIntersects(IsCollidingCubes, IsCollidingFloor, IsCollidingGhost);
	}

	private void OnTriggerEnter(Collider other)
	{
		CheckFloorCollision(other);
		GameObject gameObject = other.get_gameObject();
		if (gameObject.get_layer() == GameLayers.GHOST_CUBE)
		{
			_ghostCollisionCount++;
		}
		else if (gameObject.get_layer() != GameLayers.CUBE_FLOOR_LAYER)
		{
			RegisterCollision(other.get_attachedRigidbody().get_gameObject());
		}
		cubeCaster.SetGhostIntersects(IsCollidingCubes, IsCollidingFloor, IsCollidingGhost);
	}

	private void OnTriggerStay(Collider other)
	{
		CheckFloorCollision(other);
	}

	private void OnTriggerExit(Collider other)
	{
		GameObject gameObject = other.get_gameObject();
		if (gameObject.get_layer() == GameLayers.GHOST_CUBE)
		{
			_ghostCollisionCount--;
		}
		else if (gameObject.get_layer() != GameLayers.CUBE_FLOOR_LAYER)
		{
			UnregisterCollision(other.get_attachedRigidbody().get_gameObject());
		}
		else
		{
			IsCollidingFloor = false;
		}
		cubeCaster.SetGhostIntersects(IsCollidingCubes, IsCollidingFloor, IsCollidingGhost);
	}

	private void CheckFloorCollision(Collider other)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (other.get_gameObject().get_layer() != _gridFloorLayer)
		{
			return;
		}
		IsCollidingFloor = true;
		float num = 0f;
		Bounds bounds = other.get_bounds();
		Vector3 min = bounds.get_min();
		float y = min.y;
		Bounds bounds2 = other.get_bounds();
		Vector3 max = bounds2.get_max();
		float num2 = Mathf.Min(y, max.y);
		Transform component = this.GetComponent<Transform>();
		if (component != null)
		{
			Collider[] componentsInChildren = component.GetComponentsInChildren<Collider>();
			Collider[] array = componentsInChildren;
			foreach (Collider val in array)
			{
				Bounds bounds3 = val.get_bounds();
				Vector3 min2 = bounds3.get_min();
				float y2 = min2.y;
				if (y2 < num2)
				{
					num2 = y2;
				}
				Bounds bounds4 = val.get_bounds();
				min2 = bounds4.get_max();
				y2 = min2.y;
				if (y2 < num2)
				{
					num2 = y2;
				}
			}
		}
		float val2 = num - num2;
		float num3 = GridScaleUtility.InverseWorldScale(val2, TargetType.Player) * Mathf.Sign(num2);
		cubeCaster.SetRequiredDisplacement(new Int3(0, (int)Math.Floor(num3), 0));
	}

	private void HandleOnRemoveCubeAt(Byte3 arg1, MachineCell cell)
	{
		if (_collidingObjects.Remove(cell.gameObject))
		{
			colorUpdater.MarkAsValid(cell.gameObject, isValid: true);
			_collisionsPerObject.Remove(cell.gameObject);
			cubeCaster.SetGhostIntersects(IsCollidingCubes, IsCollidingFloor, IsCollidingGhost);
		}
	}

	private void RegisterCollision(GameObject gameObject)
	{
		if (!_collidingObjects.Contains(gameObject))
		{
			_collisionsPerObject[gameObject] = 1;
			_collidingObjects.Add(gameObject);
			colorUpdater.MarkAsValid(gameObject, isValid: false);
		}
		else
		{
			Dictionary<GameObject, int> collisionsPerObject;
			GameObject key;
			(collisionsPerObject = _collisionsPerObject)[key = gameObject] = collisionsPerObject[key] + 1;
		}
	}

	private void UnregisterCollision(GameObject gameObject)
	{
		if (_collidingObjects.Contains(gameObject))
		{
			Dictionary<GameObject, int> collisionsPerObject;
			GameObject key;
			(collisionsPerObject = _collisionsPerObject)[key = gameObject] = collisionsPerObject[key] - 1;
			if (_collisionsPerObject[gameObject] == 0)
			{
				_collidingObjects.Remove(gameObject);
				colorUpdater.MarkAsValid(gameObject, isValid: true);
				_collisionsPerObject.Remove(gameObject);
			}
		}
	}

	private void ClearCollidingObjects()
	{
		foreach (GameObject collidingObject in _collidingObjects)
		{
			if (collidingObject != null)
			{
				colorUpdater.MarkAsValid(collidingObject, isValid: true);
			}
		}
		_collidingObjects.Clear();
		_collisionsPerObject.Clear();
	}

	private void OnDisable()
	{
		ClearCollidingObjects();
		_justEnabled = false;
		IsCollidingFloor = false;
		_ghostCollisionCount = 0;
		cubeCaster.SetGhostIntersects(IsCollidingCubes, IsCollidingFloor, IsCollidingGhost);
	}

	private void OnEnable()
	{
		_justEnabled = true;
	}

	private void Update()
	{
		if (_justEnabled)
		{
			_justEnabled = false;
			cubeCaster.SetGhostIntersects(IsCollidingCubes, IsCollidingFloor, IsCollidingGhost);
		}
	}

	private void OnDestroy()
	{
		_machineMap.OnRemoveCubeAt -= HandleOnRemoveCubeAt;
	}
}
