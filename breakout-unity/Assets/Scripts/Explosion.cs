using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Explosion : MonoBehaviour {
	[SerializeField] private Transform _grow;
	[SerializeField] private float _lifeTime;
	[SerializeField] private float _growTime;
	[SerializeField] private float _maxSize;

	private void Start() {
		var growRenderer = _grow.GetComponent<Renderer>();

		_grow.localScale = Vector3.zero;
		
		var sequence = DOTween.Sequence();
		sequence.Append(_grow.DOScale(Vector3.one * _maxSize, _growTime).SetEase(Ease.OutQuad));
		
		// TODO - Clean this up
		var targetColor = growRenderer.material.color;
		targetColor.a = 0;
		sequence.Append(growRenderer.material.DOColor(targetColor, "_BaseColor", _lifeTime - _growTime));
		
		sequence.AppendInterval(4);
		sequence.AppendCallback(Kill);
	}
	
	private void Kill() {
		Destroy(gameObject);
	}
}