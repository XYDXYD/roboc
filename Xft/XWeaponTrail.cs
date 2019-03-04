using Svelto.Tasks.Enumerators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Xft
{
	public class XWeaponTrail : MonoBehaviour, IDisableOffScreen
	{
		public class Element
		{
			public Vector3 PointStart;

			public Vector3 PointEnd;

			public Vector3 Pos => (PointStart + PointEnd) / 2f;

			public Element(Vector3 start, Vector3 end)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				PointStart = start;
				PointEnd = end;
			}

			public Element()
			{
			}
		}

		public static string Version = "1.0.1";

		public Transform PointStart;

		public Transform PointEnd;

		private int MaxFrame = 36;

		private int Granularity = 20;

		private float Fps = 60f;

		public float updateUVInterval = 0.5f;

		public float scaleUV = 1f;

		public float updateUVspeed = 1f;

		public int roundValue = 3;

		public Color MyColor = Color.get_white();

		public Material MyMaterial;

		public float uvTile;

		protected float mTrailWidth;

		protected Element mHeadElem = new Element();

		protected List<Element> mSnapshotList = new List<Element>();

		protected Spline mSpline = new Spline();

		protected float mFadeT = 1f;

		protected bool mIsFading;

		protected float mFadeTime = 1f;

		protected float mElapsedTime;

		protected float mFadeElapsedime;

		protected GameObject mMeshObj;

		protected MeshRenderer mMeshRenderer;

		protected VertexPool mVertexPool;

		protected VertexPool.VertexSegment mVertexSegment;

		protected bool mInited;

		public GameObject MMeshObj
		{
			get
			{
				return mMeshObj;
			}
			set
			{
				mMeshObj = value;
			}
		}

		public float UpdateInterval => 1f / Fps;

		public Vector3 CurHeadPos => (PointStart.get_position() + PointEnd.get_position()) / 2f;

		public float TrailWidth => mTrailWidth;

		public XWeaponTrail()
			: this()
		{
		}//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)


		public void Init()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			if (!mInited)
			{
				Vector3 val = PointStart.get_position() - PointEnd.get_position();
				mTrailWidth = val.get_magnitude();
				InitMeshObj();
				InitOriginalElements();
				InitSpline();
				mInited = true;
			}
		}

		public void Activate()
		{
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			Init();
			if (mMeshObj == null)
			{
				InitMeshObj();
				return;
			}
			this.get_gameObject().SetActive(true);
			if (mMeshObj != null)
			{
				mMeshObj.SetActive(true);
			}
			mFadeT = 1f;
			mIsFading = false;
			mFadeTime = 1f;
			mFadeElapsedime = 0f;
			mElapsedTime = 0f;
			for (int i = 0; i < mSnapshotList.Count; i++)
			{
				mSnapshotList[i].PointStart = PointStart.get_position();
				mSnapshotList[i].PointEnd = PointEnd.get_position();
				mSpline.ControlPoints[i].Position = mSnapshotList[i].Pos;
				mSpline.ControlPoints[i].Normal = mSnapshotList[i].PointEnd - mSnapshotList[i].PointStart;
			}
			RefreshSpline();
			UpdateVertex();
		}

		public void Deactivate()
		{
			this.get_gameObject().SetActive(false);
			if (mMeshObj != null)
			{
				mMeshObj.SetActive(false);
			}
		}

		public void StopSmoothly(float fadeTime)
		{
			mIsFading = true;
			mFadeTime = fadeTime;
		}

		private void Update()
		{
			if (!mInited)
			{
				return;
			}
			if (mMeshObj == null)
			{
				InitMeshObj();
				return;
			}
			UpdateHeadElem();
			mElapsedTime += Time.get_deltaTime();
			if (!(mElapsedTime < UpdateInterval))
			{
				mElapsedTime -= UpdateInterval;
				RecordCurElem();
				RefreshSpline();
				UpdateFade();
				UpdateVertex();
			}
		}

		private void LateUpdate()
		{
			if (mInited)
			{
				mVertexPool.LateUpdate();
			}
		}

		private void Start()
		{
			Init();
		}

		private void OnEnable()
		{
			InitOriginalElements();
			this.StartCoroutine(TileUVs());
		}

		private void OnDisable()
		{
			this.StopAllCoroutines();
			if (mMeshRenderer != null)
			{
				mMeshRenderer.set_enabled(false);
			}
		}

		private void OnDestroy()
		{
			Object.Destroy(mMeshObj);
		}

		private void OnDrawGizmos()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			if (!(PointEnd == null) && !(PointStart == null))
			{
				Vector3 val = PointStart.get_position() - PointEnd.get_position();
				float magnitude = val.get_magnitude();
				if (!(magnitude < Mathf.Epsilon))
				{
					Gizmos.set_color(Color.get_red());
					Gizmos.DrawSphere(PointStart.get_position(), magnitude * 0.04f);
					Gizmos.set_color(Color.get_blue());
					Gizmos.DrawSphere(PointEnd.get_position(), magnitude * 0.04f);
				}
			}
		}

		private void InitSpline()
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			mSpline.Granularity = Granularity;
			mSpline.Clear();
			for (int i = 0; i < MaxFrame; i++)
			{
				mSpline.AddControlPoint(CurHeadPos, PointStart.get_position() - PointEnd.get_position());
			}
		}

		private void RefreshSpline()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < mSnapshotList.Count; i++)
			{
				mSpline.ControlPoints[i].Position = mSnapshotList[i].Pos;
				mSpline.ControlPoints[i].Normal = mSnapshotList[i].PointEnd - mSnapshotList[i].PointStart;
			}
			mSpline.RefreshSpline();
		}

		private IEnumerator TileUVs()
		{
			while (mSnapshotList.Count < MaxFrame)
			{
				yield return (object)new WaitForSecondsEnumerator(0.3f);
			}
			if (mMeshRenderer != null)
			{
				mMeshRenderer.set_enabled(true);
			}
			else
			{
				Console.LogWarning("Flag mesh not initiated");
			}
			while (true)
			{
				float t = 0f;
				float uvTempTile = Vector3.Distance(mSnapshotList[mSnapshotList.Count - 1].Pos, mSnapshotList[mSnapshotList.Count - 2].Pos);
				for (int num = mSnapshotList.Count - 1; num >= 1; num--)
				{
					float num2 = Vector3.Distance(mSnapshotList[num].Pos, mSnapshotList[num - 1].Pos);
					if (num2 > 0f)
					{
						uvTempTile += num2;
					}
				}
				uvTile = uvTempTile * scaleUV + 1.5f;
				if (uvTile > 3f)
				{
					int newStretch = Mathf.RoundToInt(uvTile / (float)roundValue) * roundValue;
					while (t < 1f)
					{
						t += Time.get_deltaTime() * updateUVspeed;
						Material meshMaterial = mMeshRenderer.get_material();
						Vector2 textureScale = meshMaterial.GetTextureScale("_MainTex");
						float lerpUV = Mathf.Lerp(textureScale.x, (float)newStretch, t);
						meshMaterial.SetTextureScale("_MainTex", new Vector2(lerpUV, 1f));
						yield return null;
					}
				}
				yield return (object)new WaitForSecondsEnumerator(updateUVInterval);
			}
		}

		private void UpdateVertex()
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			VertexPool pool = mVertexSegment.Pool;
			for (int i = 0; i < Granularity; i++)
			{
				int num = mVertexSegment.VertStart + i * 3;
				float num2 = (float)i / (float)Granularity;
				float tl = num2 * mFadeT;
				Vector2 zero = Vector2.get_zero();
				Vector2 zero2 = Vector2.get_zero();
				Vector3 val = mSpline.InterpolateByLen(tl);
				Vector3 val2 = mSpline.InterpolateNormalByLen(tl);
				Vector3 val3 = val - val2.get_normalized() * mTrailWidth * 0.5f;
				Vector3 val4 = val + val2.get_normalized() * mTrailWidth * 0.5f;
				pool.Vertices[num] = val3;
				pool.Colors[num] = MyColor;
				zero.y = (zero2.y = 0f);
				zero.x = (zero2.x = num2);
				pool.UVs[num] = zero;
				pool.UVs2[num] = zero2;
				pool.Vertices[num + 1] = val;
				pool.Colors[num + 1] = MyColor;
				zero.y = (zero2.y = 0.5f);
				zero.x = (zero2.x = num2);
				pool.UVs[num + 1] = zero;
				pool.UVs2[num + 1] = zero2;
				pool.Vertices[num + 2] = val4;
				pool.Colors[num + 2] = MyColor;
				zero.y = (zero2.y = 1f);
				zero.x = (zero2.x = num2);
				pool.UVs[num + 2] = zero;
				pool.UVs2[num + 2] = zero2;
			}
			mVertexSegment.Pool.UVChanged = true;
			mVertexSegment.Pool.UV2Changed = true;
			mVertexSegment.Pool.VertChanged = true;
			mVertexSegment.Pool.ColorChanged = true;
		}

		private void UpdateIndices()
		{
			VertexPool pool = mVertexSegment.Pool;
			for (int i = 0; i < Granularity - 1; i++)
			{
				int num = mVertexSegment.VertStart + i * 3;
				int num2 = mVertexSegment.VertStart + (i + 1) * 3;
				int num3 = mVertexSegment.IndexStart + i * 12;
				pool.Indices[num3] = num2;
				pool.Indices[num3 + 1] = num2 + 1;
				pool.Indices[num3 + 2] = num;
				pool.Indices[num3 + 3] = num2 + 1;
				pool.Indices[num3 + 4] = num + 1;
				pool.Indices[num3 + 5] = num;
				pool.Indices[num3 + 6] = num2 + 1;
				pool.Indices[num3 + 7] = num2 + 2;
				pool.Indices[num3 + 8] = num + 1;
				pool.Indices[num3 + 9] = num2 + 2;
				pool.Indices[num3 + 10] = num + 2;
				pool.Indices[num3 + 11] = num + 1;
			}
			pool.IndiceChanged = true;
		}

		private void UpdateHeadElem()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			mSnapshotList[0].PointStart = PointStart.get_position();
			mSnapshotList[0].PointEnd = PointEnd.get_position();
		}

		private void UpdateFade()
		{
			if (mIsFading)
			{
				mFadeElapsedime += Time.get_deltaTime();
				float num = mFadeElapsedime / mFadeTime;
				mFadeT = 1f - num;
				if (mFadeT < 0f)
				{
					Deactivate();
				}
			}
		}

		private void RecordCurElem()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			Element item = new Element(PointStart.get_position(), PointEnd.get_position());
			if (mSnapshotList.Count < MaxFrame)
			{
				mSnapshotList.Insert(1, item);
				return;
			}
			mSnapshotList.RemoveAt(mSnapshotList.Count - 1);
			mSnapshotList.Insert(1, item);
		}

		private void InitOriginalElements()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			mSnapshotList.Clear();
			mSnapshotList.Add(new Element(PointStart.get_position(), PointEnd.get_position()));
			mSnapshotList.Add(new Element(PointStart.get_position(), PointEnd.get_position()));
		}

		private void InitMeshObj()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Expected O, but got Unknown
			mMeshObj = new GameObject("_XWeaponTrailMesh: " + this.get_gameObject().get_name());
			mMeshObj.set_layer(this.get_gameObject().get_layer());
			mMeshObj.SetActive(true);
			MeshFilter val = mMeshObj.AddComponent<MeshFilter>();
			mMeshRenderer = mMeshObj.AddComponent<MeshRenderer>();
			mMeshRenderer.set_shadowCastingMode(0);
			mMeshRenderer.set_receiveShadows(false);
			mMeshRenderer.set_sharedMaterial(MyMaterial);
			mMeshRenderer.set_enabled(false);
			val.set_sharedMesh(new Mesh());
			mVertexPool = new VertexPool(val.get_sharedMesh(), MyMaterial);
			mVertexSegment = mVertexPool.GetVertices(Granularity * 3, (Granularity - 1) * 12);
			UpdateIndices();
		}
	}
}
