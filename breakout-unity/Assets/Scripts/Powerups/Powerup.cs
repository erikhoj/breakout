using System;
using DG.Tweening;
using UnityEngine;

public abstract class Powerup : MonoBehaviour {
	[SerializeField] private float _fallSpeed;

	private Rigidbody2D _rigidBody;

	private void Awake() {
		_rigidBody = GetComponent<Rigidbody2D>();
		
		transform.DOScale(transform.localScale, 0.4f).ChangeStartValue(Vector3.zero).SetEase(Ease.OutSine);
	}
	
	private void OnCollisionEnter2D(Collision2D other) {
		Trigger();
		Destroy(gameObject);
	}

	private void FixedUpdate() {
		_rigidBody.MovePosition(_rigidBody.position + Vector2.down * _fallSpeed * Time.fixedDeltaTime);	
	}

	protected abstract void Trigger();
}
