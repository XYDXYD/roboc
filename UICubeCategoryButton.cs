using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class UICubeCategoryButton : MonoBehaviour
{
	public CubeCategory cubeCategory = CubeCategory.Chassis;

	public Transform listenerParent;

	public UICubeCategoryButton()
		: this()
	{
	}

	public void OnClick()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		new SignalChain(listenerParent).Broadcast<CubeCategory>(cubeCategory);
	}
}
