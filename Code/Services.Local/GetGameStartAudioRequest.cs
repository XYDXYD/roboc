using Simulation;
using Svelto.ServiceLayer;
using System;
using UnityEngine;

namespace Services.Local
{
	internal class GetGameStartAudioRequest : IGetGameStartAudioRequest, IServiceRequest, IAnswerOnComplete<string>
	{
		private IServiceAnswer<string> _answer;

		public void Execute()
		{
			MapBasedAudioEvents mapBasedAudioEvents = Object.FindObjectOfType<MapBasedAudioEvents>();
			if (mapBasedAudioEvents == null)
			{
				if (_answer != null && _answer.failed != null)
				{
					ServiceBehaviour serviceBehaviour = new ServiceBehaviour("strLocalError", "strLoadMapAudioFail");
					serviceBehaviour.exceptionThrown = new Exception("Failed to find a MapBasedAudioEvents component in the scene - such audio will be missing");
					_answer.failed(serviceBehaviour);
				}
			}
			else
			{
				_answer.succeed(mapBasedAudioEvents.GameStart);
			}
		}

		public IServiceRequest SetAnswer(IServiceAnswer<string> answer)
		{
			_answer = answer;
			return this;
		}
	}
}
