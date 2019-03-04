using Svelto.ECS;
using UnityEngine;

namespace Mothership.TechTree
{
	internal interface ITechTreeItemStateComponent
	{
		GameObject NormalState
		{
			get;
		}

		GameObject LockedState
		{
			get;
		}

		GameObject UnlockableState
		{
			get;
		}

		DispatchOnChange<bool> IsUnLocked
		{
			get;
		}

		DispatchOnChange<bool> IsUnlockable
		{
			get;
		}
	}
}
