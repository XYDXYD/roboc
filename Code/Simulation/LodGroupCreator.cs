using UnityEngine;

namespace Simulation
{
	internal class LodGroupCreator : MonoBehaviour
	{
		public LodGroupCreator()
			: this()
		{
		}

		private void Awake()
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			Renderer[] componentsInChildren = this.GetComponentsInChildren<Renderer>();
			LODGroup val = this.get_gameObject().AddComponent<LODGroup>();
			val.SetLODs((LOD[])new LOD[1]
			{
				new LOD(0.03f, componentsInChildren)
			});
			val.RecalculateBounds();
		}
	}
}
