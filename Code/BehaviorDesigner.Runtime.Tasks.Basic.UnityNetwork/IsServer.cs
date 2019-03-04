using UnityEngine.Networking;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityNetwork
{
	public class IsServer : Conditional
	{
		public IsServer()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			return (!NetworkServer.get_active()) ? 1 : 2;
		}
	}
}
