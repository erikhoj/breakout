using UnityEngine;

public interface IHittable {
	void OnHit(RaycastHit2D hit, Vector2 direction);
}
