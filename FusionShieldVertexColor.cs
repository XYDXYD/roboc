using UnityEngine;

public class FusionShieldVertexColor : MonoBehaviour
{
	public MeshFilter shield;

	private Vector3[] vertices;

	private Color[] colors;

	private float onFade;

	private float offFade;

	[Range(0f, 0.1f)]
	public float highlightThickness = 0.1f;

	private float oldOnFade = 1f;

	private float oldOffFade = 1f;

	private bool isUpdatingVertices;

	public float OnFade
	{
		get
		{
			return onFade;
		}
		set
		{
			onFade = value;
		}
	}

	public float OffFade
	{
		get
		{
			return offFade;
		}
		set
		{
			offFade = value;
		}
	}

	public FusionShieldVertexColor()
		: this()
	{
	}

	private void Start()
	{
		vertices = shield.get_mesh().get_vertices();
		colors = (Color[])new Color[vertices.Length];
		oldOnFade = onFade;
		oldOffFade = offFade;
	}

	private void Update()
	{
		if (oldOnFade != onFade && !isUpdatingVertices)
		{
			isUpdatingVertices = true;
			oldOnFade = onFade;
			UpdateVertices(isPowered: true);
		}
		if (oldOffFade != offFade && !isUpdatingVertices)
		{
			isUpdatingVertices = true;
			oldOffFade = offFade;
			UpdateVertices(isPowered: false);
		}
	}

	private void UpdateVertices(bool isPowered)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < vertices.Length; i++)
		{
			if (isPowered)
			{
				if (vertices[i].y <= onFade - highlightThickness)
				{
					colors[i] = Color.get_clear();
				}
				else if (vertices[i].y >= onFade)
				{
					colors[i] = Color.get_red();
				}
				else
				{
					colors[i] = Color.get_red();
				}
			}
			else if (vertices[i].y <= offFade - highlightThickness)
			{
				colors[i] = Color.get_red();
			}
			else if (vertices[i].y >= offFade)
			{
				colors[i] = Color.get_clear();
			}
			else
			{
				colors[i] = Color.get_red();
			}
		}
		shield.get_mesh().set_colors(colors);
		isUpdatingVertices = false;
	}
}
