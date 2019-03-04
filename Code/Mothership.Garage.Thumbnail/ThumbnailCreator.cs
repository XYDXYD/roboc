using Svelto.IoC;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership.Garage.Thumbnail
{
	internal sealed class ThumbnailCreator : MonoBehaviour, IInitialize
	{
		public enum Positioning
		{
			CopyGameCameraTransformation,
			AutoTargetRobot,
			KeepPrevious
		}

		[SerializeField]
		private Camera ThumbnailCamera;

		private const int scaleUpSize = 4;

		private RenderTexture _renderTexture;

		private Texture2D _ScaledUpThumbnail;

		private Vector3 _minMachinePos;

		private Vector3 _maxMachinePos;

		private const int OFFSET = 1000;

		private Transform _machine;

		[Inject]
		internal RobotShopSubmissionController submissioncontroller
		{
			private get;
			set;
		}

		[Inject]
		internal ThumbnailManager thumbnailManager
		{
			private get;
			set;
		}

		[Inject]
		internal IMachineMap machineMap
		{
			private get;
			set;
		}

		[Inject]
		internal IMothershipPropPresenter mothershipPropPresenter
		{
			private get;
			set;
		}

		public ThumbnailCreator()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			submissioncontroller.SetupThumbnailCreator(this);
			thumbnailManager.SetupThumbnailCreator(this);
		}

		private static float GetLinearValue(float value, float inStart, float inStop, float outStart, float outStop)
		{
			if (value <= inStart)
			{
				return outStart;
			}
			if (inStop <= value)
			{
				return outStop;
			}
			return outStart + (value - inStart) / (inStop - inStart) * (outStop - outStart);
		}

		public IEnumerator RenderGarageThumbnail(bool highQuality, Action<Texture2D> onSuccess)
		{
			IEnumerator en = machineMap.GetMachineBounds(null);
			yield return en;
			Texture2D tex = SetupPropsAndRenderThumbnail((Bounds)en.Current, highQuality);
			SafeEvent.SafeRaise<Texture2D>(onSuccess, tex);
		}

		private Texture2D SetupPropsAndRenderThumbnail(Bounds bounds, bool highQuality)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			if (_machine == null)
			{
				_machine = MachineBoard.Instance.board;
			}
			_minMachinePos = bounds.get_min();
			_maxMachinePos = bounds.get_max();
			MothershipPropState prevState = mothershipPropPresenter.PushThumbnailRenderState();
			Texture2D result = RenderThumbnail(Positioning.AutoTargetRobot, highQuality);
			mothershipPropPresenter.PopThumbnailRenderState(prevState);
			return result;
		}

		public Texture2D RenderThumbnail(Positioning positioning, bool highQuality, Texture2D thumbnail = null)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Expected O, but got Unknown
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Expected O, but got Unknown
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			Resources.UnloadUnusedAssets();
			ThumbnailCamera.get_gameObject().SetActive(true);
			switch (positioning)
			{
			case Positioning.CopyGameCameraTransformation:
				ThumbnailCamera.get_transform().set_position(Camera.get_main().get_transform().get_position());
				ThumbnailCamera.get_transform().set_rotation(Camera.get_main().get_transform().get_rotation());
				break;
			case Positioning.AutoTargetRobot:
			{
				Vector3 minMachinePos = _minMachinePos;
				Vector3 maxMachinePos = _maxMachinePos;
				float x = Mathf.Clamp(minMachinePos.x, 0f, minMachinePos.x);
				float z = Mathf.Clamp(minMachinePos.z, 0f, minMachinePos.z);
				float x2 = Mathf.Clamp(maxMachinePos.x, maxMachinePos.x, 11f);
				float z2 = Mathf.Clamp(maxMachinePos.z, maxMachinePos.z, 11f);
				minMachinePos.x = x;
				minMachinePos.z = z;
				maxMachinePos.x = x2;
				maxMachinePos.z = z2;
				float linearValue = GetLinearValue(maxMachinePos.z - minMachinePos.z, 1f, 4f, 0f, 1f);
				float linearValue2 = GetLinearValue(maxMachinePos.y - minMachinePos.y, 1f, 4f, 0f, 1f);
				minMachinePos.y -= linearValue2;
				minMachinePos.z += linearValue;
				maxMachinePos.z -= linearValue;
				Vector3 val = maxMachinePos - minMachinePos;
				val.Normalize();
				Vector3 position = maxMachinePos + val * 1f;
				Vector3 val2 = -val;
				Vector3 val3 = Vector3.Cross(Vector3.get_up(), val2);
				Vector3 val4 = Vector3.Cross(val2, val3);
				Quaternion rotation = Quaternion.LookRotation(val2, val4);
				ThumbnailCamera.get_transform().set_position(position);
				ThumbnailCamera.get_transform().set_rotation(rotation);
				break;
			}
			}
			if (thumbnail == null)
			{
				thumbnail = GameUtility.CreateRobotShopTexture();
			}
			int num = (!highQuality) ? 1 : 4;
			int num2 = num * thumbnail.get_width();
			int num3 = num * thumbnail.get_height();
			if (_renderTexture == null || _renderTexture.get_width() != num2 || _renderTexture.get_height() != num3)
			{
				_renderTexture = new RenderTexture(num2, num3, 24);
			}
			ThumbnailCamera.set_targetTexture(_renderTexture);
			RenderTexture.set_active(ThumbnailCamera.get_targetTexture());
			ThumbnailCamera.Render();
			if (highQuality)
			{
				if (_ScaledUpThumbnail == null)
				{
					_ScaledUpThumbnail = new Texture2D(thumbnail.get_width() * 4, thumbnail.get_height() * 4, thumbnail.get_format(), false);
				}
				_ScaledUpThumbnail.ReadPixels(new Rect(0f, 0f, (float)_ScaledUpThumbnail.get_width(), (float)_ScaledUpThumbnail.get_height()), 0, 0);
				_ScaledUpThumbnail.Apply(false);
				TextureScaler.ResizeTexture(thumbnail, _ScaledUpThumbnail);
			}
			else
			{
				thumbnail.ReadPixels(new Rect(0f, 0f, (float)thumbnail.get_width(), (float)thumbnail.get_height()), 0, 0);
				thumbnail.Apply(false);
			}
			ThumbnailCamera.set_targetTexture(null);
			RenderTexture.set_active(null);
			ThumbnailCamera.get_gameObject().SetActive(false);
			return thumbnail;
		}

		private void TranslateLastMachineDimension(int y)
		{
			ref Vector3 minMachinePos = ref _minMachinePos;
			minMachinePos.y += (float)y;
			ref Vector3 maxMachinePos = ref _maxMachinePos;
			maxMachinePos.y += (float)y;
		}
	}
}
