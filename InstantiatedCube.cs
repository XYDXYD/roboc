using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class InstantiatedCube
{
	public float mass = 5f;

	public float physcisMassScalar = 1f;

	public float drag;

	public float angularDrag = 0.04f;

	public float cubeBuff = 1f;

	public int health = 1;

	public int healingAmount;

	public int pendingDamage;

	public int pendingHealing;

	public int lastHealingApplied;

	public int lastDamageApplied;

	public Byte3 gridPos;

	public bool customCOM;

	public Vector3 comOffset;

	public List<Byte3> boundsOccupiedCells = new List<Byte3>();

	public int rotationIndex;

	public string name;

	public Color colour = Color.get_white();

	public bool isConnected;

	public byte isRed;

	public CubeColliderInfo[] colliderInfo;

	public PaletteColor paletteColor;

	public byte paletteIndex;

	public PaletteColor previousPaletteColor;

	public byte previousPaletteIndex;

	private int _initialTotalHealth;

	public bool isDestroyed => health <= 0;

	public int initialTotalHealth
	{
		get
		{
			return _initialTotalHealth;
		}
		set
		{
			if (value == 0)
			{
				throw new Exception("Cannot set initial total health to zero");
			}
			_initialTotalHealth = value;
		}
	}

	public int totalHealth => Mathf.CeilToInt((float)initialTotalHealth * cubeBuff);

	public PersistentCubeData persistentCubeData
	{
		get;
		private set;
	}

	public CubeNodeInstance cubeNodeInstance
	{
		get;
		private set;
	}

	public InstantiatedCube(CubeInstance instance, CubeNodeInstance nodeInstance, PersistentCubeData persistentData, Byte3 _gridPos, int _rotationIndex)
		: this(instance, persistentData, _gridPos, _rotationIndex)
	{
		cubeNodeInstance = nodeInstance;
	}

	public InstantiatedCube(CubeInstance instance, PersistentCubeData persistentData, Byte3 _gridPos, int _rotationIndex)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		persistentCubeData = persistentData;
		mass = instance.mass;
		physcisMassScalar = instance.physicsMassScalar;
		drag = instance.drag;
		angularDrag = instance.angularDrag;
		customCOM = instance.customCOM;
		comOffset = instance.comOffset;
		gridPos = _gridPos;
		rotationIndex = _rotationIndex;
		name = instance.get_name();
	}

	public InstantiatedCube(CubeNodeInstance nodeInstance, PersistentCubeData persistentData, Byte3 _gridPos, int _rotationIndex)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		cubeNodeInstance = nodeInstance;
		persistentCubeData = persistentData;
		gridPos = _gridPos;
		rotationIndex = _rotationIndex;
	}

	public void SetParams(CubeInstance instance)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		mass = instance.mass;
		physcisMassScalar = instance.physicsMassScalar;
		drag = instance.drag;
		angularDrag = instance.angularDrag;
		name = instance.get_name();
		customCOM = instance.customCOM;
		comOffset = instance.comOffset;
	}

	public override bool Equals(object obj)
	{
		InstantiatedCube instantiatedCube = obj as InstantiatedCube;
		if ((object)instantiatedCube == null)
		{
			return false;
		}
		return gridPos == instantiatedCube.gridPos;
	}

	public bool Equals(InstantiatedCube p)
	{
		return gridPos == p.gridPos;
	}

	public override int GetHashCode()
	{
		return gridPos.GetHashCode();
	}

	public bool IsFaceSelectable(CubeFace face)
	{
		return persistentCubeData.IsFaceSelectable(face);
	}

	public bool IsDirectionSelectable(Quaternion quaternion, Vector3 direction, Vector3 offset)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return persistentCubeData.IsDirectionSelectable(quaternion, direction, offset);
	}

	public static bool operator ==(InstantiatedCube a, InstantiatedCube b)
	{
		if (object.ReferenceEquals(a, b))
		{
			return true;
		}
		if ((object)a == null || (object)b == null)
		{
			return false;
		}
		return a.gridPos == b.gridPos;
	}

	public static bool operator !=(InstantiatedCube a, InstantiatedCube b)
	{
		return !(a == b);
	}
}
