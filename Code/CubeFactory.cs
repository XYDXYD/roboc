using Simulation.Hardware.Weapons;
using Svelto.Factories;
using Svelto.IoC;
using UnityEngine;

internal abstract class CubeFactory : ICubeFactory
{
	[Inject]
	internal ICubeList cubeList
	{
		get;
		set;
	}

	[Inject]
	internal IGameObjectFactory gameObjectFactory
	{
		get;
		set;
	}

	protected abstract bool isEditor
	{
		get;
	}

	public GameObject BuildCube(CubeTypeID ID, Vector3 position, Quaternion rotation, TargetType targetType)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = BuildCube(ID, isDummy: false);
		Transform transform = val.get_transform();
		transform.set_position(position);
		transform.set_rotation(rotation);
		val.set_name(ID.ID.ToString());
		return val;
	}

	public GameObject BuildCube(CubeTypeID ID)
	{
		return BuildCube(ID, isDummy: false);
	}

	protected GameObject BuildCube(CubeTypeID ID, bool isDummy)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		CubeTypeData cubeTypeData = cubeList.CubeTypeDataOf(ID);
		CubeInstance component = cubeTypeData.prefab.GetComponent<CubeInstance>();
		GameObject val = isDummy ? Object.Instantiate<GameObject>((!isEditor) ? component.simulationCube : component.editorCube) : gameObjectFactory.Build((!isEditor) ? component.simulationCube : component.editorCube);
		val.get_transform().set_parent(MachineBoard.Instance.board);
		val.get_transform().set_localPosition(Vector3.get_zero());
		CubeInstance cubeInstance = val.AddComponent<CubeInstance>();
		cubeInstance.Init(component);
		return val;
	}
}
