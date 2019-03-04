using Svelto.ECS;
using System;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal class StackDamageBonusEngine : SingleEntityViewEngine<StackDamageBonusNode>, IQueryingEntityViewEngine, IEngine
	{
		private int _currentStackIndex;

		private float _lastValidHitTime;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(StackDamageBonusNode obj)
		{
			obj.stackDamageComponent.stackableHit.NotifyOnValueSet((Action<int, bool>)OnValidHit);
		}

		protected override void Remove(StackDamageBonusNode obj)
		{
			obj.stackDamageComponent.stackableHit.StopNotify((Action<int, bool>)OnValidHit);
		}

		private void OnValidHit(int id, bool isValid)
		{
			StackDamageBonusNode stackDamageBonusNode = default(StackDamageBonusNode);
			if (!entityViewsDB.TryQueryEntityView<StackDamageBonusNode>(id, ref stackDamageBonusNode) || !stackDamageBonusNode.entitySourceComponent.isLocal)
			{
				return;
			}
			if (isValid)
			{
				float timeSinceLevelLoad = Time.get_timeSinceLevelLoad();
				if (timeSinceLevelLoad - _lastValidHitTime <= stackDamageBonusNode.stackDamageComponent.buffStackExpireTime)
				{
					_currentStackIndex = Mathf.Min(stackDamageBonusNode.stackDamageComponent.buffMaxStacks, _currentStackIndex + 1);
				}
				else
				{
					_currentStackIndex = 0;
				}
				_lastValidHitTime = timeSinceLevelLoad;
			}
			else
			{
				_currentStackIndex = 0;
				_lastValidHitTime = 0f;
			}
			stackDamageBonusNode.stackDamageComponent.currentStackIndex = _currentStackIndex;
		}
	}
}
