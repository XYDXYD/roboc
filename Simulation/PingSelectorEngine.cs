using Fabric;
using Svelto.ES.Legacy;
using Svelto.Ticker.Legacy;
using System;
using UnityEngine;

namespace Simulation
{
	internal class PingSelectorEngine : IEngine, ITickable, ITickableBase
	{
		private IMiniMapViewComponent _minimapViewComponent;

		private IPingSelectorComponent _pingSelectorComponent;

		private IPingObjectsManagementComponent _pingManagementComponent;

		private bool _disabled;

		private bool _clickPressed;

		public Type[] AcceptedComponents()
		{
			return new Type[4]
			{
				typeof(IMiniMapViewComponent),
				typeof(IPingSelectorComponent),
				typeof(IMapInputComponent),
				typeof(IPingObjectsManagementComponent)
			};
		}

		public void Add(IComponent component)
		{
			if (component is IMapInputComponent)
			{
				(component as IMapInputComponent).OnInputData += HandleOnInputData;
			}
			else if (component is IMiniMapViewComponent)
			{
				_minimapViewComponent = (component as IMiniMapViewComponent);
			}
			else if (component is IPingSelectorComponent)
			{
				_pingSelectorComponent = (component as IPingSelectorComponent);
			}
			else if (component is IPingObjectsManagementComponent)
			{
				_pingManagementComponent = (component as IPingObjectsManagementComponent);
			}
		}

		public void Remove(IComponent component)
		{
			if (component is IMapInputComponent)
			{
				(component as IMapInputComponent).OnInputData -= HandleOnInputData;
			}
			else if (component is IMiniMapViewComponent)
			{
				_minimapViewComponent = null;
			}
			else if (component is IPingSelectorComponent)
			{
				_pingSelectorComponent = null;
			}
			else if (component is IPingObjectsManagementComponent)
			{
				_pingManagementComponent = null;
			}
		}

		public void Tick(float deltaSec)
		{
			float mapPingCurrentTimerValue = _pingManagementComponent.GetMapPingCurrentTimerValue();
			float mapPingCooldown = _pingManagementComponent.GetMapPingCooldown();
			if (mapPingCurrentTimerValue > 0f)
			{
				if (!_disabled)
				{
					SetPingSelectorButtonsEnabled(enabled: false);
					_disabled = true;
				}
				_pingSelectorComponent.SetProgressBarValue(1f - mapPingCurrentTimerValue / mapPingCooldown);
			}
			else if (_disabled)
			{
				SetPingSelectorButtonsEnabled(enabled: true);
				_pingSelectorComponent.SetProgressBarValue(0f);
				_disabled = false;
			}
		}

		private void HandleOnInputData(InputCharacterData input)
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			float num = input.data[16];
			float num2 = input.data[18];
			float num3 = input.data[17];
			if (num > 0f || num2 > 0f || num3 > 0f)
			{
				if (!_clickPressed)
				{
					_pingSelectorComponent.SetPingSelectorActive(active: true);
					EventManager.get_Instance().PostEvent("GUI_MapPing_Wheel_ON", 0);
					float scaledHalfMapSize = _minimapViewComponent.GetScaledHalfMapSize();
					float unscaledHalfMapSize = _minimapViewComponent.GetUnscaledHalfMapSize();
					Vector2 pixelOffset = _minimapViewComponent.GetPixelOffset();
					Vector3 mousePosition = Input.get_mousePosition();
					Vector3 val = default(Vector3);
					val._002Ector((float)Screen.get_width() - mousePosition.x + pixelOffset.x, mousePosition.y - pixelOffset.y, mousePosition.z);
					_pingSelectorComponent.SetPingSelectorPosition(new Vector3(0f - val.x / (scaledHalfMapSize * 2f) * unscaledHalfMapSize * 2f, val.y / (scaledHalfMapSize * 2f) * unscaledHalfMapSize * 2f, val.z));
					if (CheckIfOutsideMap(Input.get_mousePosition()))
					{
						_pingSelectorComponent.SetPingSelectorScale(2f);
					}
					else
					{
						_pingSelectorComponent.SetPingSelectorScale(1.5f);
					}
					_clickPressed = true;
				}
			}
			else if (_clickPressed)
			{
				_pingSelectorComponent.SetPingSelectorActive(active: false);
				EventManager.get_Instance().PostEvent("GUI_MapPing_Wheel_OFF", 0);
				_clickPressed = false;
			}
		}

		private bool CheckIfOutsideMap(Vector3 clickMousePosition)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			Vector2 pixelOffset = _minimapViewComponent.GetPixelOffset();
			float scaledHalfMapSize = _minimapViewComponent.GetScaledHalfMapSize();
			Vector3 val = default(Vector3);
			val._002Ector((float)Screen.get_width() - clickMousePosition.x + pixelOffset.x, clickMousePosition.y - pixelOffset.y, clickMousePosition.z);
			if (val.x > scaledHalfMapSize * 2f || val.y > scaledHalfMapSize * 2f)
			{
				return true;
			}
			return false;
		}

		private void SetPingSelectorButtonsEnabled(bool enabled)
		{
			_pingSelectorComponent.SetButtonsEnabledOfType(PingType.MOVE_HERE, enabled);
			_pingSelectorComponent.SetButtonsEnabledOfType(PingType.GOING_HERE, enabled);
			_pingSelectorComponent.SetButtonsEnabledOfType(PingType.DANGER, enabled);
		}
	}
}
