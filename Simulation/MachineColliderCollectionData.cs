using Svelto.DataStructures;
using UnityEngine;

namespace Simulation
{
	internal class MachineColliderCollectionData
	{
		public FasterList<Collider> NewColliders
		{
			get;
			private set;
		}

		public FasterList<Collider> RemovedColliders
		{
			get;
			private set;
		}

		public int MachineId
		{
			get;
			private set;
		}

		public MachineColliderCollectionData()
			: this(-1)
		{
		}

		public MachineColliderCollectionData(int machineId)
		{
			NewColliders = new FasterList<Collider>();
			RemovedColliders = new FasterList<Collider>();
			MachineId = machineId;
		}

		public void ResetData(int newMachineId)
		{
			NewColliders.Clear();
			RemovedColliders.Clear();
			MachineId = newMachineId;
		}
	}
}
