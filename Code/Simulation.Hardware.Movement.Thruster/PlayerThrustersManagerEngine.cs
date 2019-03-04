using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Ticker.Legacy;
using System;

namespace Simulation.Hardware.Movement.Thruster
{
	internal class PlayerThrustersManagerEngine : SingleEntityViewEngine<ThrusterManagerNode>, ITickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private int _prevNumPlayerThrusters;

		private int _prevNumPlayerPropellers;

		private FasterList<ThrusterManagerNode> _localPlayerThrustersNodes = new FasterList<ThrusterManagerNode>();

		private FasterList<ThrusterManagerNode> _localPlayerPropellersNodes = new FasterList<ThrusterManagerNode>();

		private int[] _thrusterCountPerSide = new int[CubeFaceExtensions.NumberOfFaces()];

		private int[] _propellersCountPerSide = new int[CubeFaceExtensions.NumberOfFaces()];

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		protected override void Add(ThrusterManagerNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				if (!node.disabledComponent.isPartDisabled.get_value())
				{
					AddNode(node);
				}
				node.disabledComponent.isPartDisabled.NotifyOnValueSet((Action<int, bool>)OnMovementPartDestroyed);
			}
		}

		private void AddNode(ThrusterManagerNode node)
		{
			if (node.typeComponent.type == ThrusterType.SingleDirection)
			{
				_localPlayerThrustersNodes.Add(node);
			}
			else
			{
				_localPlayerPropellersNodes.Add(node);
			}
		}

		protected override void Remove(ThrusterManagerNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				RemoveNode(node);
				node.disabledComponent.isPartDisabled.StopNotify((Action<int, bool>)OnMovementPartDestroyed);
			}
		}

		private void RemoveNode(ThrusterManagerNode node)
		{
			if (node.typeComponent.type == ThrusterType.SingleDirection)
			{
				_localPlayerThrustersNodes.UnorderedRemove(node);
			}
			else
			{
				_localPlayerPropellersNodes.UnorderedRemove(node);
			}
		}

		public void Tick(float deltaSec)
		{
			if (_prevNumPlayerThrusters != _localPlayerThrustersNodes.get_Count())
			{
				UpdateThrusterCount();
				_prevNumPlayerThrusters = _localPlayerThrustersNodes.get_Count();
			}
			if (_prevNumPlayerPropellers != _localPlayerPropellersNodes.get_Count())
			{
				UpdatePropellerCount();
				_prevNumPlayerPropellers = _localPlayerPropellersNodes.get_Count();
			}
		}

		private void OnMovementPartDestroyed(int i, bool destroyed)
		{
			ThrusterManagerNode thrusterManagerNode = default(ThrusterManagerNode);
			if (entityViewsDB.TryQueryEntityView<ThrusterManagerNode>(i, ref thrusterManagerNode) && thrusterManagerNode.ownerComponent.ownedByMe)
			{
				if (destroyed)
				{
					RemoveNode(thrusterManagerNode);
				}
				else
				{
					AddNode(thrusterManagerNode);
				}
			}
		}

		private void UpdateThrusterCount()
		{
			Array.Clear(_thrusterCountPerSide, 0, _thrusterCountPerSide.Length);
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < _localPlayerThrustersNodes.get_Count(); i++)
			{
				ThrusterManagerNode thrusterManagerNode = _localPlayerThrustersNodes.get_Item(i);
				_thrusterCountPerSide[(int)thrusterManagerNode.facingComponent.legacyDirection]++;
				if (thrusterManagerNode.facingComponent.pitchDirection == CubeFace.Up)
				{
					num++;
				}
				else if (thrusterManagerNode.facingComponent.pitchDirection == CubeFace.Down)
				{
					num2++;
				}
			}
			for (int j = 0; j < _localPlayerThrustersNodes.get_Count(); j++)
			{
				ThrusterManagerNode thrusterManagerNode2 = _localPlayerThrustersNodes.get_Item(j);
				thrusterManagerNode2.verticalCountComponent.numDownThrusters = num2;
				thrusterManagerNode2.verticalCountComponent.numUpThrusters = num;
			}
		}

		private void UpdatePropellerCount()
		{
			Array.Clear(_propellersCountPerSide, 0, _propellersCountPerSide.Length);
			for (int i = 0; i < _localPlayerPropellersNodes.get_Count(); i++)
			{
				ThrusterManagerNode thrusterManagerNode = _localPlayerPropellersNodes.get_Item(i);
				switch (thrusterManagerNode.facingComponent.legacyDirection)
				{
				case CubeFace.Up:
				case CubeFace.Down:
					_propellersCountPerSide[0]++;
					_propellersCountPerSide[1]++;
					break;
				case CubeFace.Front:
				case CubeFace.Back:
					_propellersCountPerSide[2]++;
					_propellersCountPerSide[3]++;
					break;
				case CubeFace.Right:
				case CubeFace.Left:
					_propellersCountPerSide[4]++;
					_propellersCountPerSide[5]++;
					break;
				}
			}
			for (int j = 0; j < _localPlayerPropellersNodes.get_Count(); j++)
			{
				ThrusterManagerNode thrusterManagerNode2 = _localPlayerPropellersNodes.get_Item(j);
				thrusterManagerNode2.verticalCountComponent.numDownThrusters = _propellersCountPerSide[1];
				thrusterManagerNode2.verticalCountComponent.numUpThrusters = _propellersCountPerSide[0];
			}
		}

		public void Ready()
		{
		}
	}
}
