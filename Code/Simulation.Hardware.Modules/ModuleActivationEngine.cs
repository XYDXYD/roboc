using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Observer;
using System;
using UnityEngine;

namespace Simulation.Hardware.Modules
{
	internal class ModuleActivationEngine : SingleEntityViewEngine<PowerBarNode>, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, IEngine
	{
		private readonly ModuleSelectedObserver _moduleSelectedObserver;

		private IPowerBarDataComponent _powerBar;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe ModuleActivationEngine(ModuleSelectedObserver moduleSelectedObserver)
		{
			_moduleSelectedObserver = moduleSelectedObserver;
			_moduleSelectedObserver.AddAction(new ObserverAction<ItemCategory>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		protected override void Add(PowerBarNode powerBarNode)
		{
			_powerBar = powerBarNode.powerBarDataComponent;
		}

		protected override void Remove(PowerBarNode powerBarNode)
		{
			_powerBar = null;
		}

		public void Ready()
		{
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_moduleSelectedObserver.RemoveAction(new ObserverAction<ItemCategory>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleOnModuleSelected(ref ItemCategory moduleType)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<ModuleActivationNode> val = entityViewsDB.QueryEntityViews<ModuleActivationNode>();
			int num = 0;
			ModuleActivationNode moduleActivationNode;
			while (true)
			{
				if (num >= val.get_Count())
				{
					return;
				}
				moduleActivationNode = val.get_Item(num);
				if (moduleActivationNode.itemDescriptorComponent.itemDescriptor.itemCategory == moduleType && moduleActivationNode.ownerComponent.ownedByMe)
				{
					ItemDescriptor itemDescriptor = moduleActivationNode.itemDescriptorComponent.itemDescriptor;
					int num2 = ItemDescriptorKey.GenerateKey(itemDescriptor.itemCategory, itemDescriptor.itemSize);
					ModuleGroupLastShotTimeNode moduleGroupLastShotTimeNode = default(ModuleGroupLastShotTimeNode);
					if (entityViewsDB.TryQueryEntityView<ModuleGroupLastShotTimeNode>(num2, ref moduleGroupLastShotTimeNode))
					{
						bool flag = (moduleActivationNode.manaCostComponent.weaponFireCost >= 0f && moduleActivationNode.manaCostComponent.weaponFireCost <= _powerBar.powerValue) || (moduleActivationNode.manaCostComponent.weaponFireCost < 0f && _powerBar.powerPercent < 1f);
						if ((moduleGroupLastShotTimeNode.lastShotTimeComponent.lastShotTime == 0f || Time.get_time() >= moduleGroupLastShotTimeNode.lastShotTimeComponent.lastShotTime + moduleActivationNode.cooldownComponent.weaponCooldown) && !moduleActivationNode.healthStatusComponent.disabled && flag)
						{
							int value = moduleActivationNode.get_ID();
							moduleActivationNode.activationComponent.activate.Dispatch(ref value);
							return;
						}
						if (Time.get_time() < moduleGroupLastShotTimeNode.lastShotTimeComponent.lastShotTime + moduleActivationNode.cooldownComponent.weaponCooldown)
						{
							moduleActivationNode.moduleCooldownGuiComponent.cooldownStillActive.Dispatch(ref moduleType);
							return;
						}
						if (!flag)
						{
							break;
						}
					}
				}
				num++;
			}
			moduleActivationNode.moduleCooldownGuiComponent.notEnoughPower.Dispatch(ref moduleType);
		}
	}
}
