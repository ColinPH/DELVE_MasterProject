using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class TrajectoryProjectileMover : ProjectileMoverBase
    {
        public float speed = 20f;

        private float _currentTrajPercentage = 0f;
        private Trajectory _trajectory;
        private bool _isMoving = false;
        private float _trajectoryLength = -1f;
        private float _positionOnTrajectory = -1f;
        private bool _lookAtTrajectory = true;

        protected override void m_FixedUpdate()
        {
            base.m_FixedUpdate();

            if (_trajectory == null)
                return;

            if (_trajectory.hasMovingTarget)
            {
                //Check that the 2 transforms are not destroyed
                if (_trajectory.movingTransformEnd == null || _trajectory.movingTransformStart == null)
                {
                    //destroy the object and stop the movement
                    Destroy(gameObject);
                    return;
                }
            }

            if (_isMoving)
            {
                _MoveAlongTrajectory();
            }

            if (_currentTrajPercentage >= 100f && _isMoving)
            {
                _isMoving = false;
                _OnTrajectoryEnded();
            }
        }

        public void StartMoving(Trajectory traj, bool lookAtTrajectory = true)
        {
            _trajectory = traj;
            _isMoving = true;
            _lookAtTrajectory = lookAtTrajectory;
            _trajectoryLength = _trajectory.GetTrajectoryLength();
            transform.position = _trajectory.GetPointOnTrajectory(_currentTrajPercentage);
            if (_lookAtTrajectory)
                transform.LookAt(_trajectory.GetPointOnTrajectory(_currentTrajPercentage + 1));
        }

        protected override void m_StopMoving()
        {
            base.m_StopMoving();
            _isMoving = false;
        }

        private void _MoveAlongTrajectory()
        {
            _positionOnTrajectory += speed * Time.fixedDeltaTime;
            _currentTrajPercentage = _positionOnTrajectory / _trajectoryLength;

            transform.position = _trajectory.GetPointOnTrajectory(_currentTrajPercentage);
            if (_lookAtTrajectory)
                transform.LookAt(_trajectory.GetPointOnTrajectory(_currentTrajPercentage + 1));
        }

        private void _OnTrajectoryEnded()
        {
            GetComponent<ProjectileBase>().OnProjectileTrajectoryEnded();
        }
    }
}