using System;
using System.Collections;
using UnityEngine;

internal sealed class MirrorReflection3 : MonoBehaviour
{
	public bool m_DisablePixelLights = true;

	public int m_TextureSize = 256;

	public float m_ClipPlaneOffset = 0.07f;

	public LayerMask m_ReflectLayers = LayerMask.op_Implicit(-1);

	private Hashtable m_ReflectionCameras = new Hashtable();

	private RenderTexture m_ReflectionTexture;

	private int m_OldReflectionTextureSize;

	private static bool s_InsideRendering;

	public MirrorReflection3()
		: this()
	{
	}//IL_001f: Unknown result type (might be due to invalid IL or missing references)
	//IL_0024: Unknown result type (might be due to invalid IL or missing references)


	public void OnWillRenderObject()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		if (!this.get_enabled() || !Object.op_Implicit(this.GetComponent<Renderer>()) || !Object.op_Implicit(this.GetComponent<Renderer>().get_sharedMaterial()) || !this.GetComponent<Renderer>().get_enabled())
		{
			return;
		}
		Camera current = Camera.get_current();
		if (!Object.op_Implicit(current) || s_InsideRendering)
		{
			return;
		}
		s_InsideRendering = true;
		CreateMirrorObjects(current, out Camera reflectionCamera);
		Vector3 position = this.get_transform().get_position();
		Vector3 up = this.get_transform().get_up();
		int pixelLightCount = QualitySettings.get_pixelLightCount();
		if (m_DisablePixelLights)
		{
			QualitySettings.set_pixelLightCount(0);
		}
		UpdateCameraModes(current, reflectionCamera);
		float num = 0f - Vector3.Dot(up, position) - m_ClipPlaneOffset;
		Vector4 plane = default(Vector4);
		plane._002Ector(up.x, up.y, up.z, num);
		Matrix4x4 reflectionMat = Matrix4x4.get_zero();
		CalculateReflectionMatrix(ref reflectionMat, plane);
		Vector3 position2 = current.get_transform().get_position();
		Vector3 position3 = reflectionMat.MultiplyPoint(position2);
		reflectionCamera.set_worldToCameraMatrix(current.get_worldToCameraMatrix() * reflectionMat);
		Vector4 clipPlane = CameraSpacePlane(reflectionCamera, position, up, 1f);
		Matrix4x4 projection = current.get_projectionMatrix();
		CalculateObliqueMatrix(ref projection, clipPlane);
		reflectionCamera.set_projectionMatrix(projection);
		reflectionCamera.set_cullingMask(-17 & m_ReflectLayers.get_value());
		reflectionCamera.set_targetTexture(m_ReflectionTexture);
		GL.set_invertCulling(true);
		reflectionCamera.get_transform().set_position(position3);
		Vector3 eulerAngles = current.get_transform().get_eulerAngles();
		reflectionCamera.get_transform().set_eulerAngles(new Vector3(0f, eulerAngles.y, eulerAngles.z));
		reflectionCamera.Render();
		reflectionCamera.get_transform().set_position(position2);
		GL.set_invertCulling(false);
		Material[] sharedMaterials = this.GetComponent<Renderer>().get_sharedMaterials();
		Material[] array = sharedMaterials;
		foreach (Material val in array)
		{
			if (val.HasProperty("_ReflectionTex"))
			{
				val.SetTexture("_ReflectionTex", m_ReflectionTexture);
			}
		}
		Matrix4x4 val2 = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.get_identity(), new Vector3(0.5f, 0.5f, 0.5f));
		Vector3 lossyScale = this.get_transform().get_lossyScale();
		Matrix4x4 val3 = this.get_transform().get_localToWorldMatrix() * Matrix4x4.Scale(new Vector3(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z));
		val3 = val2 * current.get_projectionMatrix() * current.get_worldToCameraMatrix() * val3;
		Material[] array2 = sharedMaterials;
		foreach (Material val4 in array2)
		{
			val4.SetMatrix("_ProjMatrix", val3);
		}
		if (m_DisablePixelLights)
		{
			QualitySettings.set_pixelLightCount(pixelLightCount);
		}
		s_InsideRendering = false;
	}

	private void OnDisable()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit(m_ReflectionTexture))
		{
			Object.Destroy(m_ReflectionTexture);
			m_ReflectionTexture = null;
		}
		IDictionaryEnumerator enumerator = m_ReflectionCameras.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Object.Destroy(((DictionaryEntry)enumerator.Current).Value.get_gameObject());
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		m_ReflectionCameras.Clear();
	}

	private void UpdateCameraModes(Camera src, Camera dest)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I4
		if (dest == null)
		{
			return;
		}
		dest.set_clearFlags(1);
		dest.set_backgroundColor(src.get_backgroundColor());
		if ((int)src.get_clearFlags() == 1)
		{
			Skybox val = src.GetComponent(typeof(Skybox)) as Skybox;
			Skybox val2 = dest.GetComponent(typeof(Skybox)) as Skybox;
			if (!Object.op_Implicit(val) || !Object.op_Implicit(val.get_material()))
			{
				val2.set_enabled(false);
			}
			else
			{
				val2.set_enabled(true);
				val2.set_material(val.get_material());
			}
		}
		dest.set_farClipPlane(src.get_farClipPlane());
		dest.set_nearClipPlane(src.get_nearClipPlane());
		dest.set_orthographic(src.get_orthographic());
		dest.set_fieldOfView(src.get_fieldOfView());
		dest.set_aspect(src.get_aspect());
		dest.set_orthographicSize(src.get_orthographicSize());
	}

	private void CreateMirrorObjects(Camera currentCamera, out Camera reflectionCamera)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		reflectionCamera = null;
		if (!Object.op_Implicit(m_ReflectionTexture) || m_OldReflectionTextureSize != m_TextureSize)
		{
			if (Object.op_Implicit(m_ReflectionTexture))
			{
				Object.Destroy(m_ReflectionTexture);
			}
			m_ReflectionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16);
			m_ReflectionTexture.set_name("__MirrorReflection" + this.GetInstanceID());
			m_ReflectionTexture.set_isPowerOfTwo(true);
			m_OldReflectionTextureSize = m_TextureSize;
		}
		reflectionCamera = (m_ReflectionCameras[currentCamera] as Camera);
		if (!Object.op_Implicit(reflectionCamera))
		{
			GameObject val = new GameObject("Mirror Refl Camera id" + this.GetInstanceID() + " for " + currentCamera.GetInstanceID(), new Type[2]
			{
				typeof(Camera),
				typeof(Skybox)
			});
			reflectionCamera = val.GetComponent<Camera>();
			reflectionCamera.set_enabled(false);
			reflectionCamera.get_transform().set_position(this.get_transform().get_position());
			reflectionCamera.get_transform().set_rotation(this.get_transform().get_rotation());
			reflectionCamera.get_gameObject().AddComponent<FlareLayer>();
			val.set_hideFlags(61);
			m_ReflectionCameras[currentCamera] = reflectionCamera;
		}
	}

	private static float sgn(float a)
	{
		if (a > 0f)
		{
			return 1f;
		}
		if (a < 0f)
		{
			return -1f;
		}
		return 0f;
	}

	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = pos + normal * m_ClipPlaneOffset;
		Matrix4x4 worldToCameraMatrix = cam.get_worldToCameraMatrix();
		Vector3 val2 = worldToCameraMatrix.MultiplyPoint(val);
		Vector3 val3 = worldToCameraMatrix.MultiplyVector(normal);
		Vector3 val4 = val3.get_normalized() * sideSign;
		return new Vector4(val4.x, val4.y, val4.z, 0f - Vector3.Dot(val2, val4));
	}

	private static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Vector4 val = projection.get_inverse() * new Vector4(sgn(clipPlane.x), sgn(clipPlane.y), 1f, 1f);
		Vector4 val2 = clipPlane * (2f / Vector4.Dot(clipPlane, val));
		projection.set_Item(2, val2.x - projection.get_Item(3));
		projection.set_Item(6, val2.y - projection.get_Item(7));
		projection.set_Item(10, val2.z - projection.get_Item(11));
		projection.set_Item(14, val2.w - projection.get_Item(15));
	}

	private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
	{
		reflectionMat.m00 = 1f - 2f * plane.get_Item(0) * plane.get_Item(0);
		reflectionMat.m01 = -2f * plane.get_Item(0) * plane.get_Item(1);
		reflectionMat.m02 = -2f * plane.get_Item(0) * plane.get_Item(2);
		reflectionMat.m03 = -2f * plane.get_Item(3) * plane.get_Item(0);
		reflectionMat.m10 = -2f * plane.get_Item(1) * plane.get_Item(0);
		reflectionMat.m11 = 1f - 2f * plane.get_Item(1) * plane.get_Item(1);
		reflectionMat.m12 = -2f * plane.get_Item(1) * plane.get_Item(2);
		reflectionMat.m13 = -2f * plane.get_Item(3) * plane.get_Item(1);
		reflectionMat.m20 = -2f * plane.get_Item(2) * plane.get_Item(0);
		reflectionMat.m21 = -2f * plane.get_Item(2) * plane.get_Item(1);
		reflectionMat.m22 = 1f - 2f * plane.get_Item(2) * plane.get_Item(2);
		reflectionMat.m23 = -2f * plane.get_Item(3) * plane.get_Item(2);
		reflectionMat.m30 = 0f;
		reflectionMat.m31 = 0f;
		reflectionMat.m32 = 0f;
		reflectionMat.m33 = 1f;
	}
}
