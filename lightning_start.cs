using UnityEngine;

public class lightning_start : MonoBehaviour
{
	public GameObject[] targets;

	public LineRenderer lineRend;

	public float maxZapTimer = 1f;

	public float minZapTimer = 2f;

	public float maxDelay = 0.5f;

	private float arcLength = 1f;

	private float arcVariation = 1f;

	private float inaccuracy = 0.5f;

	private float zapTimer;

	private float delay;

	private int index;

	private float b;

	private float c;

	public lightning_start()
		: this()
	{
	}

	private void OnEnable()
	{
		zapTimer = Random.Range(minZapTimer, maxZapTimer);
		delay = Random.Range(0f, maxDelay);
		b = zapTimer / (float)targets.Length;
		c = b;
		index = 0;
	}

	private void Start()
	{
		lineRend = this.get_gameObject().GetComponent<LineRenderer>();
		lineRend.SetVertexCount(1);
	}

	private void Update()
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		if (zapTimer > 0f)
		{
			if (!((delay -= Time.get_deltaTime()) > 0f))
			{
				c -= Time.get_deltaTime();
				if (c <= 0f)
				{
					index = Random.Range(0, targets.Length);
					c = b;
				}
				Vector3 val = this.get_transform().get_position();
				int num = 1;
				lineRend.SetPosition(0, this.get_transform().get_position());
				while (Vector3.Distance(targets[index].get_transform().get_position(), val) > 3f)
				{
					lineRend.SetVertexCount(num + 1);
					Vector3 val2 = targets[index].get_transform().get_position() - val;
					val2.Normalize();
					val2 = Randomize(val2, inaccuracy);
					val2 *= Random.Range(arcLength * arcVariation, arcLength);
					val2 += val;
					lineRend.SetPosition(num, val2);
					num++;
					val = val2;
				}
				lineRend.SetVertexCount(num + 1);
				lineRend.SetPosition(num, targets[index].get_transform().get_position());
				zapTimer -= Time.get_deltaTime();
			}
		}
		else
		{
			lineRend.SetVertexCount(1);
		}
	}

	private Vector3 Randomize(Vector3 newVector, float devation)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		newVector += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * devation;
		newVector.Normalize();
		return newVector;
	}
}
