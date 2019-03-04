using UnityEngine;

internal sealed class RegisterTeamBaseDependency
{
	public int team
	{
		get;
		private set;
	}

	public Vector3 position
	{
		get;
		private set;
	}

	public Quaternion rotation
	{
		get;
		private set;
	}

	public RegisterTeamBaseDependency(int _team, Vector3 _position, Quaternion _rotation)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		position = _position;
		rotation = _rotation;
		team = _team;
	}
}
