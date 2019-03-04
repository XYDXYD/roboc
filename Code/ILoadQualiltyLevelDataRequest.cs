using Svelto.ServiceLayer;

internal interface ILoadQualiltyLevelDataRequest : IServiceRequest, IAnswerOnComplete<QualityLevelDataAnswerData>
{
}
