using System.Collections;

namespace Login
{
	internal interface IPlatformUtilities
	{
		IEnumerator ReadyPlatform();

		bool VerifyPlatformReadyness();

		IEnumerator PreAuthenticate();

		IEnumerator RegisterNewUser(RegisterNewUserDependency registerUserDependency);

		IEnumerator AuthenticateExistingUser(string identifier, string password);

		IEnumerator AuthenticateExistingUser();

		IEnumerator CheckIdentifierAvailableDuringEntry(string identifier);

		IEnumerator CheckDisplayNameChangeFlag();

		IEnumerator ChangeDisplayName(string newName);
	}
}
