using Svelto.ES.Legacy;

namespace Simulation
{
	internal interface IAlignmentRectifierPausable : IComponent
	{
		void AlignmentRectifierStarted();

		void AlignmentRectifierEnded();
	}
}
