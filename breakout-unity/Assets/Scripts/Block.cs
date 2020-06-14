using System;
using UnityEngine;

public class Block : MonoBehaviour, IHittable {
	[SerializeField] private SpriteRenderer _boxPrefab;

	private int _hits = 1;

	private void Awake() {
		_boxPrefab.gameObject.SetActive(false);
	}
	
	public void SetHits(int hits) {
		_hits = hits;

		var padding = 0f;

		var minWidth = 0.5f;
		var maxWidth = 1f;
		var minWidthCount = 4f;
		
		for (var i = 0; i < hits; i++) {
			var instance = Instantiate(_boxPrefab, transform);

			var desiredSize = instance.size;
			desiredSize.x -= padding;
			desiredSize.y -= padding;

			var desiredScale = Mathf.Lerp(maxWidth, minWidth, i / minWidthCount);

			instance.transform.localScale = Vector3.one * desiredScale;

			desiredSize /= desiredScale;
			instance.size = desiredSize;

			instance.gameObject.SetActive(true);

			padding += 0.125f * desiredScale;
		}
	}
	
	public void OnHit(RaycastHit2D hit) {
		_hits -= 1;

		if (_hits == 0) {
			Destroy(gameObject);
		}
	}
}
