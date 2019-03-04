using Svelto.IoC;
using UnityEngine;

internal class MinimapLevelData : MonoBehaviour, IInitialize
{
	public Transform bottomRight;

	public Transform topLeft;

	public Texture minimapTexture;

	[Inject]
	internal IMinimapPresenter presenter
	{
		private get;
		set;
	}

	[Inject]
	internal MapDataObserver mapDataObserver
	{
		get;
		set;
	}

	public MinimapLevelData()
		: this()
	{
	}

	void IInitialize.OnDependenciesInjected()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		presenter.SetWorldBounds(bottomRight.get_position(), topLeft.get_position());
		presenter.SetMinimapTexture(minimapTexture);
		mapDataObserver.InitializeData(bottomRight, topLeft);
	}
}
