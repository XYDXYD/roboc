namespace Network
{
	public enum NetworkError
	{
		Ok = 0,
		VersionMismatch = 9,
		CRCMismatch = 10,
		DNSFailure = 11,
		WrongHost = 1,
		Timeout = 6,
		WrongConnection = 2,
		WrongChannel = 3,
		MessageToLong = 7,
		NoResources = 4,
		BadMessage = 5,
		WrongOperation = 8,
		ConnectionFailed = 9,
		AuthChallengeFailed = 10,
		DisconnectedBySendError = 11,
		DisconnectedByReceieveError = 12
	}
}
