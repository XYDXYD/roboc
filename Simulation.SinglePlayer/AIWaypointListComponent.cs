using UnityEngine;

namespace Simulation.SinglePlayer
{
	public class AIWaypointListComponent : MonoBehaviour, IAIWaypointListComponent
	{
		[SerializeField]
		private int _ownTeamId;

		private PathNode[] _nodes;

		int IAIWaypointListComponent.teamId
		{
			get
			{
				return _ownTeamId;
			}
		}

		PathNode[] IAIWaypointListComponent.nodes
		{
			get
			{
				if (_nodes == null)
				{
					_nodes = this.GetComponentsInChildren<PathNode>(true);
				}
				return _nodes;
			}
		}

		public AIWaypointListComponent()
			: this()
		{
		}
	}
}
