using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Robocraft.ImageEffects
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class RobocraftGlobalFog : PostEffectsBase
	{
		[Tooltip("Apply distance-based fog?")]
		public bool distanceFog = true;

		[Tooltip("Exclude far plane pixels from distance-based fog? (Skybox or clear color)")]
		public bool excludeFarPixels = true;

		[Tooltip("Distance fog is based on radial distance from camera when checked")]
		public bool useRadialDistance;

		[Tooltip("Apply height-based fog?")]
		public bool heightFog = true;

		[Tooltip("Fog top Y coordinate")]
		public float height = 1f;

		[Range(0.001f, 10f)]
		public float heightDensity = 2f;

		[Tooltip("Push fog away from the camera by this amount")]
		public float startDistance;

		public Shader fogShader;

		private Material fogMaterial;

		private Camera attachedCamera;

		private bool isFXSupported;

		public RobocraftGlobalFog()
			: this()
		{
		}

		private void OnEnable()
		{
			attachedCamera = this.GetComponent<Camera>();
			isFXSupported = this.CheckResources();
		}

		public override bool CheckResources()
		{
			this.CheckSupport(true);
			fogMaterial = this.CheckShaderAndCreateMaterial(fogShader, fogMaterial);
			if (!base.isSupported)
			{
				this.ReportAutoDisable();
			}
			return base.isSupported;
		}

		[ImageEffectOpaque]
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Invalid comparison between Unknown and I4
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			if (!isFXSupported || (!distanceFog && !heightFog))
			{
				Graphics.Blit(source, destination);
				return;
			}
			Camera val = attachedCamera;
			Transform transform = val.get_transform();
			float nearClipPlane = val.get_nearClipPlane();
			float farClipPlane = val.get_farClipPlane();
			float fieldOfView = val.get_fieldOfView();
			float aspect = val.get_aspect();
			Matrix4x4 identity = Matrix4x4.get_identity();
			float num = fieldOfView * 0.5f;
			Vector3 val2 = transform.get_right() * nearClipPlane * Mathf.Tan(num * ((float)Math.PI / 180f)) * aspect;
			Vector3 val3 = transform.get_up() * nearClipPlane * Mathf.Tan(num * ((float)Math.PI / 180f));
			Vector3 val4 = transform.get_forward() * nearClipPlane - val2 + val3;
			float num2 = val4.get_magnitude() * farClipPlane / nearClipPlane;
			val4.Normalize();
			val4 *= num2;
			Vector3 val5 = transform.get_forward() * nearClipPlane + val2 + val3;
			val5.Normalize();
			val5 *= num2;
			Vector3 val6 = transform.get_forward() * nearClipPlane + val2 - val3;
			val6.Normalize();
			val6 *= num2;
			Vector3 val7 = transform.get_forward() * nearClipPlane - val2 - val3;
			val7.Normalize();
			val7 *= num2;
			identity.SetRow(0, Vector4.op_Implicit(val4));
			identity.SetRow(1, Vector4.op_Implicit(val5));
			identity.SetRow(2, Vector4.op_Implicit(val6));
			identity.SetRow(3, Vector4.op_Implicit(val7));
			Vector3 position = transform.get_position();
			float num3 = position.y - height;
			float num4 = (!(num3 <= 0f)) ? 0f : 1f;
			float num5 = (!excludeFarPixels) ? 2f : 1f;
			fogMaterial.SetMatrix("_FrustumCornersWS", identity);
			fogMaterial.SetVector("_CameraWS", Vector4.op_Implicit(position));
			fogMaterial.SetVector("_HeightParams", new Vector4(height, num3, num4, heightDensity * 0.5f));
			fogMaterial.SetVector("_DistanceParams", new Vector4(0f - Mathf.Max(startDistance, 0f), num5, 0f, 0f));
			FogMode fogMode = RenderSettings.get_fogMode();
			float fogDensity = RenderSettings.get_fogDensity();
			float fogStartDistance = RenderSettings.get_fogStartDistance();
			float fogEndDistance = RenderSettings.get_fogEndDistance();
			bool flag = (int)fogMode == 1;
			float num6 = (!flag) ? 0f : (fogEndDistance - fogStartDistance);
			float num7 = (!(Mathf.Abs(num6) > 0.0001f)) ? 0f : (1f / num6);
			Vector4 val8 = default(Vector4);
			val8.x = fogDensity * 1.2011224f;
			val8.y = fogDensity * 1.442695f;
			val8.z = ((!flag) ? 0f : (0f - num7));
			val8.w = ((!flag) ? 0f : (fogEndDistance * num7));
			fogMaterial.SetVector("_SceneFogParams", val8);
			fogMaterial.SetVector("_SceneFogMode", new Vector4((float)fogMode, (float)(useRadialDistance ? 1 : 0), 0f, 0f));
			int num8 = 0;
			CustomGraphicsBlit(passNr: (!distanceFog || !heightFog) ? (distanceFog ? 1 : 2) : 0, source: source, dest: destination, fxMaterial: fogMaterial);
		}

		private static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material fxMaterial, int passNr)
		{
			RenderTexture.set_active(dest);
			fxMaterial.SetTexture("_MainTex", source);
			GL.PushMatrix();
			GL.LoadOrtho();
			fxMaterial.SetPass(passNr);
			GL.Begin(7);
			GL.MultiTexCoord2(0, 0f, 0f);
			GL.Vertex3(0f, 0f, 3f);
			GL.MultiTexCoord2(0, 1f, 0f);
			GL.Vertex3(1f, 0f, 2f);
			GL.MultiTexCoord2(0, 1f, 1f);
			GL.Vertex3(1f, 1f, 1f);
			GL.MultiTexCoord2(0, 0f, 1f);
			GL.Vertex3(0f, 1f, 0f);
			GL.End();
			GL.PopMatrix();
		}
	}
}
