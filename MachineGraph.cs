using Simulation;

internal sealed class MachineGraph
{
	public CubeNodeInstance root;

	public MachineCluster cluster;

	public MicrobotCollisionSphere sphere;

	public static void MakeLink(CubeNodeInstance nodeA, CubeNodeInstance nodeB, ConnectionPoint nodeAConnectionPoint)
	{
		nodeA.AddLink(nodeB, nodeAConnectionPoint);
	}
}
