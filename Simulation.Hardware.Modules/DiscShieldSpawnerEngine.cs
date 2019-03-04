using RCNetwork.Events;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using UnityEngine;

namespace Simulation.Hardware.Modules
{
	internal sealed class DiscShieldSpawnerEngine : SingleEntityViewEngine<ShieldModuleActivationNode>, IQueryingEntityViewEngine, IWaitForFrameworkInitialization, IEngine
	{
		private IEntityViewsDB _entityViewsesDb;

		private Vector3 _middleScreenPoint;

		private Camera _mainCamera;

		private ShieldModuleEventDependency _dependency = new ShieldModuleEventDependency();

		private BuildShieldParametersData _parametersData = new BuildShieldParametersData();

		[Inject]
		internal DiscShieldFactory discShieldFactory
		{
			private get;
			set;
		}

		[Inject]
		internal INetworkEventManagerClient networkEventManager
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			set
			{
				_entityViewsesDb = value;
			}
		}

		protected override void Add(ShieldModuleActivationNode node)
		{
			node.activationComponent.activate.subscribers += SpawnDiscShield;
			if (node.ownerComponent.isEnemy)
			{
				discShieldFactory.PreallocateShield("T5_Disc_Shield_Module_Shield_E", 1);
			}
			else
			{
				discShieldFactory.PreallocateShield("T5_Disc_Shield_Module_Shield", 1);
			}
		}

		protected override void Remove(ShieldModuleActivationNode node)
		{
			node.activationComponent.activate.subscribers -= SpawnDiscShield;
		}

		public void Ready()
		{
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			_middleScreenPoint = new Vector3((float)(Screen.get_width() / 2), (float)(Screen.get_height() / 2), 1f);
			_mainCamera = Camera.get_main();
		}

		private void SpawnDiscShield(IModuleActivationComponent sender, int moduleId)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			Ray val = _mainCamera.ScreenPointToRay(_middleScreenPoint);
			Vector3 val2 = _mainCamera.ScreenToWorldPoint(_middleScreenPoint);
			ShieldModuleActivationNode shieldModuleActivationNode = default(ShieldModuleActivationNode);
			if (_entityViewsesDb.TryQueryEntityView<ShieldModuleActivationNode>(moduleId, ref shieldModuleActivationNode))
			{
				shieldModuleActivationNode.confirmActivationComponent.activationConfirmed.Dispatch(ref moduleId);
				int ownerId = shieldModuleActivationNode.ownerComponent.ownerId;
				Rigidbody rb = shieldModuleActivationNode.rigidbodyComponent.rb;
				RaycastHit val3 = default(RaycastHit);
				ShieldEntity shieldEntity;
				if (Physics.Raycast(val, ref val3, shieldModuleActivationNode.rangeComponent.moduleRange, GameLayers.INTERACTIVE_ENVIRONMENT_LAYER_MASK))
				{
					_parametersData.SetValues(val3.get_point(), val3.get_normal(), rb, Vector3.get_zero(), Quaternion.get_identity(), ownerId, isMine_: true, isOnMyTeam_: true);
					shieldEntity = discShieldFactory.Build("T5_Disc_Shield_Module_Shield", _parametersData, hitSomething: true);
				}
				else
				{
					Vector3 position_ = val2 + _mainCamera.get_transform().get_forward() * shieldModuleActivationNode.rangeComponent.moduleRange;
					Quaternion rotation_ = Quaternion.FromToRotation(Vector3.get_up(), _mainCamera.get_transform().get_forward());
					_parametersData.SetValues(Vector3.get_zero(), Vector3.get_zero(), null, position_, rotation_, ownerId, isMine_: true, isOnMyTeam_: true);
					shieldEntity = discShieldFactory.Build("T5_Disc_Shield_Module_Shield", _parametersData, hitSomething: false);
				}
				shieldEntity.get_gameObject().SetActive(true);
				_dependency = _dependency.Inject(shieldEntity.get_transform().get_position(), shieldEntity.get_transform().get_rotation(), ownerId);
				networkEventManager.SendEventToServer(NetworkEvent.ShieldSpawned, _dependency);
			}
		}
	}
}
