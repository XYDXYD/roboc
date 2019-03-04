namespace Svelto.ES.Legacy
{
	public interface IEntitySystemContext
	{
		void AddComponent(IComponent component);

		void RemoveComponent(IComponent component);
	}
}
