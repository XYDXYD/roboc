using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

internal class ColorEffects : MonoBehaviour
{
	public float beamCollapseSpeed = 0.2f;

	public float beamDisplayTime = 0.25f;

	public LineRenderer lineRenderer;

	public Transform beamStartTransform;

	public Material paintCubeBeamMaterial;

	public Material paintAllBeamMaterial;

	public GameObject paintImpactPrefab;

	public GameObject paintFillImpactPrefab;

	public float smallParticleSystemDuration = 0.25f;

	public float largeParticleSystemDuration = 0.75f;

	private PaletteColor _beamColor;

	private ParticleSystem _paintFillParticleSystem;

	private GameObject _paintFillGameObject;

	private Vector3 _targetPos;

	[Inject]
	internal GameObjectPool gameObjectPool
	{
		private get;
		set;
	}

	public ColorEffects()
		: this()
	{
	}

	private void Start()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		_paintFillGameObject = Object.Instantiate<GameObject>(paintFillImpactPrefab);
		_paintFillGameObject.get_transform().set_position(Vector3.get_zero());
		_paintFillGameObject.get_transform().set_localScale(Vector3.get_one());
		_paintFillParticleSystem = _paintFillGameObject.GetComponent<ParticleSystem>();
		_paintFillGameObject.SetActive(false);
		gameObjectPool.Preallocate(paintImpactPrefab.get_name(), 10, (Func<GameObject>)(() => gameObjectPool.AddRecycleOnDisableForLoopParticles(paintImpactPrefab, smallParticleSystemDuration)));
	}

	public void PlayImpactEffect(Vector3 hitPoint)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = gameObjectPool.Use(paintImpactPrefab.get_name(), (Func<GameObject>)(() => gameObjectPool.AddRecycleOnDisableForLoopParticles(paintImpactPrefab, smallParticleSystemDuration)));
		val.SetActive(true);
		val.get_transform().set_position(hitPoint);
		ParticleSystem component = val.GetComponent<ParticleSystem>();
		if (component.get_startColor() != Color32.op_Implicit(_beamColor.diffuse))
		{
			component.set_startColor(Color32.op_Implicit(_beamColor.diffuse));
		}
	}

	public void StartBeam(Vector3 target)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		lineRenderer.get_materials()[0] = paintCubeBeamMaterial;
		lineRenderer.SetPosition(0, beamStartTransform.get_position());
		lineRenderer.SetPosition(1, target);
		_targetPos = target;
		TaskRunner.get_Instance().Run((Func<IEnumerator>)DisplayBeam);
	}

	public void StartFillBeam(Vector3 target)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		lineRenderer.get_materials()[0] = paintAllBeamMaterial;
		lineRenderer.SetPosition(0, beamStartTransform.get_position());
		lineRenderer.SetPosition(1, target);
		if (_paintFillParticleSystem.get_startColor() != Color32.op_Implicit(_beamColor.diffuse))
		{
			_paintFillParticleSystem.set_startColor(Color32.op_Implicit(_beamColor.diffuse));
		}
		_paintFillGameObject.SetActive(true);
		_paintFillParticleSystem.Play();
		_targetPos = target;
	}

	public void UpdateFillBeamTarget(Vector3 target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		_targetPos = target;
		lineRenderer.SetPosition(0, beamStartTransform.get_position());
		lineRenderer.SetPosition(1, target);
		_paintFillGameObject.get_transform().set_position(target);
	}

	public void StopBeam()
	{
		TaskRunner.get_Instance().Run((Func<IEnumerator>)CollapseBeam);
	}

	public void UpdateColor(PaletteColor newColor)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		_beamColor = newColor;
		lineRenderer.SetColors(Color32.op_Implicit(newColor.diffuse), Color32.op_Implicit(newColor.diffuse));
	}

	private IEnumerator DisplayBeam()
	{
		float timer = 0f;
		while (timer < beamDisplayTime)
		{
			timer += Time.get_deltaTime();
			yield return null;
		}
		StopBeam();
	}

	private IEnumerator CollapseBeam()
	{
		float timer = 0f;
		while (timer < beamCollapseSpeed)
		{
			timer += Time.get_deltaTime();
			float progress = timer / beamCollapseSpeed;
			Vector3 newPos = Vector3.Lerp(beamStartTransform.get_position(), _targetPos, progress);
			lineRenderer.SetPosition(0, newPos);
			yield return null;
		}
		_paintFillParticleSystem.Stop();
		_paintFillGameObject.SetActive(false);
	}
}
