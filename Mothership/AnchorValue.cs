using UnityEngine;

namespace Mothership
{
	internal class AnchorValue
	{
		public readonly Transform tranform;

		public readonly float absolute;

		public readonly float relative;

		public AnchorValue(Transform tranform_, float relative_, float absolute_)
		{
			tranform = tranform_;
			relative = relative_;
			absolute = absolute_;
		}
	}
}
