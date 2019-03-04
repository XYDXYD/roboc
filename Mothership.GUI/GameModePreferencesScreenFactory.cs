using Svelto.ECS;
using UnityEngine;

namespace Mothership.GUI
{
	internal class GameModePreferencesScreenFactory
	{
		public static void Build(IEntityFactory entityFactory, GameObject go)
		{
			GameModePreferencesScreen componentInChildren = go.GetComponentInChildren<GameModePreferencesScreen>();
			int instanceID = go.GetInstanceID();
			componentInChildren.Initialize(instanceID);
			go.SetActive(false);
			entityFactory.BuildEntity<GameModePreferencesScreenEntityDescriptor>(instanceID, (object[])new GameModePreferencesScreen[1]
			{
				componentInChildren
			});
		}
	}
}
