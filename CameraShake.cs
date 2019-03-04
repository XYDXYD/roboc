using UnityEngine;

internal sealed class CameraShake : MonoBehaviour
{
	[Header("This value is cached. Do not change it runtime!")]
	public int maxSimultaneouslyShake = 10;

	[Space(10f)]
	public float rotationDamageMagnitudeMultiplier = 10f;

	public float translationDamageMagnitudeMultiplier = 10f;

	public float damageDuration = 0.2f;

	public TranslationCurve translationDamageCurves;

	public RotationCurve rotationDamageCurves;

	public CameraShake()
		: this()
	{
	}

	private void OnValidate()
	{
		translationDamageCurves.ValidateInInspector();
		rotationDamageCurves.ValidateInInspector();
	}
}
