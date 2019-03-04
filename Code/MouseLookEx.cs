using Svelto.IoC;
using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Look")]
internal sealed class MouseLookEx : MonoBehaviour, ICameraPosition
{
	public Transform CameraLook;

	public float sensitivityX = 15f;

	public float sensitivityY = 15f;

	public float minimumY = -60f;

	public float maximumY = 60f;

	public float minMouseSpeedX = 5f;

	public float maxMouseSpeedX = 25f;

	public float minMouseSpeedY = 5f;

	public float maxMouseSpeedY = 25f;

	private CharacterInputWrapper characterInputWrapper;

	private CharacterMotorEx characterMotor;

	[Inject]
	internal MouseSettings mouseSettings
	{
		private get;
		set;
	}

	public MouseLookEx()
		: this()
	{
	}

	private void Awake()
	{
		characterInputWrapper = this.GetComponent<CharacterInputWrapper>();
		characterMotor = this.GetComponent<CharacterMotorEx>();
	}

	private void Start()
	{
		mouseSettings.OnChangeMouseSettings += HandleOnChangeMouseSettings;
		sensitivityX = mouseSettings.GetBuildSpeed() * (maxMouseSpeedX - minMouseSpeedX) + minMouseSpeedX;
		sensitivityY = mouseSettings.GetBuildSpeed() * (maxMouseSpeedY - minMouseSpeedY) + minMouseSpeedY;
		if (mouseSettings.IsInvertY())
		{
			sensitivityY = 0f - sensitivityY;
		}
	}

	private void HandleOnChangeMouseSettings(float build, float fight, bool invert)
	{
		sensitivityX = build * (maxMouseSpeedX - minMouseSpeedX) + minMouseSpeedX;
		sensitivityY = build * (maxMouseSpeedY - minMouseSpeedY) + minMouseSpeedY;
		if (invert)
		{
			sensitivityY = 0f - sensitivityY;
		}
	}

	private void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		Quaternion rotation = CameraLook.get_rotation();
		Vector3 eulerAngles = rotation.get_eulerAngles();
		for (int i = 0; i < 3; i++)
		{
			if (eulerAngles.get_Item(i) > 180f)
			{
				int num;
				eulerAngles.set_Item(num = i, eulerAngles.get_Item(num) - 360f);
			}
		}
		float num2 = characterInputWrapper.mouseX * sensitivityX;
		float num3 = characterInputWrapper.mouseY * sensitivityY;
		num3 = Mathf.Clamp(num3 - eulerAngles.x, minimumY, maximumY) + eulerAngles.x;
		CharacterMotorEx characterMotorEx = characterMotor;
		characterMotorEx.LookEulerian += new Vector3(0f - num3, num2, 0f);
		CameraLook.set_localRotation(GetCameraOrientation());
	}

	public Quaternion GetCameraOrientation()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return Quaternion.Euler(characterMotor.LookEulerian.x, 0f, 0f);
	}
}
