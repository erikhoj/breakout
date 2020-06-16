using System;
using DefaultNamespace;
using UnityEngine;

public class Level : MonoBehaviour {
	[SerializeField] private BlockSpawner _blockSpawner;
	[SerializeField] private DestroyBallZone _ballDestroyer;
	
	[SerializeField] private Ball _ballPrefab;
	
	private Paddle _paddle;
	private Ball _ball;
	private Vector3 _ballSpawnPos;
	private int _index;

	public event Action won;
	public event Action lost;

	private void Awake() {
		_paddle = GetComponentInChildren<Paddle>();
		_ball = GetComponentInChildren<Ball>();

		_paddle.enabled = false;
		_ball.enabled = false;
		_ballSpawnPos = _ball.transform.localPosition;

		_blockSpawner.allDestroyed += () => won?.Invoke();
		_ballDestroyer.destroyedBall += OnBallDestroyed;
	}

	private void OnBallDestroyed() {
		var balls = GetComponentsInChildren<Ball>();

		if (balls.Length == 0) {
			lost?.Invoke();
		}
	}

	public void SetIndex(int index) {
		_blockSpawner.SpawnLevel(index);
		_index = index;
	}

	public void Reset() {
		_blockSpawner.SpawnLevel(_index);
		_paddle.transform.localPosition = new Vector3(0, _paddle.transform.localPosition.y, _paddle.transform.localPosition.z);

		foreach (var ball in GetComponentsInChildren<Ball>()) {
			Destroy(ball.gameObject);
		}

		foreach (var powerup in GetComponentsInChildren<Powerup>()) {
			Destroy(powerup.gameObject);
		}

		_ball = Instantiate(_ballPrefab, transform);
		_ball.transform.localPosition = _ballSpawnPos;

		_ball.enabled = false;
		_paddle.enabled = false;
	}

	public void StartLevel() {
		_paddle.enabled = true;
		_ball.enabled = true;
	}
}
