using Svelto.ECS;
using Svelto.Factories;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership.RobotConfiguration
{
	internal class RobotConfigurationGUIFactory
	{
		[Inject]
		private IGameObjectFactory gameObjectFactory
		{
			get;
			set;
		}

		[Inject]
		private IEntityFactory entityFactory
		{
			get;
			set;
		}

		public void BuildSingleItem(RobotConfigurationDisplayEngine.ListItemDisplayData entityData, IRobotConfigListDisplayComponent listComponent, bool startShown, Action<int, string> selectedCallback)
		{
			GameObject val = null;
			GameObject templateGO = null;
			switch (entityData.itemCategory)
			{
			case ListGroupSelection.MothershipBaySkin:
				templateGO = listComponent.mothershipBaylistItemTemplateGO;
				break;
			case ListGroupSelection.SpawnEffects:
				templateGO = listComponent.spawnEffectslistItemTemplateGO;
				break;
			case ListGroupSelection.DeathEffects:
				templateGO = listComponent.deathEffectslistItemTemplateGO;
				break;
			}
			val = CreateGO(templateGO, listComponent.listParentTransform);
			val.set_name(entityData.identifier);
			RobotConfigListItemDisplayImplementor component = val.GetComponent<RobotConfigListItemDisplayImplementor>();
			entityFactory.BuildEntity<RobotConfigDisplayListItemEntityDescriptor>(val.GetInstanceID(), new object[1]
			{
				component
			});
			component.Initialise(entityData, startShown);
		}

		public void BuildList(List<RobotConfigurationDisplayEngine.ListItemDisplayData> allEntities, IRobotConfigListDisplayComponent listComponent, ListGroupSelection initialCategorySelection, Action<int, string> selectedCallback)
		{
			for (int i = 0; i < allEntities.Count; i++)
			{
				RobotConfigurationDisplayEngine.ListItemDisplayData listItemDisplayData = allEntities[i];
				bool startShown = initialCategorySelection == listItemDisplayData.itemCategory;
				BuildSingleItem(allEntities[i], listComponent, startShown, selectedCallback);
			}
		}

		public void Build(GameObject go)
		{
			RobotConfigurationDisplayImplementor componentInChildren = go.GetComponentInChildren<RobotConfigurationDisplayImplementor>();
			int instanceID = go.GetInstanceID();
			componentInChildren.Initialize(instanceID);
		}

		private GameObject CreateGO(GameObject templateGO, Transform parentT)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = gameObjectFactory.Build(templateGO);
			val.get_transform().set_parent(parentT);
			Transform transform = val.get_transform();
			transform.set_localPosition(Vector3.get_zero());
			transform.set_localRotation(Quaternion.get_identity());
			transform.set_localScale(Vector3.get_one());
			val.SetActive(true);
			return val;
		}
	}
}
