using System.Collections;
using UnityEngine;

namespace Mothership
{
	internal interface IMothershipPropPresenter
	{
		void SetView(MothershipPropActivator view);

		Vector3 GetBayCentre();

		IEnumerator LoadInitialState();

		void SetRobotShopName(string name);

		MothershipPropState PushThumbnailRenderState();

		void PopThumbnailRenderState(MothershipPropState prevState);
	}
}
