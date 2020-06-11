using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _radius;
    
    private Vector2 _velocity;
    
    private void Awake() {
        _velocity = Vector2.down * _speed;
    }

    private void FixedUpdate()
    {
        var previousPos = transform.position;

        transform.position += (Vector3) _velocity * Time.fixedDeltaTime;

        var ray = new Ray(previousPos, (transform.position - previousPos).normalized);
        var dist = Vector3.Distance(previousPos, transform.position);
        if (Physics.SphereCast(ray, _radius, out var info, dist, LayerMask.GetMask("Default"))) {
            _velocity = Vector2.Reflect(_velocity, info.normal);
            
            
        }
    }
}
