using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    [SerializeField]
    private WavepointPath _WavepointPath;

    [SerializeField]
    private float _speed;

    private int _targetWavepointIndex;

    private Transform _previousWavepoint;
    private Transform _targetWavepoint;

    private float _timeToWavepoint;
    private float _elapsedTime;

    void Start()
    {
        TargetNextWavepoint();
    }

    void FixedUpdate()
    {
        _elapsedTime += Time.deltaTime;

        float elapsedPercentage = _elapsedTime / _timeToWavepoint;
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
        transform.position = Vector3.Lerp(_previousWavepoint.position, _targetWavepoint.position, elapsedPercentage);

        if (elapsedPercentage >= 1)
        {
            TargetNextWavepoint();
        }
    }

    private void TargetNextWavepoint()
    {
        _previousWavepoint = _WavepointPath.GetWavepoint(_targetWavepointIndex);
        _targetWavepointIndex = _WavepointPath.GetNextWavepointIndex(_targetWavepointIndex);
        _targetWavepoint = _WavepointPath.GetWavepoint(_targetWavepointIndex);

        _elapsedTime = 0;

        float distanceToWavepoint = Vector3.Distance(_previousWavepoint.position, _targetWavepoint.position);
        _timeToWavepoint = distanceToWavepoint / _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
    }
}
