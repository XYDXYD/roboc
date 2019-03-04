namespace Svelto.ES.Legacy
{
	public interface IEnginesRoot
	{
		void AddEngine(IEngine engine);

		void RemoveEngine(IEngine engine);

		void AddComponent(IComponent component);

		void RemoveComponent(IComponent component);
	}
}
