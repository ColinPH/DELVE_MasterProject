using System.Collections;
using UnityEngine;
using System;

namespace PropellerCap
{
    [Serializable]
    public class Trajectory
    {
        public Trajectory(Vector3 startPos, Vector3 endPos, float tempCurvature = 0f)
        {
            startPosition = startPos;
            endPosition = endPos;
            curvature = tempCurvature;
        }

        public float curvature = 1f;
        public Vector3 startPosition = new Vector3();
        public Vector3 endPosition = new Vector3();

        public bool hasMovingTarget = false;
        public Transform movingTransformStart;
        public Transform movingTransformEnd;

        public Vector3 GetIntermediatePoint()
        {
            Vector3 startPoint = startPosition;
            Vector3 endPoint = endPosition;

            if (hasMovingTarget)
            {
                startPoint = movingTransformStart.position;
                endPoint = movingTransformEnd.position;
            }

            Vector3 intermediatePoint = startPoint + (endPoint - startPoint) / 2;
            intermediatePoint.y += curvature * (Mathf.Abs(Mathf.Abs(endPoint.y) - Mathf.Abs(startPoint.y)) + 1);
            return intermediatePoint;
        }

        /// <summary>
        /// Returns the position along the trajectory corresponding to the given 0-100 percentage
        /// </summary>
        /// <param name="percentage">Ranging from 0 to 100</param>
        public Vector3 GetPointOnTrajectory(float percentage)
        {
            if (percentage > 100) percentage = 100;

            percentage = percentage / 100;

            Vector3 intermediatePoint = GetIntermediatePoint();
            Vector3 startPoint = startPosition;
            Vector3 endPoint = endPosition;

            if (hasMovingTarget)
            {
                startPoint = movingTransformStart.position;
                endPoint = movingTransformEnd.position;
            }

            //Calculate the steps of the two sides
            Vector3 upPoint = startPoint + (intermediatePoint - startPoint) * percentage;
            Vector3 downPoint = intermediatePoint + (endPoint - intermediatePoint) * percentage;

            Vector3 intermediateLine = downPoint - upPoint;

            return upPoint + intermediateLine * percentage;
        }

        public Vector3[] GetPointsOnTrajectory(int amountPoints)
        {
            Vector3[] toReturn = new Vector3[amountPoints];

            //Assign the first and last positions of the line
            toReturn[0] = startPosition;
            toReturn[amountPoints - 1] = endPosition;

            for (int i = 1; i < amountPoints - 1; i++)
            {
                float percentageStep = 100f / amountPoints;
                toReturn[i] = GetPointOnTrajectory(i * percentageStep);
            }

            return toReturn;
        }

        public float GetTrajectoryLength()
        {
            int smoothness = Mathf.CeilToInt(Vector3.Distance(startPosition, endPosition)) * 5;
            float totalDistance = 0f;
            Vector3[] points = GetPointsOnTrajectory(smoothness);
            for (int i = 0; i < smoothness - 1; i++)
            {
                totalDistance += Vector3.Distance(points[i], points[i + 1]);
            }
            return totalDistance;
        }
    }
}