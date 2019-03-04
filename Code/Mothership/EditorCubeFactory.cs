using Simulation.Hardware.Weapons;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Mothership
{
	internal sealed class EditorCubeFactory : CubeFactory, IEditorCubeFactory, ICubeFactory
	{
		[Inject]
		internal GameObjectPool objectPool
		{
			get;
			set;
		}

		protected override bool isEditor => true;

		public new GameObject BuildCube(CubeTypeID ID, Vector3 position, Quaternion rotation, TargetType targetType)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			GameObject pooledCubeInMothership = GetPooledCubeInMothership(ID);
			Transform transform = pooledCubeInMothership.get_transform();
			transform.set_localPosition(position);
			transform.set_rotation(rotation);
			return pooledCubeInMothership;
		}

		public GameObject BuildDummyCube(CubeTypeID ID, bool onGrid)
		{
			GameObject val = BuildCube(ID, isDummy: true);
			DestroyMarkedComponents(val, onGrid);
			SwitchToGhostCubeColliders(val);
			return val;
		}

		private void DestroyMarkedComponents(GameObject go, bool onGrid)
		{
			DestroyOnDummyCube[] componentsInChildren = go.GetComponentsInChildren<DestroyOnDummyCube>();
			DestroyOnDummyCube[] array = componentsInChildren;
			foreach (DestroyOnDummyCube destroyOnDummyCube in array)
			{
				if (!onGrid)
				{
					Object.DestroyImmediate(destroyOnDummyCube.get_gameObject());
				}
			}
		}

		private void SwitchToGhostCubeColliders(GameObject go)
		{
			GhostCubeSpecificCollision componentInChildren = go.GetComponentInChildren<GhostCubeSpecificCollision>();
			if (componentInChildren != null)
			{
				componentInChildren.SwitchToGhostCubeColliders();
			}
		}

		private GameObject GetPooledCubeInMothership(CubeTypeID ID)
		{
			string text = ID.ID.ToString();
			GameObject val = objectPool.Use(text, (Func<GameObject>)(() => CreateNewGameObject(ID)));
			val.SetActive(true);
			val.set_name(text);
			return val;
		}

		private GameObject CreateNewGameObject(CubeTypeID ID)
		{
			CubeTypeData cubeTypeData = base.cubeList.CubeTypeDataOf(ID);
			CubeInstance component = cubeTypeData.prefab.GetComponent<CubeInstance>();
			GameObject val = base.gameObjectFactory.Build(component.editorCube);
			CubeInstance cubeInstance = val.AddComponent<CubeInstance>();
			cubeInstance.Init(component);
			val.AddComponent<CubeColorUpdater>();
			val.get_transform().SetParent(MachineBoard.Instance.board, false);
			return val;
		}
	}
}
