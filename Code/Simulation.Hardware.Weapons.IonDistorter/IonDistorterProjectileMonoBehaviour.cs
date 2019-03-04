using Svelto.DataStructures;
using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Weapons.IonDistorter
{
	internal sealed class IonDistorterProjectileMonoBehaviour : BaseProjectileMonoBehaviour, IIonDistorterProjectileConeComponent, IIonDistorterCollisonComponent
	{
		private FasterList<Vector3> _projectileDirections = new FasterList<Vector3>(20);

		private Dispatcher<IIonDistorterCollisonComponent, int> _checkCollision;

		private float _coneAngle;

		private int _numOfRaycasts;

		private int _numOfCircles;

		private ParticleSystem[] _bullets;

		float IIonDistorterProjectileConeComponent.coneAngle
		{
			get
			{
				return _coneAngle;
			}
			set
			{
				_coneAngle = value;
			}
		}

		int IIonDistorterProjectileConeComponent.numOfRaycasts
		{
			get
			{
				return _numOfRaycasts;
			}
			set
			{
				_numOfRaycasts = value;
			}
		}

		int IIonDistorterProjectileConeComponent.numOfCircles
		{
			get
			{
				return _numOfCircles;
			}
			set
			{
				_numOfCircles = value;
			}
		}

		ParticleSystem[] IIonDistorterProjectileConeComponent.bullets
		{
			get
			{
				return _bullets;
			}
		}

		Dispatcher<IIonDistorterCollisonComponent, int> IIonDistorterCollisonComponent.checkCollision
		{
			get
			{
				return _checkCollision;
			}
		}

		FasterList<Vector3> IIonDistorterCollisonComponent.projectileDirections
		{
			get
			{
				return _projectileDirections;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			_checkCollision = new Dispatcher<IIonDistorterCollisonComponent, int>(this);
			int childCount = this.get_transform().get_childCount();
			_bullets = (ParticleSystem[])new ParticleSystem[childCount];
			for (int num = childCount - 1; num >= 0; num--)
			{
				_bullets[num] = this.get_transform().GetChild(num).GetComponent<ParticleSystem>();
			}
		}

		internal override void SetGenericProjectileParamaters(WeaponShootingNode weapon, Vector3 launchPosition, Vector3 direction, Vector3 robotStartPos, bool isLocal)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			IonDistorterShootingNode ionDistorterShootingNode = weapon as IonDistorterShootingNode;
			_coneAngle = ionDistorterShootingNode.accuracyStats.baseInAccuracyDegrees;
			_numOfRaycasts = ionDistorterShootingNode.projectileSettingsComponent.numOfRaycasts;
			_numOfCircles = ionDistorterShootingNode.projectileSettingsComponent.numOfCircles;
			base.SetGenericProjectileParamaters(weapon, launchPosition, direction, robotStartPos, isLocal);
		}
	}
}
