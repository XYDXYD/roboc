using UnityEngine;

internal class RenderQueueSetter : MonoBehaviour
{
	public int GeometryRenderQueueOffset;

	private const int GEOMETRY = 2000;

	public RenderQueueSetter()
		: this()
	{
	}

	private void Start()
	{
		int renderQueue = 2000 + GeometryRenderQueueOffset;
		Renderer[] componentsInChildren = this.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material[] materials = componentsInChildren[i].get_materials();
			for (int j = 0; j < materials.Length; j++)
			{
				materials[j].set_renderQueue(renderQueue);
			}
		}
	}
}
