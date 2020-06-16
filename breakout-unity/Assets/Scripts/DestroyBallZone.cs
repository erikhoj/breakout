using System;
using UnityEngine;

namespace DefaultNamespace {
	public class DestroyBallZone : MonoBehaviour, IHittable {
		public event Action destroyedBall;
		
		public void OnHit(RaycastHit2D hit, Ball ball) {
			DestroyImmediate(ball.gameObject);
			
			destroyedBall?.Invoke();
		}
	}
}