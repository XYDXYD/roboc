using System.Collections.ObjectModel;
using UnityEngine;

[AddComponentMenu("Network/Utilities/EdsConsole")]
internal sealed class EdsConsoleRender : MonoBehaviour
{
	public int lineHeight = 16;

	public Vector2 position = new Vector2(300f, 0f);

	public Vector2 size = new Vector2(400f, 200f);

	public EdsConsoleRender()
		: this()
	{
	}//IL_0013: Unknown result type (might be due to invalid IL or missing references)
	//IL_0018: Unknown result type (might be due to invalid IL or missing references)
	//IL_0028: Unknown result type (might be due to invalid IL or missing references)
	//IL_002d: Unknown result type (might be due to invalid IL or missing references)


	private void OnGUI()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		ReadOnlyCollection<string> data = EdsConsole.GetData();
		for (int i = 0; i < data.Count; i++)
		{
			GUI.Label(new Rect(position.x, position.y + (float)(lineHeight * i), size.x, size.y), data[i]);
		}
	}
}
