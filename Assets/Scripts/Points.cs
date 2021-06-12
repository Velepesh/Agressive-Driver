using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Points : MonoBehaviour
{
    public int BoardingCount => _boardingCount;
    public int NearMissCount => _nearMissCount;
    [SerializeField] private Player _player;
    [SerializeField] private NearMissChecker _checker;

    private List<Enemy> _boarding = new List<Enemy>();
    private List<Enemy> _nearMiss = new List<Enemy>();
    private bool _isCollision = false;
    private Coroutine _calculatePoints;

    private int _boardingCount = 0;
    private int _nearMissCount = 0;

    public event Action<int> Collected;
    public event Action<string, int> CollectedWithText;

    private void OnEnable()
    {
        _player.Boarding += OnBoarding;
        _player.PlayerHited += OnCollision;
        _checker.NearMissed += NearMiss;
    }

    private void OnDisable()
    {
        _player.Boarding -= OnBoarding;
        _player.PlayerHited -= OnCollision;
        _checker.NearMissed -= NearMiss;
    }

    private void OnCollision(Vector3 vector)
    {
        _isCollision = true;
        StartCalculate();
    }

    private void NearMiss(Enemy enemy)
    {
        if (_nearMiss.Contains(enemy) == false)
        {
            _nearMiss.Add(enemy);
            StartCalculate();
        }
    }

    private void OnBoarding(Enemy enemy)
    {
        if (_boarding.Contains(enemy) == false)
        {
            _boarding.Add(enemy);
            StartCalculate();
        }
    }

    private void StartCalculate()
    {
        if (_calculatePoints != null)
            StopCoroutine(_calculatePoints);
        _calculatePoints = StartCoroutine(CalculatePoints());
    }

    private IEnumerator CalculatePoints()
    {
        yield return new WaitForSeconds(0.5f);
        var point = 0;
        var typePoint = "";
        if (_player.Health > 0)
        {
            if (_boarding.Count > 0)
            {
                point = _boarding.Count * 50;
                typePoint = "BRUTAL";
                _boardingCount += _boarding.Count;
            }
            else if (_isCollision == false && _nearMiss.Count > 0)
            {
                point = _nearMiss.Count * 100;
                typePoint = "NEAR MISS";
                _nearMissCount += _nearMiss.Count;
            }
            CollectedWithText?.Invoke(typePoint, point);
            Collected?.Invoke(point);
            _boarding.Clear();
            _nearMiss.Clear();
            _isCollision = false;
        }
    }
}
