using System.Collections;

namespace Mothership
{
	internal interface IAutoSaveController
	{
		void FlagDataDirty();

		IEnumerator PerformSave();

		IEnumerator PerformSaveButOnlyIfNecessary();
	}
}
