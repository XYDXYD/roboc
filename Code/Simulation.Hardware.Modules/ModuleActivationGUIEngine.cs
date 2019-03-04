using Fabric;
using Services.Simulation;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Ticker.Legacy;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Modules
{
	internal class ModuleActivationGUIEngine : MultiEntityViewsEngine<ModuleGUIEntityView, PowerBarNode, MachineWeaponOrderView>, IInitialize, IWaitForFrameworkDestruction, ITickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private class ModuleActivationGUIData
		{
			public bool alive;

			public float cooldownEndTime;

			public float powerCost;

			public ModuleActivationGUIData(bool alive, float cooldownEndTime, float powerCost)
			{
				this.alive = alive;
				this.cooldownEndTime = cooldownEndTime;
				this.powerCost = powerCost;
			}
		}

		private HardwareDestroyedObserver _hardwareDestroyedObserver;

		private HardwareEnabledObserver _hardwareEnabledObserver;

		private Dictionary<ItemCategory, ModuleActivationGUIData> _moduleActivationGuiData = new Dictionary<ItemCategory, ModuleActivationGUIData>();

		private IPowerBarDataComponent _powerBar;

		private IDictionary<int, WeaponStatsData> _weaponStatsData;

		[Inject]
		internal WeaponOrderPresenter weaponOrderPresenter
		{
			get;
			set;
		}

		[Inject]
		internal ICubeList cubeList
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public ModuleActivationGUIEngine(HardwareDestroyedObserver destroyedObserver, HardwareEnabledObserver enabledObserver)
		{
			_hardwareDestroyedObserver = destroyedObserver;
			_hardwareEnabledObserver = enabledObserver;
		}

		public void Ready()
		{
		}

		public void OnDependenciesInjected()
		{
			ILoadWeaponStatsRequest loadWeaponStatsRequest = serviceFactory.Create<ILoadWeaponStatsRequest>();
			loadWeaponStatsRequest.SetAnswer(new ServiceAnswer<IDictionary<int, WeaponStatsData>>(delegate(IDictionary<int, WeaponStatsData> statsData)
			{
				_weaponStatsData = statsData;
			})).Execute();
		}

		public void Tick(float deltaSec)
		{
			Dictionary<ItemCategory, ModuleActivationGUIData>.Enumerator enumerator = _moduleActivationGuiData.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ItemCategory key = enumerator.Current.Key;
				ModuleActivationGUIData value = enumerator.Current.Value;
				if (_powerBar != null)
				{
					float powerCost = value.powerCost;
					bool flag = ((powerCost >= 0f && powerCost <= _powerBar.powerValue) || powerCost < 0f) && value.alive && Time.get_time() > value.cooldownEndTime;
					weaponOrderPresenter.ShowActivatableModule(key, flag);
					ActivateReadyEffect(key, flag);
				}
				weaponOrderPresenter.SetModuleCooldownTime(key, value.cooldownEndTime - Time.get_time());
			}
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_hardwareDestroyedObserver.RemoveAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_hardwareEnabledObserver.RemoveAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleOnHardwareEnabled(ref ItemDescriptor hardwareType)
		{
			if (hardwareType is ModuleDescriptor)
			{
				ItemCategory itemCategory = hardwareType.itemCategory;
				_moduleActivationGuiData[itemCategory].alive = true;
			}
		}

		private void HandleOnHardwareDestroyed(ref ItemDescriptor hardwareType)
		{
			if (hardwareType is ModuleDescriptor)
			{
				ItemCategory itemCategory = hardwareType.itemCategory;
				_moduleActivationGuiData[itemCategory].alive = false;
			}
		}

		private void HandleOnStartCooldown(IModuleGuiCooldownComponent sender, int nodeId)
		{
			ModuleGUIEntityView moduleGUIEntityView = default(ModuleGUIEntityView);
			if (entityViewsDB.TryQueryEntityView<ModuleGUIEntityView>(nodeId, ref moduleGUIEntityView))
			{
				ItemCategory itemCategory = moduleGUIEntityView.itemDescriptorComponent.itemDescriptor.itemCategory;
				weaponOrderPresenter.ShowActivatableModule(itemCategory, activable: false);
				float weaponCooldown = moduleGUIEntityView.cooldownComponent.weaponCooldown;
				weaponOrderPresenter.SetCooldown(itemCategory, weaponCooldown);
				_moduleActivationGuiData[itemCategory].cooldownEndTime = Time.get_time() + weaponCooldown;
			}
		}

		private void HandleOnResetCooldown(IModuleGuiCooldownComponent sender, ItemCategory itemCategory)
		{
			weaponOrderPresenter.ResetCooldown(itemCategory);
		}

		private void HandleNotEnoughPower(IModuleGuiCooldownComponent arg1, ItemCategory moduleType)
		{
			weaponOrderPresenter.PlayNotEnoughPowerAnimation(moduleType);
			_powerBar.PlayNotEnoughPowerAnimation();
			EventManager.get_Instance().PostEvent("GUI_NoMana", 0);
		}

		private void HandleCooldownStillActive(IModuleGuiCooldownComponent arg1, ItemCategory moduleType)
		{
			EventManager.get_Instance().PostEvent("GUI_CoolDown_Module", 0);
		}

		private void ActivateReadyEffect(ItemCategory moduleType, bool effectActive)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<ModuleGUIEntityView> val = entityViewsDB.QueryEntityViews<ModuleGUIEntityView>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				ModuleGUIEntityView moduleGUIEntityView = val.get_Item(i);
				if (moduleGUIEntityView.itemDescriptorComponent.itemDescriptor.itemCategory == moduleType)
				{
					moduleGUIEntityView.readyEffectActivationComponent.activateReadyEffect.set_value(effectActive);
				}
			}
		}

		protected override void Add(ModuleGUIEntityView moduleNode)
		{
			if (moduleNode.ownerComponent.ownedByMe)
			{
				moduleNode.moduleCooldownGuiComponent.startCooldown.subscribers += HandleOnStartCooldown;
				moduleNode.moduleCooldownGuiComponent.resetCooldown.subscribers += HandleOnResetCooldown;
				moduleNode.moduleCooldownGuiComponent.notEnoughPower.subscribers += HandleNotEnoughPower;
				moduleNode.moduleCooldownGuiComponent.cooldownStillActive.subscribers += HandleCooldownStillActive;
			}
		}

		protected override void Remove(ModuleGUIEntityView moduleNode)
		{
			if (moduleNode.ownerComponent.ownedByMe)
			{
				moduleNode.moduleCooldownGuiComponent.startCooldown.subscribers -= HandleOnStartCooldown;
				moduleNode.moduleCooldownGuiComponent.resetCooldown.subscribers -= HandleOnResetCooldown;
				moduleNode.moduleCooldownGuiComponent.notEnoughPower.subscribers -= HandleNotEnoughPower;
				moduleNode.moduleCooldownGuiComponent.cooldownStillActive.subscribers -= HandleCooldownStillActive;
			}
		}

		protected override void Add(PowerBarNode powerNode)
		{
			_powerBar = powerNode.powerBarDataComponent;
		}

		protected override void Remove(PowerBarNode powerNode)
		{
			_powerBar = null;
		}

		protected unsafe override void Add(MachineWeaponOrderView entityView)
		{
			if (entityView.ownerComponent.ownedByMe)
			{
				_hardwareDestroyedObserver.AddAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
				_hardwareEnabledObserver.AddAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
				InitModuleData(entityView.orderComponent.weaponOrder);
			}
		}

		private void InitModuleData(WeaponOrderSimulation order)
		{
			for (int i = 0; i < order.Count(); i++)
			{
				int itemDescriptorKeyByIndex = order.GetItemDescriptorKeyByIndex(i);
				if (itemDescriptorKeyByIndex > 0)
				{
					ItemDescriptor itemDescriptorFromCube = cubeList.GetItemDescriptorFromCube(itemDescriptorKeyByIndex);
					if (itemDescriptorFromCube is ModuleDescriptor)
					{
						WeaponStatsData weaponStatsData = _weaponStatsData[itemDescriptorKeyByIndex];
						ModuleActivationGUIData value = new ModuleActivationGUIData(alive: false, Time.get_time(), weaponStatsData.manaCost);
						_moduleActivationGuiData.Add(itemDescriptorFromCube.itemCategory, value);
					}
				}
			}
		}

		protected unsafe override void Remove(MachineWeaponOrderView entityView)
		{
			if (entityView.ownerComponent.ownedByMe)
			{
				_hardwareDestroyedObserver.RemoveAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
				_hardwareEnabledObserver.RemoveAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
				_moduleActivationGuiData.Clear();
			}
		}
	}
}
