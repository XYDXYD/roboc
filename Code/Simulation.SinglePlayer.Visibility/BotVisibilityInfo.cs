namespace Simulation.SinglePlayer.Visibility
{
	public class BotVisibilityInfo
	{
		public bool isVisible
		{
			get;
			set;
		}

		internal AIAgentDataComponentsNode agentData
		{
			get;
			private set;
		}

		internal BotVisibilityInfo(AIAgentDataComponentsNode obj)
		{
			agentData = obj;
		}
	}
}
