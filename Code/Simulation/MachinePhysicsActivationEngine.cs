using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Observer;
using System;

namespace Simulation
{
	internal sealed class MachinePhysicsActivationEngine : SingleEntityViewEngine<MachinePhysicsActivationEntityView>, IQueryingEntityViewEngine, IWaitForFrameworkInitialization, IWaitForFrameworkDestruction, IEngine
	{
		private readonly AllowMovementObserver _allowMovementObserver;

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public MachinePhysicsActivationEngine(AllowMovementObserver allowMovementObserver)
		{
			_allowMovementObserver = allowMovementObserver;
		}

		public unsafe void OnFrameworkInitialized()
		{
			_allowMovementObserver.AddAction(new ObserverAction<bool>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public unsafe void OnFrameworkDestroyed()
		{
			_allowMovementObserver.RemoveAction(new ObserverAction<bool>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		protected override void Add(MachinePhysicsActivationEntityView entityView)
		{
			entityView.spawnableComponent.isSpawning.NotifyOnValueSet((Action<int, bool>)OnSpawningChanged);
		}

		protected override void Remove(MachinePhysicsActivationEntityView entityView)
		{
			entityView.spawnableComponent.isSpawning.StopNotify((Action<int, bool>)OnSpawningChanged);
		}

		private void OnMovementAllowedChanged(ref bool allowMovement)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			GameMovementAllowedEntityView gameEntityView = GetGameEntityView();
			gameEntityView.gameStartedComponent.hasGameStarted = allowMovement;
			FasterListEnumerator<MachinePhysicsActivationEntityView> enumerator = entityViewsDB.QueryEntityViews<MachinePhysicsActivationEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					MachinePhysicsActivationEntityView current = enumerator.get_Current();
					UpdateMachine(current, allowMovement);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		private void OnSpawningChanged(int entityId, bool dontcare)
		{
			GameMovementAllowedEntityView gameEntityView = GetGameEntityView();
			MachinePhysicsActivationEntityView machineEntityView = entityViewsDB.QueryEntityView<MachinePhysicsActivationEntityView>(entityId);
			UpdateMachine(machineEntityView, gameEntityView.gameStartedComponent.hasGameStarted);
		}

		private static void UpdateMachine(MachinePhysicsActivationEntityView machineEntityView, bool hasGameStarted)
		{
			machineEntityView.rigidbodyComponent.rb.set_isKinematic(!hasGameStarted || machineEntityView.spawnableComponent.isSpawning.get_value());
		}

		private GameMovementAllowedEntityView GetGameEntityView()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return entityViewsDB.QueryEntityViews<GameMovementAllowedEntityView>().get_Item(0);
		}

		public void Ready()
		{
		}
	}
}
