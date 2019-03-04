using Simulation.Hardware.Weapons;
using UnityEngine;

internal sealed class CubeInstance : MonoBehaviour
{
	private const float MASS_MULTIPLIER = 10f;

	public GameObject editorCube;

	public GameObject simulationCube;

	[Tooltip("Actual mass of the cube")]
	public float mass = 5f;

	[Tooltip("Scale for the mass of the cube used to calculate the Centre of Mass")]
	public float physicsMassScalar = 1f;

	public float drag;

	public float angularDrag = 0.04f;

	public bool customCOM;

	public Vector3 comOffset = Vector3.get_zero();

	public bool displayCenterOfMass;

	public float DisplayedMass => mass * 10f;

	public CubeInstance()
		: this()
	{
	}//IL_0022: Unknown result type (might be due to invalid IL or missing references)
	//IL_0027: Unknown result type (might be due to invalid IL or missing references)


	public void Init(CubeInstance instance)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		mass = instance.mass;
		physicsMassScalar = instance.physicsMassScalar;
		drag = instance.drag;
		angularDrag = instance.angularDrag;
		customCOM = instance.customCOM;
		comOffset = instance.comOffset;
	}

	private void OnDrawGizmos()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (displayCenterOfMass)
		{
			Transform transform = this.get_transform();
			if (transform != null && !Application.get_isPlaying())
			{
				Vector3 val = transform.get_position() + GridScaleUtility.WorldScale(transform.get_rotation() * comOffset, TargetType.Player);
				Gizmos.set_color(Color.get_red());
				Gizmos.DrawSphere(val, 0.06f);
			}
		}
	}
}
