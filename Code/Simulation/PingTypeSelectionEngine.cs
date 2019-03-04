using Fabric;
using Svelto.ES.Legacy;
using Svelto.Ticker.Legacy;
using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class PingTypeSelectionEngine : IEngine, ITickable, ITickableBase
	{
		private IMiniMapViewComponent _minimapViewComponent;

		private IPingSelectorComponent _pingSelectorComponent;

		private bool _clickPressed;

		private Vector3 _center;

		private Vector3 _clickMousePosition;

		public Type[] AcceptedComponents()
		{
			return new Type[3]
			{
				typeof(IMiniMapViewComponent),
				typeof(IPingSelectorComponent),
				typeof(IMapInputComponent)
			};
		}

		public void Add(IComponent component)
		{
			if (component is IMapInputComponent)
			{
				(component as IMapInputComponent).OnInputData += HandleInputData;
			}
			else if (component is IMiniMapViewComponent)
			{
				_minimapViewComponent = (component as IMiniMapViewComponent);
			}
			else if (component is IPingSelectorComponent)
			{
				_pingSelectorComponent = (component as IPingSelectorComponent);
			}
		}

		public void Remove(IComponent component)
		{
			if (component is IMapInputComponent)
			{
				(component as IMapInputComponent).OnInputData -= HandleInputData;
			}
			else if (component is IMiniMapViewComponent)
			{
				_minimapViewComponent = null;
			}
			else if (component is IPingSelectorComponent)
			{
				_pingSelectorComponent = null;
			}
		}

		public void Tick(float deltaSec)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			if (!_clickPressed)
			{
				return;
			}
			Vector2 pixelOffset = _minimapViewComponent.GetPixelOffset();
			float scaledHalfMapSize = _minimapViewComponent.GetScaledHalfMapSize();
			float unscaledHalfMapSize = _minimapViewComponent.GetUnscaledHalfMapSize();
			Vector3 mousePosition = Input.get_mousePosition();
			Vector3 val = default(Vector3);
			val._002Ector((float)Screen.get_width() - mousePosition.x + pixelOffset.x, mousePosition.y - pixelOffset.y, mousePosition.z);
			Vector3 val2 = val - _center;
			val2._002Ector(0f - val2.x, val2.y, val2.z);
			float num = Vector3.Angle(Vector3.get_up(), val2);
			float num2 = Vector3.Dot(Vector3.get_right(), val2);
			float num3 = 1f;
			if (num2 > 0f)
			{
				num = 0f - num;
			}
			num3 = ((!CheckIfOutsideMap(_clickMousePosition)) ? 1.5f : 2f);
			if (val2.get_magnitude() > 16f * (scaledHalfMapSize / unscaledHalfMapSize) * num3)
			{
				if (num > 60f)
				{
					SelectPingType(select: true, PingType.DANGER);
					SelectPingType(select: false, PingType.MOVE_HERE);
					SelectPingType(select: false, PingType.GOING_HERE);
				}
				else if (num < -60f)
				{
					SelectPingType(select: true, PingType.MOVE_HERE);
					SelectPingType(select: false, PingType.GOING_HERE);
					SelectPingType(select: false, PingType.DANGER);
				}
				else if (num < 60f && num > -60f)
				{
					SelectPingType(select: true, PingType.GOING_HERE);
					SelectPingType(select: false, PingType.MOVE_HERE);
					SelectPingType(select: false, PingType.DANGER);
				}
			}
			else
			{
				SelectPingType(select: false, PingType.MOVE_HERE);
				SelectPingType(select: false, PingType.GOING_HERE);
				SelectPingType(select: false, PingType.DANGER);
				_pingSelectorComponent.SetSelectedPingType(PingType.UNKNOWN);
			}
		}

		private void HandleInputData(InputCharacterData input)
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			float num = input.data[16];
			float num2 = input.data[17];
			float num3 = input.data[18];
			if (!_minimapViewComponent.GetIsPingContextActive())
			{
				return;
			}
			if (num > 0f || num2 > 0f || num3 > 0f)
			{
				if (!_clickPressed)
				{
					_clickMousePosition = Input.get_mousePosition();
					Vector2 pixelOffset = _minimapViewComponent.GetPixelOffset();
					_center = new Vector3((float)Screen.get_width() - _clickMousePosition.x + pixelOffset.x, _clickMousePosition.y - pixelOffset.y, _clickMousePosition.z);
					_clickPressed = true;
				}
			}
			else
			{
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

		private void SelectPingType(bool select, PingType type)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Invalid comparison between Unknown and I4
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			if (!_pingSelectorComponent.GetButtonsEnabledOfType(type))
			{
				return;
			}
			if (select)
			{
				if ((int)_pingSelectorComponent.GetStateButtonsOfType(type) != 1)
				{
					_pingSelectorComponent.SetStateButtonsOfType(type, 1);
					_pingSelectorComponent.SetHoverButtonScalerOfType(type, hover: true);
					_pingSelectorComponent.SetSelectedPingType(type);
					EventManager.get_Instance().PostEvent("GUI_MapPing_Select", 0);
				}
			}
			else if ((int)_pingSelectorComponent.GetStateButtonsOfType(type) != 0)
			{
				_pingSelectorComponent.SetStateButtonsOfType(type, 0);
				_pingSelectorComponent.SetHoverButtonScalerOfType(type, hover: false);
			}
		}
	}
}
