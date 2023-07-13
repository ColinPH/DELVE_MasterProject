using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class AIMoveBehaviourPatrol : MonoBehaviour
    {
        public float _speed = 2f;
        public List<GameObject> _patrolPoints = new List<GameObject>();

        GameObject _previousPoint;
        GameObject _nextPoint;
        float _progress = 0f;
        int pointindex = 0;

        Vector3 firstPoint = new Vector3();
        bool _targettingFirstPoint = true;

        private void Start()
        {
            if (_patrolPoints.Count <= 1)
                Debug.LogError("Not enough patrol points on " + gameObject.name);

            //_previousPoint = transform.position;
            _nextPoint = _patrolPoints[0];
            _progress = 0f;

            firstPoint = transform.position;
        }

        private void Update()
        {
            _progress += _speed * Time.deltaTime;
            if (_targettingFirstPoint)
            {
                transform.position = Vector3.Lerp(firstPoint, _nextPoint.transform.position, _progress);
            }
            else
            {
                transform.position = Vector3.Lerp(_previousPoint.transform.position, _nextPoint.transform.position, _progress);
            }


            if (_progress >= 0.97f)
            {
                _targettingFirstPoint = false;
                //Change target point
                _progress = 0f;

                _previousPoint = _patrolPoints[pointindex];
                pointindex++;

                if (pointindex >= _patrolPoints.Count)
                    pointindex = 0;

                _nextPoint = _patrolPoints[pointindex];
            }
        }
    }
}