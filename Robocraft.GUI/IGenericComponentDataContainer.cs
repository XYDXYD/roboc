namespace Robocraft.GUI
{
	public interface IGenericComponentDataContainer
	{
		T UnpackData<T>();

		void PackData<T>(T data);
	}
}
