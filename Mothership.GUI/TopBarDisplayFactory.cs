using Mothership.OpsRoom;
using Svelto.ECS;
using Svelto.Factories;
using Svelto.IoC;
using UnityEngine;

namespace Mothership.GUI
{
	internal sealed class TopBarDisplayFactory
	{
		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IEntityFactory entityFactory
		{
			private get;
			set;
		}

		public void Build(ITopBarDisplay topBarDisplay)
		{
			GameObject val = gameObjectFactory.Build("Hud_TopBarInventory");
			topBarDisplay.SetTopBarBuildMode(val.GetComponent<TopBarBuildMode>());
			GameObject val2 = gameObjectFactory.Build("HUD_MothershipTopBar_Infinity");
			topBarDisplay.SetTopBar(val2.GetComponent<TopBar>());
			topBarDisplay.AddSelfToScreensList();
			topBarDisplay.SetDisplayStyle(TopBarStyle.Default);
			int instanceID = val2.GetInstanceID();
			MonoBehaviour[] componentsInChildren = val2.GetComponentsInChildren<MonoBehaviour>(true);
			entityFactory.BuildEntity<TopBarEntityDescriptor>(instanceID, (object[])componentsInChildren);
		}
	}
}
