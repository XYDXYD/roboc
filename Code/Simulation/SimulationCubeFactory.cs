using Simulation.Hardware.Weapons;
using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal sealed class SimulationCubeFactory : CubeFactory, ICubeFactory
	{
		private static CubeTypeID _ID;

		[Inject]
		internal GameObjectPool objectPool
		{
			get;
			set;
		}

		protected override bool isEditor => false;

		public new GameObject BuildCube(CubeTypeID ID, Vector3 position, Quaternion rotation, TargetType targetType)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			GameObject pooledCubeInSimulation = GetPooledCubeInSimulation(ID);
			Transform transform = pooledCubeInSimulation.get_transform();
			transform.set_position(position);
			transform.set_rotation(rotation);
			return pooledCubeInSimulation;
		}

		private GameObject GetPooledCubeInSimulation(CubeTypeID ID)
		{
			string name = ID.ID.ToString();
			_ID = ID;
			GameObject val = BuildCube();
			val.SetActive(true);
			val.set_name(name);
			return val;
		}

		private GameObject BuildCube()
		{
			return BuildCube(_ID);
		}
	}
}
