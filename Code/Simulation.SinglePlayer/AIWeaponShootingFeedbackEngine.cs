using Simulation.Hardware.Weapons;
using Simulation.Hardware.Weapons.Laser;
using Simulation.Hardware.Weapons.Plasma;
using Svelto.ECS;
using System.Collections.Generic;

namespace Simulation.SinglePlayer
{
	internal class AIWeaponShootingFeedbackEngine : MultiEntityViewsEngine<LaserWeaponShootingNode, PlasmaWeaponShootingNode, AIAgentDataComponentsNode>, IQueryingEntityViewEngine, IEngine
	{
		private Dictionary<int, IAIWeaponShootingFeedbackComponent> _weaponFeedbackComponents = new Dictionary<int, IAIWeaponShootingFeedbackComponent>();

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		protected override void Add(LaserWeaponShootingNode obj)
		{
			if (obj.weaponOwner.ownedByAi)
			{
				obj.shootingComponent.shotIsGoingToBeFired.subscribers += HandleLaserWeaponShot;
			}
		}

		protected override void Remove(LaserWeaponShootingNode obj)
		{
			obj.shootingComponent.shotIsGoingToBeFired.subscribers -= HandleLaserWeaponShot;
		}

		protected override void Add(PlasmaWeaponShootingNode obj)
		{
			if (obj.weaponOwner.ownedByAi)
			{
				obj.shootingComponent.shotIsGoingToBeFired.subscribers += HandlePlasmaWeaponShot;
			}
		}

		protected override void Remove(PlasmaWeaponShootingNode obj)
		{
			obj.shootingComponent.shotIsGoingToBeFired.subscribers -= HandlePlasmaWeaponShot;
		}

		protected override void Add(AIAgentDataComponentsNode obj)
		{
			_weaponFeedbackComponents.Add(obj.aiBotIdData.playerId, obj.aiWeaponShootingFeedbackComponent);
		}

		protected override void Remove(AIAgentDataComponentsNode obj)
		{
			_weaponFeedbackComponents.Remove(obj.aiBotIdData.playerId);
		}

		private void HandleLaserWeaponShot(IShootingComponent shootingComponent, int nodeId)
		{
			LaserWeaponShootingNode laserWeaponShootingNode = default(LaserWeaponShootingNode);
			if (entityViewsDB.TryQueryEntityView<LaserWeaponShootingNode>(nodeId, ref laserWeaponShootingNode))
			{
				int value = laserWeaponShootingNode.weaponOwner.ownerId;
				_weaponFeedbackComponents[laserWeaponShootingNode.weaponOwner.ownerId].shotIsGoingToBeFired.Dispatch(ref value);
			}
		}

		private void HandlePlasmaWeaponShot(IShootingComponent shootingComponent, int nodeId)
		{
			PlasmaWeaponShootingNode plasmaWeaponShootingNode = default(PlasmaWeaponShootingNode);
			if (entityViewsDB.TryQueryEntityView<PlasmaWeaponShootingNode>(nodeId, ref plasmaWeaponShootingNode))
			{
				int value = plasmaWeaponShootingNode.weaponOwner.ownerId;
				_weaponFeedbackComponents[plasmaWeaponShootingNode.weaponOwner.ownerId].shotIsGoingToBeFired.Dispatch(ref value);
			}
		}

		public void Ready()
		{
		}
	}
}
