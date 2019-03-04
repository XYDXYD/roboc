using Simulation;
using System;
using UnityEngine;

internal interface IMinimapPresenter
{
	event Action<float> OnMinimapZoom;

	void RegisterView(MinimapView view);

	void OnDestroy();

	void Start();

	void MinimapResized(float pixelOffset);

	void SetWorldBounds(Vector3 bottomRight, Vector3 topLeft);

	void SetMinimapTexture(Texture minimapTexture);

	void SetCapturePointOwner(bool isMyTeam, int towerIndex);

	bool GetVisible(int playerId);

	void RegisterPlayerCamera(SimulationCamera camera);

	void RegisterCapturePointPosition(Vector3 basePosition, int index);

	void RegisterBasePosition(Vector3 basePosition, bool isFriendly);

	void RegisterEqualizerPosition(Vector3 position);

	void SetEqualizerOwner(bool isMyTeam);

	void HideEqualizer();
}
