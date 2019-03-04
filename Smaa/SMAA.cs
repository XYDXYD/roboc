using UnityEngine;

namespace Smaa
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Image Effects/Subpixel Morphological Antialiasing")]
	public class SMAA : MonoBehaviour
	{
		public HDRMode Hdr;

		public DebugPass DebugPass;

		public QualityPreset Quality = QualityPreset.High;

		public EdgeDetectionMethod DetectionMethod = EdgeDetectionMethod.Luma;

		public bool UsePredication;

		public Preset CustomPreset;

		public PredicationPreset CustomPredicationPreset;

		public Shader Shader;

		public Texture2D AreaTex;

		public Texture2D SearchTex;

		protected Camera m_Camera;

		protected Preset[] m_StdPresets;

		protected Material m_Material;

		public Material Material
		{
			get
			{
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Expected O, but got Unknown
				if (m_Material == null)
				{
					m_Material = new Material(Shader);
					m_Material.set_hideFlags(61);
				}
				return m_Material;
			}
		}

		public SMAA()
			: this()
		{
		}

		private void OnEnable()
		{
			if (AreaTex == null)
			{
				AreaTex = Resources.Load<Texture2D>("AreaTex");
			}
			if (SearchTex == null)
			{
				SearchTex = Resources.Load<Texture2D>("SearchTex");
			}
			m_Camera = this.GetComponent<Camera>();
			CreatePresets();
		}

		private void Start()
		{
			if (!SystemInfo.get_supportsImageEffects())
			{
				this.set_enabled(false);
			}
			else if (!Object.op_Implicit(Shader) || !Shader.get_isSupported())
			{
				this.set_enabled(false);
			}
		}

		private void OnDisable()
		{
			if (m_Material != null)
			{
				Object.Destroy(m_Material);
			}
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			int pixelWidth = m_Camera.get_pixelWidth();
			int pixelHeight = m_Camera.get_pixelHeight();
			Preset preset = CustomPreset;
			if (Quality != QualityPreset.Custom)
			{
				preset = m_StdPresets[(int)Quality];
			}
			int detectionMethod = (int)DetectionMethod;
			int num = 4;
			int num2 = 5;
			RenderTextureFormat format = source.get_format();
			if (Hdr == HDRMode.Off)
			{
				format = 0;
			}
			else if (Hdr == HDRMode.On)
			{
				format = 2;
			}
			Material.SetTexture("_AreaTex", AreaTex);
			Material.SetTexture("_SearchTex", SearchTex);
			Material.SetVector("_Metrics", new Vector4(1f / (float)pixelWidth, 1f / (float)pixelHeight, (float)pixelWidth, (float)pixelHeight));
			Material.SetVector("_Params1", new Vector4(preset.Threshold, preset.DepthThreshold, (float)preset.MaxSearchSteps, (float)preset.MaxSearchStepsDiag));
			Material.SetVector("_Params2", Vector4.op_Implicit(new Vector2((float)preset.CornerRounding, preset.LocalContrastAdaptationFactor)));
			Shader.DisableKeyword("USE_PREDICATION");
			if (DetectionMethod == EdgeDetectionMethod.Depth)
			{
				Camera camera = m_Camera;
				camera.set_depthTextureMode(camera.get_depthTextureMode() | 1);
			}
			else if (UsePredication)
			{
				Camera camera2 = m_Camera;
				camera2.set_depthTextureMode(camera2.get_depthTextureMode() | 1);
				Shader.EnableKeyword("USE_PREDICATION");
				Material.SetVector("_Params3", Vector4.op_Implicit(new Vector3(CustomPredicationPreset.Threshold, CustomPredicationPreset.Scale, CustomPredicationPreset.Strength)));
			}
			Shader.DisableKeyword("USE_DIAG_SEARCH");
			Shader.DisableKeyword("USE_CORNER_DETECTION");
			if (preset.DiagDetection)
			{
				Shader.EnableKeyword("USE_DIAG_SEARCH");
			}
			if (preset.CornerDetection)
			{
				Shader.EnableKeyword("USE_CORNER_DETECTION");
			}
			RenderTexture val = TempRT(pixelWidth, pixelHeight, format);
			RenderTexture val2 = TempRT(pixelWidth, pixelHeight, format);
			Clear(val);
			Clear(val2);
			Graphics.Blit(source, val, Material, detectionMethod);
			if (DebugPass == DebugPass.Edges)
			{
				Graphics.Blit(val, destination);
			}
			else
			{
				Graphics.Blit(val, val2, Material, num);
				if (DebugPass == DebugPass.Weights)
				{
					Graphics.Blit(val2, destination);
				}
				else
				{
					Material.SetTexture("_BlendTex", val2);
					Graphics.Blit(source, destination, Material, num2);
				}
			}
			RenderTexture.ReleaseTemporary(val);
			RenderTexture.ReleaseTemporary(val2);
		}

		private void Clear(RenderTexture rt)
		{
			Graphics.Blit(rt, rt, Material, 0);
		}

		private RenderTexture TempRT(int width, int height, RenderTextureFormat format)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			return RenderTexture.GetTemporary(width, height, num, format, 1);
		}

		private void CreatePresets()
		{
			m_StdPresets = new Preset[4];
			m_StdPresets[0] = new Preset
			{
				Threshold = 0.15f,
				MaxSearchSteps = 4
			};
			m_StdPresets[0].DiagDetection = false;
			m_StdPresets[0].CornerDetection = false;
			m_StdPresets[1] = new Preset
			{
				Threshold = 0.1f,
				MaxSearchSteps = 8
			};
			m_StdPresets[1].DiagDetection = false;
			m_StdPresets[1].CornerDetection = false;
			m_StdPresets[2] = new Preset
			{
				Threshold = 0.1f,
				MaxSearchSteps = 16,
				MaxSearchStepsDiag = 8,
				CornerRounding = 25
			};
			m_StdPresets[3] = new Preset
			{
				Threshold = 0.05f,
				MaxSearchSteps = 32,
				MaxSearchStepsDiag = 16,
				CornerRounding = 25
			};
		}
	}
}
