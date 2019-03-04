using Svelto.Command;
using Svelto.IoC;

namespace Mothership
{
	internal sealed class SwitchToGarageCommand : IInjectableCommand<SwitchWorldDependency>, ICommand
	{
		private SwitchWorldDependency _dependency;

		[Inject]
		public WorldSwitching worldSwitching
		{
			get;
			private set;
		}

		public ICommand Inject(SwitchWorldDependency dependency)
		{
			_dependency = dependency;
			return this;
		}

		public void Execute()
		{
			worldSwitching.SwitchToGarageFromBuildMode(_dependency);
		}
	}
}
