using UnityEngine;

internal static class GuiUtil
{
	private static readonly Vector3 OffScreenPos = new Vector3(4000f, 4000f);

	public static Vector3 CalculateHUDPosition(Vector3 worldPos, Vector3 screenPosOffset, Transform relativeTo, bool isInCameraView)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!isInCameraView)
		{
			return OffScreenPos;
		}
		Vector3 val = NGUIMath.WorldToLocalPoint(worldPos, Camera.get_main(), UICamera.get_mainCamera(), relativeTo);
		val += screenPosOffset;
		val.z = 0f;
		return val;
	}
}
