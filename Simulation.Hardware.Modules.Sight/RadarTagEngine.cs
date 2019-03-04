using Simulation.GUI;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Hardware.Modules.Sight
{
	internal class RadarTagEngine : SingleEntityViewEngine<RadarTagNode>, IQueryingEntityViewEngine, IEngine
	{
		private bool _radarActive;

		private FasterList<RadarTagNode> _spottedEnemies = new FasterList<RadarTagNode>();

		[Inject]
		internal HUDRadarTagPresenter radarTagPresenter
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe RadarTagEngine(TeamRadarObserver teamRadarObserver)
		{
			teamRadarObserver.AddAction(new ObserverAction<bool>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void Ready()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)Tick);
		}

		protected override void Add(RadarTagNode node)
		{
			node.spottableComponent.isSpotted.NotifyOnValueSet((Action<int, bool>)OnMachineSpotStateChanged);
		}

		protected override void Remove(RadarTagNode node)
		{
			node.spottableComponent.isSpotted.StopNotify((Action<int, bool>)OnMachineSpotStateChanged);
		}

		private void OnMachineSpotStateChanged(int machineId, bool isSpotted)
		{
			RadarTagNode radarTagNode = entityViewsDB.QueryEntityView<RadarTagNode>(machineId);
			if (isSpotted)
			{
				_spottedEnemies.Add(radarTagNode);
				if (_radarActive)
				{
					radarTagPresenter.StartTag(machineId);
				}
			}
			else
			{
				_spottedEnemies.UnorderedRemove(radarTagNode);
				if (_radarActive)
				{
					radarTagPresenter.StopTag(machineId);
				}
			}
		}

		private void OnTeamRadarStateChange(ref bool anyActive)
		{
			if (_radarActive != anyActive)
			{
				_radarActive = anyActive;
				if (_radarActive)
				{
					ShowAllTags();
				}
				else
				{
					HideAllTags();
				}
			}
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				if (_radarActive)
				{
					UpdateAllTags();
				}
				yield return null;
			}
		}

		private void UpdateAllTags()
		{
			for (int i = 0; i < _spottedEnemies.get_Count(); i++)
			{
				RadarTagNode machineNode = _spottedEnemies.get_Item(i);
				UpdateTag(machineNode);
			}
		}

		private void ShowAllTags()
		{
			for (int i = 0; i < _spottedEnemies.get_Count(); i++)
			{
				RadarTagNode radarTagNode = _spottedEnemies.get_Item(i);
				radarTagPresenter.StartTag(radarTagNode.get_ID());
			}
			UpdateAllTags();
		}

		private void HideAllTags()
		{
			for (int i = 0; i < _spottedEnemies.get_Count(); i++)
			{
				RadarTagNode radarTagNode = _spottedEnemies.get_Item(i);
				radarTagPresenter.StopTag(radarTagNode.get_ID());
			}
		}

		private void UpdateTag(RadarTagNode machineNode)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			Camera main = Camera.get_main();
			Vector3 worldCenterOfMass = machineNode.rbComponent.rb.get_worldCenterOfMass();
			Vector3 val = main.WorldToViewportPoint(worldCenterOfMass);
			bool offscreen = false;
			if (val.z > 0f)
			{
				if (val.x < 0f)
				{
					val.x = 0f;
					offscreen = true;
				}
				else if (val.x > 1f)
				{
					val.x = 1f;
					offscreen = true;
				}
				if (val.y < 0f)
				{
					val.y = 0f;
					offscreen = true;
				}
				else if (val.y > 1f)
				{
					val.y = 1f;
					offscreen = true;
				}
			}
			else
			{
				offscreen = true;
				val.x = 1f - val.x;
				val.y = 1f - val.y;
				if (val.y < val.x)
				{
					if (val.y < 1f - val.x)
					{
						val.y = 0f;
					}
					else
					{
						val.x = 1f;
					}
				}
				else if (val.y > 1f - val.x)
				{
					val.y = 1f;
				}
				else
				{
					val.x = 0f;
				}
			}
			radarTagPresenter.UpdateTag(machineNode.get_ID(), Vector2.op_Implicit(val), offscreen);
		}
	}
}
