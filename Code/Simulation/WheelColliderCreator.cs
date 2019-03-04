using UnityEngine;

namespace Simulation
{
	internal class WheelColliderCreator : MonoBehaviour, IWheelColliderComponent
	{
		[Tooltip("In order of importance")]
		public WheelColliderData[] wheelObj;

		public WheelColliderData[] wheelData => wheelObj;

		public WheelColliderCreator()
			: this()
		{
		}

		private void Awake()
		{
			IWheelColliderInfo[] components = this.GetComponents<IWheelColliderInfo>();
			for (int i = 0; i < wheelObj.Length; i++)
			{
				WheelColliderData wheelColliderData = wheelObj[i];
				wheelColliderData.cubeRoot = this.get_transform();
				wheelColliderData.priority = i;
				for (int j = 0; j < components.Length; j++)
				{
					components[j].SetWheelColliderInfo(wheelColliderData);
				}
				wheelColliderData.wheelColliderInfo.AddRange(components);
			}
		}
	}
}
