using UnityEngine;

namespace Simulation
{
	internal interface ICloakMaterialsComponent
	{
		Material fadeToMaterial
		{
			get;
		}

		Material fadeToMaterialRemote
		{
			get;
		}

		Material fadeToMaterialRemoteLow
		{
			get;
		}

		Shader skinnedShader
		{
			get;
		}

		Shader nonSkinnedShader
		{
			get;
		}

		int lowQualityThreshold
		{
			get;
		}
	}
}
