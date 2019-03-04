using Fabric;
using Services.Simulation;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Context;
using Svelto.ECS;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation.Hardware
{
	internal sealed class LocalPlayerSwitchWeaponEngine : MultiEntityViewsEngine<WeaponSwitchNode, MachineWeaponOrderView>, IInitialize, IWaitForFrameworkDestruction, IHandleCharacterInput, IQueryingEntityViewEngine, IInputComponent, IComponent, IEngine
	{
		private const string WEAPON_CHARACTER_INPUT_NAME = "SELECT_LOADOUT_SLOT";

		private static readonly WeaponOrderSimulation EMPTY_WEAPON_ORDER = new WeaponOrderSimulation(new int[0]);

		private bool _initialised;

		private WeaponOrderSimulation _weaponOrder;

		private bool _allowClick;

		private StringBuilder _stringBuilder = new StringBuilder();

		private SwitchWeaponData _activeWeaponData;

		private int _localMachineId;

		private Dictionary<CharacterInputAxis, SwitchWeaponData> _weaponTypeInputAxis = new Dictionary<CharacterInputAxis, SwitchWeaponData>(5, default(CharacterInputAxisComparer));

		private Dictionary<CharacterInputAxis, float> _previousValuesForInputAxis = new Dictionary<CharacterInputAxis, float>(5, default(CharacterInputAxisComparer));

		private Dictionary<int, SwitchWeaponData> _switchWeaponDataByItemDescriptorKey = new Dictionary<int, SwitchWeaponData>(5);

		private HardwareDestroyedObserver _destroyedObserver;

		private HardwareEnabledObserver _enabledObserver;

		private SelectWeaponDependency _weaponSelectDependency = new SelectWeaponDependency();

		private SendSelectedWeaponClientCommand _weaponSelectClientCommand;

		private ModuleSelectedObservable _moduleSelectedObservable;

		[Inject]
		internal SwitchWeaponObserver switchWeaponObserver
		{
			private get;
			set;
		}

		[Inject]
		internal WeaponOrderPresenter weaponOrderPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal MachineSpawnDispatcher spawnDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		internal GameStartDispatcher gameStartDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeList cubeList
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		private bool canClick => _initialised && _allowClick;

		public unsafe LocalPlayerSwitchWeaponEngine(HardwareDestroyedObserver destroyedObserver, HardwareEnabledObserver enabledObserver, ModuleSelectedObservable moduleSelectedObservable)
		{
			_destroyedObserver = destroyedObserver;
			_enabledObserver = enabledObserver;
			_moduleSelectedObservable = moduleSelectedObservable;
			enabledObserver.AddAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void Ready()
		{
		}

		private unsafe void HandleOnPlayerRespawnedIn(SpawnInParametersPlayer data)
		{
			if (data.isMe)
			{
				_destroyedObserver.AddAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
				_allowClick = true;
			}
		}

		private unsafe void HandleOnMachineDestroyed(int playerId, int machineId, bool isMe)
		{
			if (isMe)
			{
				_destroyedObserver.RemoveAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
				_allowClick = false;
			}
		}

		void IHandleCharacterInput.HandleCharacterInput(InputCharacterData data)
		{
			if (!_initialised)
			{
				return;
			}
			Dictionary<CharacterInputAxis, SwitchWeaponData>.Enumerator enumerator = _weaponTypeInputAxis.GetEnumerator();
			CharacterInputAxis key;
			float num;
			while (true)
			{
				if (enumerator.MoveNext())
				{
					key = enumerator.Current.Key;
					num = data.data[(int)key];
					num = ((num > 0f) ? 1 : 0);
					if (num > 0f && num != _previousValuesForInputAxis[key])
					{
						break;
					}
					_previousValuesForInputAxis[key] = num;
					continue;
				}
				return;
			}
			SwitchWeaponData switchWeaponData = _weaponTypeInputAxis[key];
			if (CanBeSelected(switchWeaponData.itemDescriptorKey))
			{
				ActivateSlot(switchWeaponData.itemDescriptorKey);
			}
			_previousValuesForInputAxis[key] = num;
		}

		void IInitialize.OnDependenciesInjected()
		{
			destructionReporter.OnMachineDestroyed += HandleOnMachineDestroyed;
			spawnDispatcher.OnPlayerRespawnedIn += HandleOnPlayerRespawnedIn;
			gameStartDispatcher.Register(HandleOnGameStart);
			_weaponSelectClientCommand = commandFactory.Build<SendSelectedWeaponClientCommand>();
		}

		protected override void Add(WeaponSwitchNode node)
		{
			RegisterWeapon(node);
		}

		protected override void Remove(WeaponSwitchNode n)
		{
			n.weaponActiveComponent.active = false;
		}

		protected unsafe override void Add(MachineWeaponOrderView node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				WeaponOrderSimulation weaponOrder = node.orderComponent.weaponOrder;
				_destroyedObserver.AddAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
				_allowClick = true;
				weaponOrderPresenter.ShowWeaponOrder(weaponOrder);
				InitSwitchData(weaponOrder, node.ownerComponent.ownerMachineId);
				_localMachineId = node.ownerComponent.ownerMachineId;
				_initialised = true;
				_weaponOrder = weaponOrder;
			}
		}

		protected unsafe override void Remove(MachineWeaponOrderView node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				_initialised = false;
				_allowClick = false;
				_activeWeaponData = null;
				_weaponTypeInputAxis.Clear();
				_previousValuesForInputAxis.Clear();
				_switchWeaponDataByItemDescriptorKey.Clear();
				_localMachineId = -1;
				_destroyedObserver.RemoveAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
		}

		private void InitSwitchData(WeaponOrderSimulation order, int machineId)
		{
			for (int i = 0; i < order.Count(); i++)
			{
				int itemDescriptorKeyByIndex = order.GetItemDescriptorKeyByIndex(i);
				if (itemDescriptorKeyByIndex > 0)
				{
					ItemDescriptor itemDescriptorFromCube = cubeList.GetItemDescriptorFromCube(itemDescriptorKeyByIndex);
					SwitchWeaponData switchWeaponData = new SwitchWeaponData(machineId, itemDescriptorFromCube);
					switchWeaponData.order = order.GetIndexByItemDescriptor(itemDescriptorFromCube);
					_switchWeaponDataByItemDescriptorKey.Add(itemDescriptorKeyByIndex, switchWeaponData);
					SetWeaponHotKey(itemDescriptorFromCube, switchWeaponData);
				}
			}
		}

		private ItemDescriptor GetFirstWeapon(WeaponOrderSimulation order)
		{
			for (int i = 0; i < order.Count(); i++)
			{
				int itemDescriptorKeyByIndex = order.GetItemDescriptorKeyByIndex(i);
				if (itemDescriptorKeyByIndex > 0)
				{
					ItemDescriptor itemDescriptorFromCube = cubeList.GetItemDescriptorFromCube(itemDescriptorKeyByIndex);
					if (itemDescriptorFromCube is WeaponDescriptor)
					{
						return itemDescriptorFromCube;
					}
				}
			}
			return null;
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			destructionReporter.OnMachineDestroyed -= HandleOnMachineDestroyed;
			spawnDispatcher.OnPlayerRespawnedIn -= HandleOnPlayerRespawnedIn;
			_destroyedObserver.RemoveAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_enabledObserver.RemoveAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleOnGameStart()
		{
			if (_activeWeaponData != null)
			{
				SendWeaponSelectCommandToServer(_activeWeaponData.machineId, _activeWeaponData.itemDescriptor);
				gameStartDispatcher.Unregister(HandleOnGameStart);
			}
		}

		private bool CanBeSelected(int itemDescriptorKey)
		{
			SwitchWeaponData switchWeaponData = _switchWeaponDataByItemDescriptorKey[itemDescriptorKey];
			if (!canClick || itemDescriptorKey == 0 || switchWeaponData.destroyed)
			{
				if (switchWeaponData.destroyed)
				{
					EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.WeaponSwitch_Destroyed));
				}
				return false;
			}
			MachineRectifierNode machineRectifierNode = default(MachineRectifierNode);
			if (entityViewsDB.TryQueryEntityView<MachineRectifierNode>(_localMachineId, ref machineRectifierNode) && !machineRectifierNode.machineFunctionalsComponent.functionalsEnabled)
			{
				return false;
			}
			return true;
		}

		private void ActivateSlot(int itemDescriptorKey)
		{
			_allowClick = false;
			if (_activeWeaponData == null || itemDescriptorKey != _activeWeaponData.itemDescriptorKey)
			{
				ItemDescriptor itemDescriptorFromCube = cubeList.GetItemDescriptorFromCube(itemDescriptorKey);
				if (itemDescriptorFromCube is ModuleDescriptor)
				{
					ItemCategory itemCategory = itemDescriptorFromCube.itemCategory;
					_moduleSelectedObservable.Dispatch(ref itemCategory);
				}
				else
				{
					SelectWeapon(itemDescriptorKey);
				}
			}
			_allowClick = true;
		}

		private void SelectWeapon(ItemDescriptor itemDescriptor)
		{
			SelectWeapon(itemDescriptor.GenerateKey());
		}

		private void SelectWeapon(int itemDescriptorKey)
		{
			if (_activeWeaponData != null)
			{
				_activeWeaponData.Activate(activate: false);
			}
			_activeWeaponData = _switchWeaponDataByItemDescriptorKey[itemDescriptorKey];
			_activeWeaponData.Activate(activate: true);
			SendWeaponSelectCommandToServer(_activeWeaponData.machineId, _activeWeaponData.itemDescriptor);
			switchWeaponObserver.SwitchWeapons(_activeWeaponData.machineId, _activeWeaponData.itemDescriptor);
			switchWeaponObserver.SwitchCrosshair(_activeWeaponData.crosshairType);
			weaponOrderPresenter.ShowSelectedItem(itemDescriptorKey);
		}

		private void SendWeaponSelectCommandToServer(int machineId, ItemDescriptor itemDescriptor)
		{
			_weaponSelectDependency.SetParameters(machineId, itemDescriptor);
			_weaponSelectClientCommand.Inject(_weaponSelectDependency);
			_weaponSelectClientCommand.Execute();
		}

		private void DeactivateWeaponSubCategory(ref ItemDescriptor itemDescriptor)
		{
			int key = itemDescriptor.GenerateKey();
			if (_switchWeaponDataByItemDescriptorKey.TryGetValue(key, out SwitchWeaponData value))
			{
				value.destroyed = true;
				weaponOrderPresenter.ShowDestroyedWeaponCategory(itemDescriptor);
			}
		}

		private void ActivateWeaponSubCategory(ref ItemDescriptor itemDescriptor)
		{
			int key = itemDescriptor.GenerateKey();
			if (_switchWeaponDataByItemDescriptorKey.TryGetValue(key, out SwitchWeaponData value))
			{
				value.destroyed = false;
				weaponOrderPresenter.ShowActiveWeaponCategory(itemDescriptor);
			}
		}

		private void RegisterWeapon(WeaponSwitchNode weaponSwitchNode)
		{
			if (weaponSwitchNode.weaponOwnerComponent.ownedByMe)
			{
				ItemDescriptor itemDescriptor = weaponSwitchNode.itemDescriptorComponent.itemDescriptor;
				int key = itemDescriptor.GenerateKey();
				SwitchWeaponData switchWeaponData = _switchWeaponDataByItemDescriptorKey[key];
				switchWeaponData.crosshairType = weaponSwitchNode.weaponCrosshairTypeComponent.crosshairType;
				switchWeaponData.AddWeapon(weaponSwitchNode);
				ItemDescriptor firstWeapon = GetFirstWeapon(_weaponOrder);
				if (firstWeapon == itemDescriptor)
				{
					SelectWeapon(firstWeapon);
				}
			}
		}

		private void SetWeaponHotKey(ItemDescriptor itemDescriptor, SwitchWeaponData switchWeaponData)
		{
			CharacterInputAxis key = CreateInputAxis(switchWeaponData.order);
			_weaponTypeInputAxis.Add(key, switchWeaponData);
			_previousValuesForInputAxis.Add(key, 0f);
		}

		private CharacterInputAxis CreateInputAxis(int index)
		{
			_stringBuilder.Length = 0;
			_stringBuilder.AppendFormat("{0}_{1}", "SELECT_LOADOUT_SLOT", index + 1);
			return (CharacterInputAxis)Enum.Parse(typeof(CharacterInputAxis), _stringBuilder.ToString());
		}
	}
}
