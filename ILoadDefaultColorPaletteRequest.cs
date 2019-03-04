using Svelto.ServiceLayer;

internal interface ILoadDefaultColorPaletteRequest : IServiceRequest, IAnswerOnComplete<ColorPaletteData>
{
}
