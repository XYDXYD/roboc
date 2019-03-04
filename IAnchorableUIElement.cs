using UnityEngine;

public interface IAnchorableUIElement
{
	void AnchorThisElementUnder(IAnchorUISource other);

	void AnchorThisElementUnder(UIRect otherWidget);

	void ReparentOnly(Transform other);
}
