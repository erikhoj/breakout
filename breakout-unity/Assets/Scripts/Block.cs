using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Block : MonoBehaviour, IHittable {
	[SerializeField] private GameObject _lifePrefab;
	[SerializeField] private SpriteRenderer _boxRenderer;
	[SerializeField] private Color[] _colors;
	[SerializeField] private Transform _graphicsParent;
	
	[SerializeField] private Transform _lifeParent;

	[SerializeField] private Powerup[] _powerups;

	[SerializeField] private ParticleSystem _deathParticleSystem;
	
	private List<GameObject> _lives = new List<GameObject>();
	
	private int _hits = 1;
	private bool _hasPowerup = false;

	public event Action destroyed;
	
	private void Awake() {
		_lifePrefab.SetActive(false);
	}
	
	public void SetInfo(BlockInfo info) {
		_hits = info.hits;
		_hasPowerup = info.hasPowerup;
		
		var spaceBetween = 0.1f;
		var startPos = -((_hits - 1f) / 2f) * spaceBetween;
		for (var i = 0; i < _hits; i++) {
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
		var index = Mathf.Clamp(_hits - 1, 0, _colors.Length - 1);
		
		return _colors[index];
	}
	
	public void OnHit(RaycastHit2D hit, Ball ball) {
		if (_hits <= 0) {
			return;
		}
		
		_hits -= 1;

		RemoveLife();
		
		if (_hits == 0) {
			DropPowerup();
			//SpawnDeathParticles(ball.direction);
			var sequence = DOTween.Sequence();

			var targetColor = Color.white;
			targetColor.a = 0;
			sequence.Append(_boxRenderer.DOColor(targetColor, 0.3f).SetEase(Ease.OutSine));
			
			sequence.Insert(0, _graphicsParent.DOScale(_graphicsParent.localScale * 1.4f, 0.3f).SetEase(Ease.OutSine));

			GetComponent<BoxCollider2D>().enabled = false;

			sequence.AppendCallback(() => {
				Destroy(gameObject);

				destroyed?.Invoke();
			});
			
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
			
			foreach (var life in _lives) {
				var r = life.GetComponent<Renderer>();
				var m = r.material;
				m.DOColor(Color.white, "_BaseColor", 0.2f);
				m.DOColor(targetColor, "_BaseColor", 0.2f).SetDelay(0.2f);
			}

			//SpawnHitParticles(ball.direction);
		}
	}

	private void SpawnHitParticles(Vector3 direction) {
		var particles = SpawnParticles(direction);

		var emission = particles.emission;
		var burst = emission.GetBurst(0);
		burst.count = 5;
	}

	private void SpawnDeathParticles(Vector3 direction) {
		SpawnParticles(direction);
	}

	private ParticleSystem SpawnParticles(Vector3 direction) {
		var particles = Instantiate(_deathParticleSystem, transform.position, Quaternion.identity);
		particles.gameObject.SetActive(true);
		var velocity = particles.velocityOverLifetime;
		velocity.space = ParticleSystemSimulationSpace.World;
		
		velocity.x = new ParticleSystem.MinMaxCurve(2 * direction.x, 4 * direction.x);
		velocity.y = new ParticleSystem.MinMaxCurve(2 * direction.y, 4 * direction.y);

		return particles;
	}

	private void RemoveLife() {
		var lifeToRemove = _lives.First();
		_lives.Remove(lifeToRemove);
		
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

	private void DropPowerup() {
		if (!_hasPowerup) {
			return;
		}
		
		var randomPowerUp = _powerups[Random.Range(0, _powerups.Length - 1)];
		var powerup = Instantiate(randomPowerUp, transform.position, Quaternion.identity);
		
		powerup.transform.SetParent(transform.parent);
	}
}
