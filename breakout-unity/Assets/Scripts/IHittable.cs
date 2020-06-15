using UnityEngine;

public interface IHittable {
	void OnHit(RaycastHit2D hit, Ball ball);
}
