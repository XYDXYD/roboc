namespace Simulation
{
	internal class SoundParameterData
	{
		public int nodeId;

		public float soundParameter;

		public void SetValues(int nodeId_, float soundParameter_)
		{
			nodeId = nodeId_;
			soundParameter = soundParameter_;
		}
	}
}
