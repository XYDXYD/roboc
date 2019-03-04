namespace Login
{
	internal interface ILoginPlatformUtilities
	{
		void InitialiseWrapper();

		bool CheckUsernameAvailable(string username);
	}
}
