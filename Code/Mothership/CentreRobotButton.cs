using Svelto.Command;
using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal class CentreRobotButton : MonoBehaviour
	{
		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		public CentreRobotButton()
			: this()
		{
		}

		private void OnClick()
		{
			commandFactory.Build<CentreRobotCommand>().Execute();
		}
	}
}
