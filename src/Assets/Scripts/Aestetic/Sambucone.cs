using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sambucone : MonoBehaviour
{
    [SerializeField] Transform _player;
    [SerializeField] float _distance = 150f;
    [SerializeField] float _offsetY = 50f;
    [SerializeField, Min(0)] float _speed = 0.1f;

    [SerializeField] Transform _sprite = null;
    [SerializeField] float _spriteFloatingAmount = 10f;
    [SerializeField] AnimationCurve _floatingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    void Update()
    {
        
        Vector3 newPosition = _player.forward * _distance;
        newPosition = new Vector3(newPosition.x, newPosition.y + _offsetY, newPosition.z);
        transform.position = Vector3.Lerp(transform.position, newPosition, _speed);

        float spriteFloatOffset = _floatingCurve.Evaluate(Mathf.Abs(Mathf.Sin(Time.time))) * _spriteFloatingAmount;
        Vector3 newSpritePosition = new Vector3(0, spriteFloatOffset, 0);
        _sprite.localPosition = newSpritePosition;
        _sprite.LookAt(_player);
    }
}
