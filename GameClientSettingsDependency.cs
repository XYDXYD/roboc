using System;

internal class GameClientSettingsDependency
{
	public readonly DateTime tutorialSignupDateThreshold;

	public readonly float healthThreshold;

	public readonly float microbotSphereRadius;

	public readonly float smartRotationMisfireAngle;

	public readonly int fusionShieldDPS;

	public readonly uint fusionShieldHPS;

	public readonly uint requestReviewAtLevel;

	public readonly float criticalRatio;

	public GameClientSettingsDependency(float _healthThreshold, uint _fusionShieldHPS, int _fusionShieldDPS, float _microbotSphereRadius, float _smartRotationMisfireAngle, uint _requestReviewAtLevel, DateTime _tutorialSignupDateThreshold, float _criticalRatio)
	{
		healthThreshold = _healthThreshold;
		fusionShieldHPS = _fusionShieldHPS;
		fusionShieldDPS = _fusionShieldDPS;
		microbotSphereRadius = _microbotSphereRadius;
		smartRotationMisfireAngle = _smartRotationMisfireAngle;
		requestReviewAtLevel = _requestReviewAtLevel;
		tutorialSignupDateThreshold = _tutorialSignupDateThreshold;
		criticalRatio = _criticalRatio;
	}
}
