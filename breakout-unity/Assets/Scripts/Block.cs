using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Block : MonoBehaviour, IHittable {
	[SerializeField] private GameObject _lifePrefab;
	[SerializeField] private SpriteRenderer _boxRenderer;
	[SerializeField] private Color[] _colors;
	[SerializeField] private Transform _graphicsParent;
	
	[SerializeField] private Transform _lifeParent;
	
	private List<GameObject> _lives = new List<GameObject>();
	
	private int _hits = 1;

	private void Awake() {
		_lifePrefab.SetActive(false);
	}
	
	public void SetHits(int hits) {
		_hits = hits;
		
		var numBoxes = hits - 1f;

		var spaceBetween = 0.1f;
		var startPos = -((numBoxes - 1f) / 2f) * spaceBetween;
		for (var i = 0; i < numBoxes; i++) {
			var instance = Instantiate(_lifePrefab, _lifeParent);
			instance.transform.localPosition = Vector3.left * (startPos + spaceBetween * i);
			
			instance.SetActive(true);
			
			_lives.Add(instance);
		}

		UpdateColor();
	}

	private void UpdateColor() {
		var color = GetColor();
		_lives.ForEach(l => {
			var material = l.GetComponent<Renderer>().material;
			material.SetColor("_BaseColor", color);
		});
		
		_boxRenderer.color = color;
	}

	private Color GetColor() {
		return _colors[Mathf.Min(_colors.Length, _hits - 1)];
	}
	
	public void OnHit(RaycastHit2D hit, Vector2 direction) {
		_hits -= 1;

		if (_hits == 0) {
			Destroy(gameObject);
		}
		else {
			var sequence = DOTween.Sequence();

			var targetColor = GetColor();
			sequence.Append(_boxRenderer.DOColor(Color.white, 0.2f).SetEase(Ease.OutSine));
			sequence.Append(_boxRenderer.DOColor(targetColor, 0.1f).SetEase(Ease.OutSine));
			
			//_graphicsParent.DOLocalMove(Vector * 0.1f, 0.1f).SetEase(Ease.OutSine).SetLoops(2, LoopType.Yoyo);
			var offset = hit.point.x - transform.position.x;
			var targetAngle = 30 * offset;
			
			_graphicsParent.eulerAngles = new Vector3(0, 0, 0);
			_graphicsParent.DOLocalRotate(new Vector3(0, 0, targetAngle), 0.1f).SetEase(Ease.OutSine).SetLoops(2, LoopType.Yoyo);
			
			var lifeToRemove = _lives.First();
			_lives.Remove(lifeToRemove);

			foreach (var life in _lives) {
				var r = life.GetComponent<Renderer>();
				var m = r.material;
				m.DOColor(Color.white, "_BaseColor", 0.2f);
				m.DOColor(targetColor, "_BaseColor", 0.2f).SetDelay(0.2f);
			}

			var lifeSequence = DOTween.Sequence();
			var lifeMaterial = lifeToRemove.GetComponent<Renderer>().material;
			lifeSequence.Append(lifeMaterial.DOColor(Color.white, "_BaseColor", 0.2f));

			var outColor = Color.white;
			outColor.a = 0;
			lifeSequence.Append(lifeMaterial.DOColor(outColor, "_BaseColor", 0.2f));
			
			var lifeRigidbody = lifeToRemove.AddComponent<Rigidbody2D>();
			lifeRigidbody.velocity = Vector2.right * 1.5f;
			lifeRigidbody.angularVelocity = -720;
			
			lifeToRemove.transform.SetParent(null);

			lifeSequence.AppendCallback(() => Destroy(lifeToRemove.gameObject));
		}
	}
}
