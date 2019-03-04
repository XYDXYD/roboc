using Svelto.Factories;
using Svelto.IoC;
using System.Collections.Generic;
using Taunts;
using UnityEngine;

namespace Mothership
{
	public class TauntsMothershipView : MonoBehaviour, IInitialize
	{
		private class IdleEffect
		{
			public Vector3 LocationOfAnchor;

			public GameObject IdleGameObject;

			public IdleEffect(Vector3 locationOfAnchor_, GameObject idleGameObject_)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				LocationOfAnchor = locationOfAnchor_;
				IdleGameObject = idleGameObject_;
			}
		}

		private List<IdleEffect> _activeEffects = new List<IdleEffect>();

		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		internal TauntsMothershipController controller
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			get;
			private set;
		}

		[Inject]
		internal ITauntMaskHelper tauntsMaskHelper
		{
			private get;
			set;
		}

		public TauntsMothershipView()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			controller.SetView(this);
		}

		public void MaskIdleAnimationsMoved(Int3 displacement)
		{
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = default(Vector3);
			val._002Ector((float)displacement.x / 5f, (float)displacement.y / 5f, (float)displacement.z / 5f);
			foreach (IdleEffect activeEffect in _activeEffects)
			{
				activeEffect.LocationOfAnchor = new Vector3(activeEffect.LocationOfAnchor.x + (float)displacement.x, activeEffect.LocationOfAnchor.y + (float)displacement.y, activeEffect.LocationOfAnchor.z + (float)displacement.z);
				Transform transform = activeEffect.IdleGameObject.get_transform();
				transform.set_localPosition(transform.get_localPosition() + val);
			}
		}

		public void ShowTauntIdleAnimation(string maskGroupName, Vector3 maskAnchorLocation, Quaternion maskOrientationForMachineSpace, GameObject machineBoardRoot)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			foreach (IdleEffect activeEffect in _activeEffects)
			{
				if (((object)activeEffect.LocationOfAnchor).Equals((object)maskAnchorLocation))
				{
					return;
				}
			}
			string idleAnimationToPlayForGroup = tauntsMaskHelper.GetIdleAnimationToPlayForGroup(maskGroupName);
			GameObject val = gameObjectFactory.Build(idleAnimationToPlayForGroup);
			val.get_transform().set_position(Vector3.get_zero());
			val.get_transform().set_localPosition(Vector3.get_zero());
			Vector3 val2 = tauntsMaskHelper.CalculateRelativeMachineMaskOffset(maskGroupName, maskAnchorLocation, maskOrientationForMachineSpace);
			val.get_transform().set_parent(machineBoardRoot.get_transform());
			val.get_transform().set_localRotation(maskOrientationForMachineSpace);
			if (guiInputController.GetActiveScreen() == GuiScreens.RobotShop)
			{
				val.get_transform().set_position(val2);
			}
			else
			{
				val.get_transform().set_localPosition(val2);
			}
			_activeEffects.Add(new IdleEffect(maskAnchorLocation, val));
		}

		public void HideTauntIdleAnimation(Vector3 effectAnchorLocation)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			foreach (IdleEffect activeEffect in _activeEffects)
			{
				if (((object)activeEffect.LocationOfAnchor).Equals((object)effectAnchorLocation))
				{
					Object.Destroy(activeEffect.IdleGameObject);
					_activeEffects.Remove(activeEffect);
					break;
				}
			}
		}
	}
}
