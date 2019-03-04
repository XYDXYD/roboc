using UnityEngine;
using UnityEngine.Rendering;

namespace Kino
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Kino Image Effects/Obscurance")]
	public class Obscurance : MonoBehaviour
	{
		public enum SampleCount
		{
			Lowest,
			Low,
			Medium,
			High,
			Custom
		}

		public enum OcclusionSource
		{
			DepthTexture,
			DepthNormalsTexture,
			GBuffer
		}

		private class PropertyObserver
		{
			private bool _downsampling;

			private OcclusionSource _occlusionSource;

			private bool _ambientOnly;

			private int _pixelWidth;

			private int _pixelHeight;

			public bool CheckNeedsReset(Obscurance target, Camera camera)
			{
				return _downsampling != target.downsampling || _occlusionSource != target.occlusionSource || _ambientOnly != target.ambientOnly || _pixelWidth != camera.get_pixelWidth() || _pixelHeight != camera.get_pixelHeight();
			}

			public void Update(Obscurance target, Camera camera)
			{
				_downsampling = target.downsampling;
				_occlusionSource = target.occlusionSource;
				_ambientOnly = target.ambientOnly;
				_pixelWidth = camera.get_pixelWidth();
				_pixelHeight = camera.get_pixelHeight();
			}
		}

		[SerializeField]
		[Range(0f, 4f)]
		[Tooltip("Degree of darkness produced by the effect.")]
		private float _intensity = 1f;

		[SerializeField]
		[Tooltip("Radius of sample points, which affects extent of darkened areas.")]
		private float _radius = 0.3f;

		[SerializeField]
		[Tooltip("Number of sample points, which affects quality and performance.")]
		private SampleCount _sampleCount = SampleCount.Medium;

		[SerializeField]
		private int _sampleCountValue = 24;

		[SerializeField]
		[Tooltip("Halves the resolution of the effect to increase performance.")]
		private bool _downsampling;

		[SerializeField]
		[Tooltip("Source buffer used for obscurance estimation")]
		private OcclusionSource _occlusionSource = OcclusionSource.GBuffer;

		[SerializeField]
		[Tooltip("If checked, the effect only affects ambient lighting.")]
		private bool _ambientOnly;

		[SerializeField]
		private Shader _aoShader;

		private Material _aoMaterial;

		private CommandBuffer _aoCommands;

		private PropertyObserver _propertyObserver = new PropertyObserver();

		[SerializeField]
		private Mesh _quadMesh;

		public float intensity
		{
			get
			{
				return _intensity;
			}
			set
			{
				_intensity = value;
			}
		}

		public float radius
		{
			get
			{
				return Mathf.Max(_radius, 0.0001f);
			}
			set
			{
				_radius = value;
			}
		}

		public SampleCount sampleCount
		{
			get
			{
				return _sampleCount;
			}
			set
			{
				_sampleCount = value;
			}
		}

		public int sampleCountValue
		{
			get
			{
				switch (_sampleCount)
				{
				case SampleCount.Lowest:
					return 3;
				case SampleCount.Low:
					return 6;
				case SampleCount.Medium:
					return 12;
				case SampleCount.High:
					return 20;
				default:
					return Mathf.Clamp(_sampleCountValue, 1, 256);
				}
			}
			set
			{
				_sampleCountValue = value;
			}
		}

		public bool downsampling
		{
			get
			{
				return _downsampling;
			}
			set
			{
				_downsampling = value;
			}
		}

		public OcclusionSource occlusionSource
		{
			get
			{
				if (_occlusionSource == OcclusionSource.GBuffer && !IsGBufferAvailable)
				{
					return OcclusionSource.DepthNormalsTexture;
				}
				return _occlusionSource;
			}
			set
			{
				_occlusionSource = value;
			}
		}

		public bool ambientOnly
		{
			get
			{
				return _ambientOnly && targetCamera.get_hdr() && occlusionSource == OcclusionSource.GBuffer;
			}
			set
			{
				_ambientOnly = value;
			}
		}

		private Material aoMaterial
		{
			get
			{
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Expected O, but got Unknown
				if (_aoMaterial == null)
				{
					Shader val = Shader.Find("Hidden/Kino/Obscurance");
					_aoMaterial = new Material(val);
					_aoMaterial.set_hideFlags(52);
				}
				return _aoMaterial;
			}
		}

		private CommandBuffer aoCommands
		{
			get
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				if (_aoCommands == null)
				{
					_aoCommands = new CommandBuffer();
					_aoCommands.set_name("Kino.Obscurance");
				}
				return _aoCommands;
			}
		}

		private Camera targetCamera => this.GetComponent<Camera>();

		private PropertyObserver propertyObserver => _propertyObserver;

		private bool IsGBufferAvailable
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Invalid comparison between Unknown and I4
				RenderingPath actualRenderingPath = targetCamera.get_actualRenderingPath();
				return (int)actualRenderingPath == 3;
			}
		}

		public Obscurance()
			: this()
		{
		}

		private void BuildAOCommands()
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			CommandBuffer aoCommands = this.aoCommands;
			int pixelWidth = targetCamera.get_pixelWidth();
			int pixelHeight = targetCamera.get_pixelHeight();
			int num = (!downsampling) ? 1 : 2;
			RenderTextureFormat val = 0;
			RenderTextureReadWrite val2 = 1;
			FilterMode val3 = 1;
			Material aoMaterial = this.aoMaterial;
			int num2 = Shader.PropertyToID("_OcclusionTexture1");
			aoCommands.GetTemporaryRT(num2, pixelWidth / num, pixelHeight / num, 0, val3, val, val2);
			aoCommands.Blit(null, RenderTargetIdentifier.op_Implicit(num2), aoMaterial, 2);
			int num3 = Shader.PropertyToID("_OcclusionTexture2");
			aoCommands.GetTemporaryRT(num3, pixelWidth, pixelHeight, 0, val3, val, val2);
			aoCommands.Blit(RenderTargetIdentifier.op_Implicit(num2), RenderTargetIdentifier.op_Implicit(num3), aoMaterial, 4);
			aoCommands.ReleaseTemporaryRT(num2);
			num2 = Shader.PropertyToID("_OcclusionTexture");
			aoCommands.GetTemporaryRT(num2, pixelWidth, pixelHeight, 0, val3, val, val2);
			aoCommands.Blit(RenderTargetIdentifier.op_Implicit(num3), RenderTargetIdentifier.op_Implicit(num2), aoMaterial, 5);
			aoCommands.ReleaseTemporaryRT(num3);
			RenderTargetIdentifier[] array = (RenderTargetIdentifier[])new RenderTargetIdentifier[2]
			{
				RenderTargetIdentifier.op_Implicit(10),
				RenderTargetIdentifier.op_Implicit(2)
			};
			aoCommands.SetRenderTarget(array, RenderTargetIdentifier.op_Implicit(2));
			aoCommands.DrawMesh(_quadMesh, Matrix4x4.get_identity(), aoMaterial, 0, 7);
			aoCommands.ReleaseTemporaryRT(num2);
		}

		private void ExecuteAOPass(RenderTexture source, RenderTexture destination)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			int width = source.get_width();
			int height = source.get_height();
			int num = (!downsampling) ? 1 : 2;
			RenderTextureFormat val = 0;
			RenderTextureReadWrite val2 = 1;
			bool flag = occlusionSource == OcclusionSource.GBuffer;
			Material aoMaterial = this.aoMaterial;
			RenderTexture temporary = RenderTexture.GetTemporary(width / num, height / num, 0, val, val2);
			Graphics.Blit(source, temporary, aoMaterial, (int)occlusionSource);
			RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, 0, val, val2);
			Graphics.Blit(temporary, temporary2, aoMaterial, (!flag) ? 3 : 4);
			RenderTexture.ReleaseTemporary(temporary);
			temporary = RenderTexture.GetTemporary(width, height, 0, val, val2);
			Graphics.Blit(temporary2, temporary, aoMaterial, 5);
			RenderTexture.ReleaseTemporary(temporary2);
			aoMaterial.SetTexture("_OcclusionTexture", temporary);
			Graphics.Blit(source, destination, aoMaterial, 6);
			RenderTexture.ReleaseTemporary(temporary);
			aoMaterial.SetTexture("_OcclusionTexture", null);
		}

		private void UpdateMaterialProperties()
		{
			Material aoMaterial = this.aoMaterial;
			aoMaterial.SetFloat("_Intensity", intensity);
			aoMaterial.SetFloat("_Radius", radius);
			aoMaterial.SetFloat("_Downsample", (!downsampling) ? 1f : 0.5f);
			aoMaterial.SetInt("_SampleCount", sampleCountValue);
		}

		private void OnEnable()
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			if (ambientOnly)
			{
				this.targetCamera.AddCommandBuffer(21, aoCommands);
			}
			if (occlusionSource == OcclusionSource.DepthTexture)
			{
				Camera targetCamera = this.targetCamera;
				targetCamera.set_depthTextureMode(targetCamera.get_depthTextureMode() | 1);
			}
			if (occlusionSource != OcclusionSource.GBuffer)
			{
				Camera targetCamera2 = this.targetCamera;
				targetCamera2.set_depthTextureMode(targetCamera2.get_depthTextureMode() | 2);
			}
		}

		private void OnDisable()
		{
			if (_aoCommands != null)
			{
				targetCamera.RemoveCommandBuffer(21, _aoCommands);
			}
		}

		private void OnDestroy()
		{
			if (Application.get_isPlaying())
			{
				Object.Destroy(_aoMaterial);
			}
			else
			{
				Object.DestroyImmediate(_aoMaterial);
			}
		}

		private void OnPreRender()
		{
			if (propertyObserver.CheckNeedsReset(this, targetCamera))
			{
				OnDisable();
				OnEnable();
				if (ambientOnly)
				{
					aoCommands.Clear();
					BuildAOCommands();
				}
				propertyObserver.Update(this, targetCamera);
			}
			if (ambientOnly)
			{
				UpdateMaterialProperties();
			}
		}

		[ImageEffectOpaque]
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (ambientOnly)
			{
				Graphics.Blit(source, destination);
				return;
			}
			UpdateMaterialProperties();
			ExecuteAOPass(source, destination);
		}
	}
}
