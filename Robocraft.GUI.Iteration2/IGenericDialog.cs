using System;
using System.Collections;

namespace Robocraft.GUI.Iteration2
{
	internal interface IGenericDialog
	{
		IEnumerator Prompt(Action<GenericDialogChoice> choiceCb);
	}
}
