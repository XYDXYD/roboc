using UnityEngine;

[CreateAssetMenu(fileName = "RealMoneyStoreExtraData", menuName = "ScriptableObjects/RealMoneyStoreSpritesMap", order = 1)]
public class RealMoneyStoreExtraDataScriptableObject : ScriptableObject
{
	public string[] StoreItemSkus;

	public string[] SpriteNames;

	public RealMoneyStoreExtraDataScriptableObject()
		: this()
	{
	}
}
