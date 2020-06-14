using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour {
    [SerializeField] private GameObject _hitEffect;

    [SerializeField] private float _speed;
    [SerializeField] private float _radius;
    
    private Vector3 _direction;
    private Vector3 _startScale;
    
    private void Awake() {
        _direction = Random.insideUnitCircle.normalized;
        _direction.y = Mathf.Max(0.7f, Mathf.Abs(_direction.y));
        _direction = _direction.normalized;

        _startScale = transform.localScale;
    }

    private void Update()
    {
        var previousPos = transform.position;

        var movement = _speed * Time.deltaTime;

        var hit = Physics2D.CircleCast(previousPos, _radius, _direction, movement, LayerMask.GetMask("Default"));

        if (hit.collider != null) {
            _direction = Vector2.Reflect(_direction, hit.normal);

            var paddle = hit.collider.GetComponent<Paddle>();
            if (paddle != null) {
                RedirectByPaddle(hit.point, paddle);
            }

            var distToWall = hit.distance - _radius;
            var movementLeft = movement - distToWall;

            transform.position = previousPos + _direction * (hit.distance - _radius) + _direction * movementLeft;

            var hittable = hit.collider.GetComponentInParent<IHittable>();

            if (hittable != null) {
                hittable.OnHit(hit);
            }

            SpawnHitEffect(hit.point);
        } else {
            transform.position += _direction * movement;
        }
        
        transform.up = _direction;

        var elongation = _speed / 5f;
        transform.localScale = new Vector3(_startScale.x * (1 / elongation), _startScale.y * elongation, _startScale.z);
    }

    private void SpawnHitEffect(Vector2 hitPoint) {
        Instantiate(_hitEffect, hitPoint, Quaternion.identity);
    }

    private void RedirectByPaddle(Vector2 hitPoint, Paddle paddle) {
        var xOffset = paddle.transform.position.x - hitPoint.x;

        _direction.x -= xOffset;
        _direction = _direction.normalized;
    }
}
