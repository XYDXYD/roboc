using Svelto.ES.Legacy;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class PingSelectorBehaviour : MonoBehaviour, IPingTypeComponent, IComponent
	{
		[SerializeField]
		private GameObject[] pingButtons;

		[SerializeField]
		private UISprite circularProgressBar;

		[SerializeField]
		private UISprite centerLineRenderer;

		public UICamera uiCamera;

		public float currentScaleFactor;

		private List<GameObject> _childElements;

		private bool _enabled;

		public event Action<PingType> OnPingTypeSelected = delegate
		{
		};

		public PingSelectorBehaviour()
			: this()
		{
		}

		private void Start()
		{
			_childElements = new List<GameObject>();
			int childCount = this.get_transform().get_childCount();
			for (int i = 0; i < childCount; i++)
			{
				_childElements.Add(this.get_transform().GetChild(i).get_gameObject());
			}
		}

		public void Show(Vector3 position)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			this.get_transform().set_localPosition(new Vector3(0f - position.x, position.y, position.z));
			for (int i = 0; i < _childElements.Count; i++)
			{
				_childElements[i].SetActive(true);
			}
			_enabled = true;
		}

		public void Hide(Vector3 position)
		{
			for (int i = 0; i < _childElements.Count; i++)
			{
				_childElements[i].SetActive(false);
			}
			_enabled = false;
		}

		public void ChangeButtonsColorToGray(bool change)
		{
			for (int i = 0; i < pingButtons.Length; i++)
			{
				pingButtons[i].GetComponent<PingTypeButtonBehaviour>().ChangeButtonColorToGray(change);
			}
		}

		public void SetProgressBar(float progress)
		{
			circularProgressBar.set_fillAmount(progress);
		}

		public void DrawLine(float magnitude, float angle)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = centerLineRenderer.get_transform();
			Vector3 localScale = centerLineRenderer.get_transform().get_localScale();
			float x = localScale.x;
			float num = currentScaleFactor;
			Vector3 localScale2 = this.get_transform().get_parent().get_localScale();
			float num2 = magnitude / (num * localScale2.x);
			Vector3 localScale3 = centerLineRenderer.get_transform().get_localScale();
			transform.set_localScale(new Vector3(x, num2, localScale3.z));
			centerLineRenderer.get_transform().set_localEulerAngles(new Vector3(0f, 0f, angle));
		}

		public void OnPingTypeChanged(PingType type)
		{
			this.OnPingTypeSelected(type);
		}

		public void SelectPingType(bool select, PingType type)
		{
			if (_enabled)
			{
				pingButtons[(int)type].GetComponent<PingTypeButtonBehaviour>().SelectPingType(select);
			}
		}
	}
}
