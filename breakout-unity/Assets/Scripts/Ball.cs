using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour {
    [SerializeField] private GameObject _hitEffect;

    [SerializeField] private float _minSpeed;
    [SerializeField] private float _speedIncrement;
    [SerializeField] private float _maxSpeed;
    
    private float _speed;
    
    [SerializeField] private float _radius;

    public Vector3 direction;
    private Vector3 _startScale;

    public bool canHitPaddle = true;
    private int _paddleLayerMask;
    private int _noPaddleLayerMask;

    private void Awake() {
        _paddleLayerMask = LayerMask.GetMask("Default", "Paddle");
        _noPaddleLayerMask = LayerMask.GetMask("Default");
        
        direction = new Vector3(Random.Range(0.2f, 0.4f), 1f, 0);
        direction = direction.normalized;

        _startScale = transform.localScale;

        _speed = _minSpeed;
    }

    private void Update()
    {
        var previousPos = transform.position;

        var movement = _speed * Time.deltaTime;

        var hit = Physics2D.CircleCast(previousPos, _radius, direction, movement, canHitPaddle ? _paddleLayerMask : _noPaddleLayerMask);
        
        if (hit.collider != null) {
            var paddle = hit.collider.GetComponent<Paddle>();
            if (paddle != null) {
                RedirectByPaddle(hit.point, paddle);
                canHitPaddle = false;
            }
            else {
                direction = Vector2.Reflect(direction, hit.normal);
                canHitPaddle = true;
            }
            
            var hittable = hit.collider.GetComponentInParent<IHittable>();

            if (hittable != null) {
                hittable.OnHit(hit, this);
            }

            if (!this) {
                return;
            }

            var block = hit.collider.GetComponent<Block>();
            if (block) {
                _speed = Mathf.Min(_speed + _speedIncrement, _maxSpeed);
            }

            var distToWall = hit.distance - _radius;
            var movementLeft = movement - distToWall;

            transform.position = previousPos + direction * (hit.distance - _radius) + direction * movementLeft;

            SpawnHitEffect(hit.point);
        } else {
            transform.position += direction * movement;
        }
        
        transform.up = direction;

        var elongation = _speed / _minSpeed;
        transform.localScale = new Vector3(_startScale.x * (1 / elongation), _startScale.y * elongation, _startScale.z);
    }

    private void SpawnHitEffect(Vector2 hitPoint) {
        Instantiate(_hitEffect, hitPoint, Quaternion.identity);
    }

    private void RedirectByPaddle(Vector2 hitPoint, Paddle paddle) {
        var xOffset = paddle.transform.position.x - hitPoint.x;

        direction = Vector2.Reflect(direction, Vector2.up);

        direction.x -= xOffset;
        direction = direction.normalized;
    }
}
