using UnityEngine.Networking;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityNetwork
{
	public class IsClient : Conditional
	{
		public IsClient()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			return (!NetworkClient.get_active()) ? 1 : 2;
		}
	}
}
