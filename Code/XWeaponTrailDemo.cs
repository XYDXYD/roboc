using UnityEngine;
using Xft;

public class XWeaponTrailDemo : MonoBehaviour
{
	public Animation SwordAnimation;

	public XWeaponTrail ProTrailDistort;

	public XWeaponTrail ProTrailShort;

	public XWeaponTrail ProTraillong;

	public XWeaponTrail SimpleTrail;

	public XWeaponTrailDemo()
		: this()
	{
	}

	public void Start()
	{
		ProTrailDistort.Init();
		ProTrailShort.Init();
		ProTraillong.Init();
		SimpleTrail.Init();
	}

	private void OnGUI()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		if (GUI.Button(new Rect(0f, 0f, 150f, 30f), "Activate Trail1"))
		{
			ProTrailDistort.Deactivate();
			ProTrailShort.Deactivate();
			ProTraillong.Deactivate();
			SwordAnimation.Play();
			SimpleTrail.Activate();
		}
		if (GUI.Button(new Rect(0f, 30f, 150f, 30f), "Stop Trail1"))
		{
			SimpleTrail.Deactivate();
		}
		if (GUI.Button(new Rect(0f, 60f, 150f, 30f), "Stop Trail1 Smoothly"))
		{
			SimpleTrail.StopSmoothly(0.3f);
		}
		if (GUI.Button(new Rect(0f, 120f, 150f, 30f), "Activate Trail2"))
		{
			SimpleTrail.Deactivate();
			SwordAnimation.Play();
			ProTrailDistort.Activate();
			ProTrailShort.Activate();
			ProTraillong.Activate();
		}
		if (GUI.Button(new Rect(0f, 150f, 150f, 30f), "Stop Trail2"))
		{
			ProTrailDistort.Deactivate();
			ProTrailShort.Deactivate();
			ProTraillong.Deactivate();
		}
		if (GUI.Button(new Rect(0f, 180f, 150f, 30f), "Stop Trail2 Smoothly"))
		{
			ProTrailDistort.StopSmoothly(0.3f);
			ProTrailShort.StopSmoothly(0.3f);
			ProTraillong.StopSmoothly(0.3f);
		}
	}
}
