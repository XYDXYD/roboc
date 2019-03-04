using UnityEngine;

namespace Mothership.TechTree
{
	internal interface ITechTreeViewComponent
	{
		GameObject TemplateItem
		{
			get;
		}

		Transform TreeRoot
		{
			get;
		}

		float GridScale
		{
			get;
		}

		UIButton BackButton
		{
			get;
		}
	}
}
