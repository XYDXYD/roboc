using Svelto.IoC;
using UnityEngine;

internal sealed class TeamBaseAnimation : MonoBehaviour
{
	public MeshRenderer meshRenderer;

	public int maxSections = 4;

	public float barInterpolateSpeed = 5f;

	private string _textureName = "_MainTex";

	private int _teamId;

	private float _lastOffset;

	[Inject]
	internal TeamBaseProgressDispatcher teamBaseProgressDispatcher
	{
		private get;
		set;
	}

	public TeamBaseAnimation()
		: this()
	{
	}

	public void SetTeamId(int teamId)
	{
		_teamId = teamId;
		if (teamBaseProgressDispatcher != null)
		{
			teamBaseProgressDispatcher.RegisterBaseChange(_teamId, OnProgressChanged);
		}
	}

	private void Start()
	{
		OnProgressChanged(0f);
	}

	private void OnProgressChanged(float newProgress)
	{
		_lastOffset = 0f - (1f - newProgress / (float)maxSections);
	}

	private void Update()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Vector2 textureOffset = meshRenderer.get_material().GetTextureOffset(_textureName);
		textureOffset.x += (_lastOffset - textureOffset.x) * barInterpolateSpeed * Time.get_deltaTime();
		meshRenderer.get_material().SetTextureOffset(_textureName, textureOffset);
	}
}
