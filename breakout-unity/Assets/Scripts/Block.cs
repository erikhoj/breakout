using UnityEngine;

public class Block : MonoBehaviour, IHittable {
	public int hits = 1;

	public void OnHit(RaycastHit2D hit) {
		hits -= 1;

		if (hits == 0) {
			Destroy(gameObject);
		}
	}
}
