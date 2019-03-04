namespace Simulation.Hardware
{
	internal class MachineOwnerImplementor : IMachineOwnerComponent, IOwnerTeamComponent
	{
		private bool _ownedByMe;

		private bool _ownedByAi;

		private int _ownerId;

		private int _ownerMachineId;

		bool IMachineOwnerComponent.ownedByMe
		{
			get
			{
				return _ownedByMe;
			}
			set
			{
				_ownedByMe = value;
			}
		}

		bool IMachineOwnerComponent.ownedByAi
		{
			get
			{
				return _ownedByAi;
			}
			set
			{
				_ownedByAi = value;
			}
		}

		int IMachineOwnerComponent.ownerId
		{
			get
			{
				return _ownerId;
			}
			set
			{
				_ownerId = value;
			}
		}

		int IMachineOwnerComponent.ownerMachineId
		{
			get
			{
				return _ownerMachineId;
			}
			set
			{
				_ownerMachineId = value;
			}
		}

		public int ownerTeamId
		{
			get;
			set;
		}

		public void SetOwnedByMe(bool ownedByMe_)
		{
			_ownedByMe = ownedByMe_;
		}

		public void SetOwnedByAi(bool ownedByAi_)
		{
			_ownedByAi = ownedByAi_;
		}

		public void SetOwner(int ownerId, int ownerMachineId)
		{
			_ownerId = ownerId;
			_ownerMachineId = ownerMachineId;
		}
	}
}
