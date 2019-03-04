using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Mothership.GUI
{
	internal sealed class StatsHintPopup : MonoBehaviour, IStatsHintComponent, IHintPopupComponent
	{
		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _description;

		[SerializeField]
		private UILabel _cpuAndRRLabel;

		[SerializeField]
		private UILabel _statValues;

		[SerializeField]
		private UIPanel _panel;

		[SerializeField]
		private UIPanel _arrowPanel;

		[SerializeField]
		private UISprite _border;

		[SerializeField]
		private Vector2 _panelOffsetBelow;

		[SerializeField]
		private Vector2 _panelOffsetAbove;

		[SerializeField]
		private float _hintDelayTime = 1f;

		[SerializeField]
		private float _showSpeed = 1f;

		[SerializeField]
		private float _hideSpeed = 2f;

		private bool _fadeIn;

		private float _displayStartTime;

		private float _hideStartTime;

		private bool _displayPopup;

		private float _hidePanelStartAlpha = 1f;

		public string title
		{
			set
			{
				_title.set_text(value);
			}
		}

		public string description
		{
			set
			{
				_description.set_text(value);
			}
		}

		public IList<ItemStat> statLines
		{
			set
			{
				SetStatLines(value);
			}
		}

		public IList<ItemStat> cpuAndRRLines
		{
			set
			{
				SetCPUAndRRLines(value);
			}
		}

		public bool fadeIn
		{
			set
			{
				_fadeIn = value;
			}
		}

		public Vector3 screenPosition
		{
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				PositionPopup(Vector2.op_Implicit(value));
			}
		}

		public StatsHintPopup()
			: this()
		{
		}

		private void Awake()
		{
		}

		private void Start()
		{
			_panel.get_gameObject().SetActive(false);
		}

		private void SetCPUAndRRLines(IList<ItemStat> lines)
		{
			_cpuAndRRLabel.set_text(GenerateDisplayStrings(lines));
		}

		private void SetStatLines(IList<ItemStat> lines)
		{
			_statValues.set_text(GenerateDisplayStrings(lines));
		}

		private string GenerateDisplayStrings(IList<ItemStat> lines)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < lines.Count; i++)
			{
				ItemStat itemStat = lines[i];
				string name = itemStat.name;
				ItemStat itemStat2 = lines[i];
				string value = itemStat2.value;
				StringBuilder values = stringBuilder;
				ItemStat itemStat3 = lines[i];
				AddStat(name, value, values, itemStat3.suffix);
			}
			return stringBuilder.ToString().TrimEnd('\n');
		}

		private void AddStat(string title, string value, StringBuilder values, string suffix = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(title);
			string str = $"{value} {suffix}";
			if (suffix == null)
			{
				stringBuilder.Append(" - " + value);
			}
			else
			{
				stringBuilder.Append(" - " + str);
			}
			values.AppendLine(stringBuilder.ToString());
		}

		private void PositionPopup(Vector2 screenPos)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector2.op_Implicit(NGUIMath.ScreenToParentPixels(screenPos, _arrowPanel.get_transform()));
			_arrowPanel.get_transform().set_localPosition(val);
			if (screenPos.x <= (float)(Screen.get_width() / 2) && screenPos.y <= (float)(Screen.get_height() / 2))
			{
				_arrowPanel.get_transform().set_localEulerAngles(new Vector3(0f, 0f, 180f));
				_panel.get_transform().set_localPosition(val + new Vector3(_panelOffsetAbove.x, (float)_border.get_height() + _panelOffsetAbove.y, 0f));
			}
			if (screenPos.x > (float)(Screen.get_width() / 2) && screenPos.y <= (float)(Screen.get_height() / 2))
			{
				_arrowPanel.get_transform().set_localEulerAngles(new Vector3(0f, 0f, 180f));
				_panel.get_transform().set_localPosition(val + new Vector3(0f - _panelOffsetAbove.x, (float)_border.get_height() + _panelOffsetAbove.y, 0f));
			}
			if (screenPos.x <= (float)(Screen.get_width() / 2) && screenPos.y > (float)(Screen.get_height() / 2))
			{
				_arrowPanel.get_transform().set_localEulerAngles(new Vector3(0f, 0f, 0f));
				_panel.get_transform().set_localPosition(val + new Vector3(_panelOffsetBelow.x, 0f - _panelOffsetBelow.y, 0f));
			}
			if (screenPos.x > (float)(Screen.get_width() / 2) && screenPos.y > (float)(Screen.get_height() / 2))
			{
				_arrowPanel.get_transform().set_localEulerAngles(new Vector3(0f, 0f, 0f));
				_panel.get_transform().set_localPosition(val + new Vector3(0f - _panelOffsetBelow.x, 0f - _panelOffsetBelow.y, 0f));
			}
		}

		private void OnEnable()
		{
			_panel.set_alpha(0f);
			_arrowPanel.set_alpha(0f);
			_panel.get_gameObject().SetActive(false);
			_arrowPanel.get_gameObject().SetActive(false);
		}

		private void DisplayHint(bool display)
		{
			if (!_displayPopup && display)
			{
				_displayPopup = true;
				_panel.get_gameObject().SetActive(true);
				_arrowPanel.get_gameObject().SetActive(true);
				_panel.set_alpha(0f);
				_arrowPanel.set_alpha(0f);
				_displayStartTime = Time.get_time();
			}
			if (_displayPopup && !display)
			{
				_displayPopup = false;
				_hidePanelStartAlpha = _panel.get_alpha();
				_hideStartTime = Time.get_time();
			}
			if (display)
			{
				if (Time.get_time() - _displayStartTime > _hintDelayTime)
				{
					float num = (Time.get_time() - _hintDelayTime - _displayStartTime) * _showSpeed;
					float alpha = Mathf.Lerp(0f, 1f, num);
					_panel.set_alpha(alpha);
					_arrowPanel.set_alpha(alpha);
				}
				return;
			}
			float num2 = (Time.get_time() - _hideStartTime) * _hideSpeed;
			float alpha2 = Mathf.Lerp(_hidePanelStartAlpha, 0f, num2);
			_panel.set_alpha(alpha2);
			_arrowPanel.set_alpha(alpha2);
			if (_panel.get_alpha() <= 0f)
			{
				_panel.get_gameObject().SetActive(false);
				_arrowPanel.get_gameObject().SetActive(false);
			}
		}

		private void Update()
		{
			DisplayHint(_fadeIn);
		}
	}
}
