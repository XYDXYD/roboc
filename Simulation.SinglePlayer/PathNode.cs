using System;
using UnityEngine;

namespace Simulation.SinglePlayer
{
	[Serializable]
	public sealed class PathNode : MonoBehaviour
	{
		public PathNode NextNode;

		public float DestinationRadius = 1f;

		public float PathRadius = 1f;

		public Vector3 position
		{
			get;
			private set;
		}

		public PathNode()
			: this()
		{
		}

		private void Awake()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			position = this.get_transform().get_position();
		}

		private void OnDrawGizmos()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			position = this.get_transform().get_position();
			Gizmos.set_color(Color.get_blue());
			Gizmos.DrawSphere(position, 0.2f);
			if (NextNode != null)
			{
				Gizmos.DrawLine(position, NextNode.get_transform().get_position());
			}
			Gizmos.set_color(new Color(0f, 0f, 1f, 0.1f));
			Gizmos.DrawSphere(position, DestinationRadius);
		}
	}
}
