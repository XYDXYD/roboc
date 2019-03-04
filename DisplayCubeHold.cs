using Svelto.Context;
using Svelto.IoC;
using System.Collections;
using UnityEngine;

internal sealed class DisplayCubeHold : MonoBehaviour, IInitialize, IWaitForFrameworkDestruction
{
	public Material transparentMaterial;

	public Transform endOfBarrelTransform;

	public BoxCollider cubePositionBox;

	public float maxRotationTime = 0.1f;

	private Renderer[] _cubeRenderers;

	private GameObject _cubeHold;

	private CubeInstance _cubeInstance;

	private Vector3 _cubeCentreOffset = Vector3.get_zero();

	private bool _inventoryAvailable;

	private bool _transparent;

	private float _positionLerpTime;

	private const float C_LOW_ALPHA = 0.25f;

	[Inject]
	internal IEditorCubeFactory cubeFactory
	{
		private get;
		set;
	}

	[Inject]
	internal ICubeHolder selectedCube
	{
		private get;
		set;
	}

	[Inject]
	internal ICubeInventory cubeInventory
	{
		private get;
		set;
	}

	public DisplayCubeHold()
		: this()
	{
	}//IL_000c: Unknown result type (might be due to invalid IL or missing references)
	//IL_0011: Unknown result type (might be due to invalid IL or missing references)


	void IInitialize.OnDependenciesInjected()
	{
		cubeInventory.RegisterInventoryLoadedCallback(OnInventoryLoaded);
	}

	private void Start()
	{
		selectedCube.OnCubeSelectedChanged += ChangeCubeDisplayed;
		selectedCube.OnColorChanged += ChangeColor;
		selectedCube.OnRotationChanged += ChangeCubeRotation;
		ChangeCubeDisplayed(selectedCube.selectedCubeID);
	}

	public void OnFrameworkDestroyed()
	{
		cubeInventory.DeRegisterInventoryLoadedCallback(OnInventoryLoaded);
	}

	private void OnDestroy()
	{
		if (selectedCube != null)
		{
			selectedCube.OnCubeSelectedChanged -= ChangeCubeDisplayed;
			selectedCube.OnColorChanged -= ChangeColor;
			selectedCube.OnRotationChanged -= ChangeCubeRotation;
		}
	}

	private void Update()
	{
		UpdateCubePosition();
	}

	public void UpdateCubePosition()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (_cubeInstance != null)
		{
			Quaternion dummyCubeRotation = GetDummyCubeRotation();
			Vector3 val = dummyCubeRotation * _cubeCentreOffset;
			Vector3 val2 = cubePositionBox.get_transform().get_position() - val;
			if (val2 != _cubeHold.get_transform().get_position())
			{
				_positionLerpTime += Time.get_deltaTime();
				float num = _positionLerpTime / maxRotationTime;
				_cubeHold.get_transform().set_position(Vector3.Lerp(_cubeHold.get_transform().get_position(), val2, num));
			}
			else
			{
				_positionLerpTime = 0f;
			}
		}
	}

	private void ChangeCubeRotation(int newRotation)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (_cubeInstance != null)
		{
			Quaternion dummyCubeRotation = GetDummyCubeRotation();
			if (_cubeHold.get_transform().get_rotation() != dummyCubeRotation)
			{
				this.StartCoroutine(LerpToNewRotation(dummyCubeRotation));
			}
		}
	}

	private IEnumerator LerpToNewRotation(Quaternion newRotation)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		float timer = 0f;
		Quaternion startRotation = _cubeHold.get_transform().get_rotation();
		while (timer < maxRotationTime)
		{
			timer += Time.get_deltaTime();
			float progress = timer / maxRotationTime;
			_cubeHold.get_transform().set_rotation(Quaternion.Lerp(startRotation, newRotation, progress));
			yield return null;
		}
	}

	public Vector3 GetCubeLaunchPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return endOfBarrelTransform.get_position();
	}

	private void ChangeColor(PaletteColor color)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < _cubeRenderers.Length; i++)
		{
			Renderer val = _cubeRenderers[i];
			for (int j = 0; j < val.get_materials().Length; j++)
			{
				Material val2 = val.get_materials()[j];
				Color val3 = Color32.op_Implicit(color.diffuse);
				if (_transparent)
				{
					val3.a = 0.25f;
				}
				val2.SetColor("_Color", val3);
				if (val2.HasProperty("_SpecColor"))
				{
					Color val4 = Color32.op_Implicit(color.specular);
					if (_transparent)
					{
						val4.a = 0.25f;
					}
					val2.SetColor("_SpecColor", val4);
				}
			}
		}
	}

	private void MakeTransparent(bool transparent)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (_cubeHold == null || _transparent == transparent)
		{
			return;
		}
		_transparent = transparent;
		if (_transparent)
		{
			Shader shader = transparentMaterial.get_shader();
			for (int i = 0; i < _cubeRenderers.Length; i++)
			{
				Renderer val = _cubeRenderers[i];
				for (int j = 0; j < val.get_materials().Length; j++)
				{
					Material val2 = val.get_materials()[j];
					val2.set_shader(shader);
					Color color = val2.GetColor("_Color");
					color.a = 0.25f;
					val2.SetColor("_Color", color);
					if (val2.HasProperty("_SpecColor"))
					{
						Color color2 = val2.GetColor("_Color");
						color2.a = 0.25f;
						val2.SetColor("_SpecColor", color2);
					}
				}
			}
		}
		else
		{
			CreateDummyCube(selectedCube.selectedCubeID);
		}
	}

	private void ChangeCubeDisplayed(CubeTypeID cubeID)
	{
		CreateDummyCube(cubeID);
		ChangeCubeRotation(selectedCube.currentRotation);
		if (_inventoryAvailable)
		{
			if (!cubeInventory.IsCubeOwned(cubeID))
			{
				MakeTransparent(transparent: true);
			}
			else
			{
				MakeTransparent(transparent: false);
			}
		}
	}

	private void OnInventoryLoaded()
	{
		_inventoryAvailable = true;
		CubeTypeID selectedCubeID = selectedCube.selectedCubeID;
		if (!cubeInventory.IsCubeOwned(selectedCubeID))
		{
			MakeTransparent(transparent: true);
		}
		else
		{
			MakeTransparent(transparent: false);
		}
	}

	private void CreateDummyCube(CubeTypeID cubeID)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if (_cubeHold != null)
		{
			Object.Destroy(_cubeHold);
		}
		_transparent = false;
		_cubeHold = cubeFactory.BuildDummyCube(cubeID, onGrid: false);
		_cubeHold.get_transform().set_position(Vector3.get_zero());
		_cubeRenderers = _cubeHold.GetComponentsInChildren<Renderer>(true);
		_cubeInstance = _cubeHold.GetComponent<CubeInstance>();
		Bounds cubeBounds = CalculateCubeBounds(_cubeHold);
		float num = CalculateCubeScale(cubePositionBox.get_transform().get_localScale(), cubeBounds);
		_cubeCentreOffset = cubeBounds.get_center() * num;
		Vector3 val = cubePositionBox.get_transform().get_rotation() * _cubeCentreOffset;
		_cubeHold.get_transform().set_parent(this.get_transform());
		_cubeHold.get_transform().set_rotation(GetDummyCubeRotation());
		_cubeHold.get_transform().set_position(cubePositionBox.get_transform().get_position() - val);
		_cubeHold.get_transform().set_localScale(new Vector3(num, num, num));
		RemoveAllCollidersAndRigidBodies();
		InitDummyCubeColor();
		SetDummyCubeLayer();
	}

	private Quaternion GetDummyCubeRotation()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.get_identity();
		val.set_eulerAngles(new Vector3(0f, (float)selectedCube.currentRotation + 90f, 0f));
		val = cubePositionBox.get_transform().get_rotation() * val;
		return val;
	}

	private void RemoveAllCollidersAndRigidBodies()
	{
		Collider[] componentsInChildren = _cubeHold.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Object.Destroy(componentsInChildren[i]);
		}
		Rigidbody[] componentsInChildren2 = _cubeHold.GetComponentsInChildren<Rigidbody>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			Object.Destroy(componentsInChildren2[j]);
		}
	}

	private void InitDummyCubeColor()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < _cubeRenderers.Length; i++)
		{
			Renderer val = _cubeRenderers[i];
			val.set_receiveShadows(false);
			val.set_shadowCastingMode(0);
			for (int j = 0; j < val.get_materials().Length; j++)
			{
				Material val2 = val.get_materials()[j];
				val2.SetColor("_Color", Color32.op_Implicit(selectedCube.currentColor.diffuse));
				if (val2.HasProperty("_SpecColor"))
				{
					val2.SetColor("_SpecColor", Color32.op_Implicit(selectedCube.currentColor.diffuse));
				}
			}
		}
	}

	private Bounds CalculateCubeBounds(GameObject cubeObject)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		Bounds result = default(Bounds);
		for (int i = 0; i < _cubeRenderers.Length; i++)
		{
			if (result.get_max() == Vector3.get_zero() && result.get_min() == Vector3.get_zero())
			{
				result = _cubeRenderers[i].get_bounds();
			}
			result.Encapsulate(_cubeRenderers[i].get_bounds());
		}
		return result;
	}

	private float CalculateCubeScale(Vector3 boundingBoxScale, Bounds cubeBounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		Vector3 size = cubeBounds.get_size();
		float x = size.x;
		Vector3 size2 = cubeBounds.get_size();
		float num;
		if (size2.y > x)
		{
			Vector3 size3 = cubeBounds.get_size();
			num = size3.y;
		}
		else
		{
			num = x;
		}
		x = num;
		Vector3 size4 = cubeBounds.get_size();
		float num2;
		if (size4.z > x)
		{
			Vector3 size5 = cubeBounds.get_size();
			num2 = size5.z;
		}
		else
		{
			num2 = x;
		}
		x = num2;
		Vector3 localScale = cubePositionBox.get_transform().get_localScale();
		return localScale.x / x;
	}

	private void SetDummyCubeLayer()
	{
		GameUtility.SetLayerRecursively(_cubeHold.get_transform(), LayerMask.NameToLayer("DrawLast"));
	}
}
