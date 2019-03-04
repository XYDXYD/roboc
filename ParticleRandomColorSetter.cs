using UnityEngine;

public class ParticleRandomColorSetter : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem[] systems;

	[SerializeField]
	private Gradient[] colorTables;

	private MainModule main;

	public ParticleRandomColorSetter()
		: this()
	{
	}

	private void Start()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < colorTables.Length; i++)
		{
			colorTables[i].set_mode(1);
		}
		for (int j = 0; j < systems.Length; j++)
		{
			main = systems[j].get_main();
			MinMaxGradient startColor = MinMaxGradient.op_Implicit(colorTables[Random.Range(0, colorTables.Length)]);
			startColor.set_mode(4);
			main.set_startColor(startColor);
		}
	}
}
