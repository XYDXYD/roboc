using UnityEngine;

namespace Mothership.RobotConfiguration
{
	internal class RobotConfigListDisplayImplementor : MonoBehaviour, IRobotConfigListDisplayComponent
	{
		[SerializeField]
		private GameObject templateBayListItem;

		[SerializeField]
		private GameObject templateSpawnEffectItem;

		[SerializeField]
		private GameObject templateDeatheffectItem;

		[SerializeField]
		private UIGrid templateGridSpecificationForBaySkins;

		[SerializeField]
		private UIGrid templateGridSpecificationForSpawnEffects;

		[SerializeField]
		private UIGrid templateGridSpecificationForDeathEffects;

		[SerializeField]
		private Transform templateListGridTransform;

		private UIGrid _grid;

		GameObject IRobotConfigListDisplayComponent.mothershipBaylistItemTemplateGO
		{
			get
			{
				return templateBayListItem;
			}
		}

		GameObject IRobotConfigListDisplayComponent.spawnEffectslistItemTemplateGO
		{
			get
			{
				return templateSpawnEffectItem;
			}
		}

		GameObject IRobotConfigListDisplayComponent.deathEffectslistItemTemplateGO
		{
			get
			{
				return templateDeatheffectItem;
			}
		}

		Transform IRobotConfigListDisplayComponent.listParentTransform
		{
			get
			{
				return templateListGridTransform;
			}
		}

		ListGroupSelection IRobotConfigListDisplayComponent.listDisplayMode
		{
			set
			{
				switch (value)
				{
				case ListGroupSelection.MothershipBaySkin:
					CopyGridParametersFrom(templateGridSpecificationForBaySkins);
					break;
				case ListGroupSelection.SpawnEffects:
					CopyGridParametersFrom(templateGridSpecificationForSpawnEffects);
					break;
				case ListGroupSelection.DeathEffects:
					CopyGridParametersFrom(templateGridSpecificationForDeathEffects);
					break;
				}
			}
		}

		public RobotConfigListDisplayImplementor()
			: this()
		{
		}

		private void CopyGridParametersFrom(UIGrid other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			_grid.arrangement = other.arrangement;
			_grid.cellWidth = other.cellWidth;
			_grid.cellHeight = other.cellHeight;
			_grid.maxPerLine = other.maxPerLine;
			_grid.sorting = other.sorting;
			_grid.pivot = 0;
			_grid.set_repositionNow(true);
		}

		private void Awake()
		{
			_grid = this.GetComponent<UIGrid>();
			_grid.get_gameObject().SetActive(true);
		}
	}
}
