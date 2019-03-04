using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class DiscShieldManagerEngine : IQueryingEntityViewEngine, ITickable, IEngine, ITickableBase
	{
		private IEntityViewsDB _entityViewsesDb;

		private FasterList<DiscShieldManagingNode> _activeShieldsList = new FasterList<DiscShieldManagingNode>();

		private FasterList<DiscShieldManagingNode> _closedShields = new FasterList<DiscShieldManagingNode>();

		private FasterList<DiscShieldManagingNode> _currentlyClosingShields = new FasterList<DiscShieldManagingNode>();

		private FasterList<DiscShieldManagingNode> _currentlyClosingShieldsToRemove = new FasterList<DiscShieldManagingNode>();

		private HashSet<DiscShieldManagingNode> _shieldsNearToClose = new HashSet<DiscShieldManagingNode>();

		[Inject]
		internal ShieldEntityObjectPool shieldEntityPool
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

		public void Ready()
		{
		}

		public void Tick(float deltaSec)
		{
			AddActiveDiscShields();
			UpdateActiveShields();
			RemoveClosedShields();
			UpdateClosingShields();
		}

		private void AddActiveDiscShields()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			FasterListEnumerator<DiscShieldManagingNode> enumerator = _entityViewsesDb.QueryEntityViews<DiscShieldManagingNode>().GetEnumerator();
			while (enumerator.MoveNext())
			{
				DiscShieldManagingNode current = enumerator.get_Current();
				if (current.justSpawnedComponent.discShieldJustSpawned)
				{
					current.justSpawnedComponent.discShieldJustSpawned = false;
					current.activationTimeComponent.activationTime = Time.get_time();
					_activeShieldsList.Add(current);
					int value = current.get_ID();
					current.effectsComponent.startOpenEffect.Dispatch(ref value);
				}
			}
		}

		private void UpdateActiveShields()
		{
			for (int i = 0; i < _activeShieldsList.get_Count(); i++)
			{
				DiscShieldManagingNode discShieldManagingNode = _activeShieldsList.get_Item(i);
				float activationTime = discShieldManagingNode.activationTimeComponent.activationTime;
				float num = Time.get_time() - activationTime;
				if (num >= discShieldManagingNode.effectsComponent.nearToCloseEffectStartTime && !_shieldsNearToClose.Contains(discShieldManagingNode))
				{
					_shieldsNearToClose.Add(discShieldManagingNode);
					int value = discShieldManagingNode.get_ID();
					discShieldManagingNode.effectsComponent.startNearToCloseEffect.Dispatch(ref value);
				}
				else if (num >= discShieldManagingNode.settingsComponent.discShieldLifeTime)
				{
					_closedShields.Add(discShieldManagingNode);
					int value2 = discShieldManagingNode.get_ID();
					discShieldManagingNode.effectsComponent.startCloseEffect.Dispatch(ref value2);
					_currentlyClosingShields.Add(discShieldManagingNode);
				}
			}
		}

		private void RemoveClosedShields()
		{
			for (int i = 0; i < _closedShields.get_Count(); i++)
			{
				DiscShieldManagingNode discShieldManagingNode = _closedShields.get_Item(i);
				_activeShieldsList.Remove(discShieldManagingNode);
				_shieldsNearToClose.Remove(discShieldManagingNode);
			}
			_closedShields.FastClear();
		}

		private void UpdateClosingShields()
		{
			for (int i = 0; i < _currentlyClosingShields.get_Count(); i++)
			{
				DiscShieldManagingNode discShieldManagingNode = _currentlyClosingShields.get_Item(i);
				IDiscShieldClosingTimeComponent closingTimeComponent = discShieldManagingNode.closingTimeComponent;
				if (closingTimeComponent.closingTime < discShieldManagingNode.effectsComponent.closeTime)
				{
					closingTimeComponent.closingTime += Time.get_deltaTime();
					continue;
				}
				ShieldEntity discShieldObject = discShieldManagingNode.objectComponent.discShieldObject;
				if (discShieldManagingNode.ownerComponent.isOnMyTeam)
				{
					shieldEntityPool.Recycle(discShieldObject, "T5_Disc_Shield_Module_Shield");
				}
				else
				{
					shieldEntityPool.Recycle(discShieldObject, "T5_Disc_Shield_Module_Shield_E");
				}
				closingTimeComponent.closingTime = 0f;
				discShieldManagingNode.objectComponent.shieldActive = false;
				_currentlyClosingShieldsToRemove.Add(discShieldManagingNode);
			}
			RemoveShieldsFromClosingList();
		}

		private void RemoveShieldsFromClosingList()
		{
			for (int i = 0; i < _currentlyClosingShieldsToRemove.get_Count(); i++)
			{
				_currentlyClosingShields.Remove(_currentlyClosingShieldsToRemove.get_Item(i));
			}
			_currentlyClosingShieldsToRemove.FastClear();
		}
	}
}
