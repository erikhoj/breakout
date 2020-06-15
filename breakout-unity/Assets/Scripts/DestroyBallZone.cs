using UnityEngine;

namespace DefaultNamespace {
	public class DestroyBallZone : MonoBehaviour, IHittable {
		public void OnHit(RaycastHit2D hit, Ball ball) {
			Destroy(ball.gameObject);
		}
	}
}