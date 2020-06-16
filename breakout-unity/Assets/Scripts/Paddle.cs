using UnityEngine;

public class Paddle : MonoBehaviour {
	[SerializeField] private float _speed;
	
	public Vector2 velocity { get; private set; }
	
	private void Update() {
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
			velocity = Vector2.left * _speed;
		} else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
			velocity = Vector2.right * _speed;
		} else {
			velocity = Vector2.zero;
		}

		transform.position += (Vector3) velocity * Time.deltaTime;

		var posX = Mathf.Clamp(transform.position.x, -5f, 5f);
		transform.position = new Vector3(posX, transform.position.y, transform.position.z);
	}
}
