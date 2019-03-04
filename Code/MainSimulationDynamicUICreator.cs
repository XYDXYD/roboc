using UnityEngine;

public class MainSimulationDynamicUICreator : MonoBehaviour
{
	[SerializeField]
	private GameObject DynamicUIPit;

	[SerializeField]
	private GameObject DynamicUINormal;

	[SerializeField]
	private GameObject DynamicPrefabsPit;

	[SerializeField]
	private GameObject DynamicPrefabsNormal;

	[SerializeField]
	private GameObject SceneContents;

	public MainSimulationDynamicUICreator()
		: this()
	{
	}

	public void BuildAllDynamicPrefabs()
	{
		MainSimulationContextHolderLoader component = this.GetComponent<MainSimulationContextHolderLoader>();
		GameObject val;
		GameObject val2;
		if (WorldSwitching.GetGameModeType() == GameModeType.Pit)
		{
			val = Object.Instantiate<GameObject>(DynamicUIPit);
			val2 = Object.Instantiate<GameObject>(DynamicPrefabsPit);
		}
		else
		{
			val = Object.Instantiate<GameObject>(DynamicUINormal);
			val2 = Object.Instantiate<GameObject>(DynamicPrefabsNormal);
		}
		val.get_transform().set_parent(component.get_transform());
		val2.get_transform().set_parent(component.get_transform());
		SceneContents.SetActive(true);
	}
}
