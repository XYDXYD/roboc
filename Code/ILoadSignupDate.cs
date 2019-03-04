using Svelto.ServiceLayer;
using System;

internal interface ILoadSignupDate : IServiceRequest, IAnswerOnComplete<DateTime>
{
}
