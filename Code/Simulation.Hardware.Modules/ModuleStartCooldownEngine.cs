using Svelto.DataStructures;
using Svelto.ECS;
using UnityEngine;

namespace Simulation.Hardware.Modules
{
	internal sealed class ModuleStartCooldownEngine : MultiEntityViewsEngine<ModuleCooldownNode, MachineInvisibilityNode>, IQueryingEntityViewEngine, IEngine
	{
		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(ModuleCooldownNode cooldownNode)
		{
			cooldownNode.confirmActivationComponent.activationConfirmed.subscribers += HandleCooldownStart;
		}

		protected override void Remove(ModuleCooldownNode cooldownNode)
		{
			cooldownNode.confirmActivationComponent.activationConfirmed.subscribers -= HandleCooldownStart;
		}

		private void HandleMachineBecameVisible(IMachineVisibilityComponent sender, int machineId)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<ModuleCooldownNode> val = entityViewsDB.QueryEntityViews<ModuleCooldownNode>();
			int num = 0;
			ModuleCooldownNode moduleCooldownNode;
			while (true)
			{
				if (num < val.get_Count())
				{
					moduleCooldownNode = val.get_Item(num);
					if (moduleCooldownNode.ownerComponent.machineId == machineId && moduleCooldownNode.itemDescriptorComponent.itemDescriptor.itemCategory == ItemCategory.GhostModule)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			StartCooldown(moduleCooldownNode);
		}

		private void HandleCooldownStart(IModuleConfirmActivationComponent sender, int moduleId)
		{
			ModuleCooldownNode node = default(ModuleCooldownNode);
			if (entityViewsDB.TryQueryEntityView<ModuleCooldownNode>(moduleId, ref node))
			{
				StartCooldown(node);
			}
		}

		private void StartCooldown(ModuleCooldownNode node)
		{
			int value = node.get_ID();
			node.moduleCooldownGuiComponent.startCooldown.Dispatch(ref value);
			ItemDescriptor itemDescriptor = node.itemDescriptorComponent.itemDescriptor;
			int num = ItemDescriptorKey.GenerateKey(itemDescriptor.itemCategory, itemDescriptor.itemSize);
			ModuleGroupLastShotTimeNode moduleGroupLastShotTimeNode = default(ModuleGroupLastShotTimeNode);
			if (entityViewsDB.TryQueryEntityView<ModuleGroupLastShotTimeNode>(num, ref moduleGroupLastShotTimeNode))
			{
				moduleGroupLastShotTimeNode.lastShotTimeComponent.lastShotTime = Time.get_time();
			}
		}

		protected override void Add(MachineInvisibilityNode invisibilityNode)
		{
			invisibilityNode.machineVisibilityComponent.machineBecameVisible.subscribers += HandleMachineBecameVisible;
		}

		protected override void Remove(MachineInvisibilityNode invisibilityNode)
		{
			invisibilityNode.machineVisibilityComponent.machineBecameVisible.subscribers -= HandleMachineBecameVisible;
		}
	}
}
