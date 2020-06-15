using UnityEngine;

namespace Powerups {
	public class ExtraBallPowerup : Powerup {
		[SerializeField] private Ball _ballPrefab;
		
		protected override void Trigger() {
			var position = transform.position+ Vector3.up * 0.3f;

			Instantiate(_ballPrefab, position, Quaternion.identity);
		}
	}
}