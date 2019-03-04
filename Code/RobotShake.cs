using Simulation.Hardware.Weapons;
using UnityEngine;

public class RobotShake : MonoBehaviour, IRobotShakeDamageComponent
{
	[Header("This value is cached. Do not change it runtime!")]
	[SerializeField]
	private int maxSimultaneouslyShake = 3;

	[Space(10f)]
	[SerializeField]
	private float damageMagnitudeMultiplier = 10f;

	[SerializeField]
	private float damageDuration = 0.2f;

	[SerializeField]
	private TranslationCurve damageCurves;

	[SerializeField]
	private float minimumMagnitude = 2f;

	int IRobotShakeDamageComponent.maxSimultaneouslyShake
	{
		get
		{
			return maxSimultaneouslyShake;
		}
	}

	float IRobotShakeDamageComponent.damageMagnitudeMultiplier
	{
		get
		{
			return damageMagnitudeMultiplier;
		}
	}

	float IRobotShakeDamageComponent.damageDuration
	{
		get
		{
			return damageDuration;
		}
	}

	TranslationCurve IRobotShakeDamageComponent.damageCurves
	{
		get
		{
			return damageCurves;
		}
	}

	float IRobotShakeDamageComponent.minimumMagnitude
	{
		get
		{
			return minimumMagnitude;
		}
	}

	public RobotShake()
		: this()
	{
	}

	private void OnValidate()
	{
		damageCurves.ValidateInInspector();
	}
}
