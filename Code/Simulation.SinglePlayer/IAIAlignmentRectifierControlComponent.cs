using Svelto.ECS.Legacy;

namespace Simulation.SinglePlayer
{
	public interface IAIAlignmentRectifierControlComponent
	{
		float lastCompletedAlignmentRectifierTimestamp
		{
			get;
		}

		Dispatcher<IAIAlignmentRectifierControlComponent, int> alignmentRectifierStarted
		{
			get;
		}

		Dispatcher<IAIAlignmentRectifierControlComponent, float> alignmentRectifierComplete
		{
			get;
		}

		bool pulseAR
		{
			get;
		}

		bool alignmentRectifierExecuting
		{
			get;
			set;
		}

		float horizontalAxis
		{
			get;
		}

		float forwardAxis
		{
			get;
		}
	}
}
