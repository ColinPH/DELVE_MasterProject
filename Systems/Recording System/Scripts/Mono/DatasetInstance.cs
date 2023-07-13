using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class DatasetInstance : MonoBehaviour
    {
        [SerializeField, HideInInspector] DatasetItem _dataset;
        [SerializeField, HideInInspector] List<Vector3> _interactionPositions = new List<Vector3>();
        [SerializeField, HideInInspector] Vector3 _deathPosition = new Vector3();
        public DatasetItem dataset => _dataset;

        //Death gizmos
        [HideInInspector] public float deathGizmoRadius = 0.5f;
        [HideInInspector] public Color deathGizmoColor = Color.red;

        //Interaction gizmos
        [HideInInspector] public Color interactionGizmoColor = Color.cyan;

        //Simulation properties
        [HideInInspector] Transform _dummyTransform;

        public GameObject DummyObj => _dummyTransform.gameObject;

        public void SetDataset(DatasetItem dataset)
        {
            _dataset = dataset;
            _dataset.datasetInstance = this;
        }

        public void ProcessDataForDeathGizmos()
        {
            //Take the last position of the player
            _deathPosition = dataset.playerPositions[dataset.playerPositions.Count - 1];
        }

        public void ProcessDataForInteractionGizmos()
        {
             /*_interactionPositions.Clear();
             _interactionPositions.Add();*/
            Debug.Log("Interactions are not implemented");
        }

        private void OnDrawGizmos()
        {
            //Player deaths
            if (dataset.showDeaths)
            {
                Gizmos.color = deathGizmoColor;
                Gizmos.DrawSphere(_deathPosition, deathGizmoRadius);
            }

            //Player interactions
            if (dataset.showInteractions)
            {
                Gizmos.color = interactionGizmoColor;
            }
        }

        /// <summary> Create the dummy if it is not already there. </summary>
        public void CreateDummy(GameObject dummyPrefab)
        {
            if (_dummyTransform == null)
            {
                //Spawn the dummy as a child of this object
                GameObject obj = Instantiate(dummyPrefab);
                _dummyTransform = obj.transform;
                _dummyTransform.parent = transform;
            }
            else
            {
                _dummyTransform.gameObject.SetActive(true);
            }

        }

        public void HideDummy()
        {
            _dummyTransform.gameObject.SetActive(false);
        }

        /// <summary>Places the player dummy at the simulation progression value along the path. </summary>
        public void UpdateDummyPosition()
        {
            _dummyTransform.position = _SamplePlayerPositionAt(dataset.playerPositions, dataset.simulationProgress, out int nextIndex);
            
            //Rotate the dummy so that it looks towards the next point
            Vector3 targetPosition = dataset.playerPositions[nextIndex];
            Vector3 direction = (targetPosition - _dummyTransform.position).normalized;
            Quaternion targetRotation;
            if (direction.magnitude > 0)
                targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            else
                targetRotation = Quaternion.identity;
            _dummyTransform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        }

        private Vector3 _SamplePlayerPositionAt(List<Vector3> points, float simulationProgress, out int nextIndex)
        {
            int index = Mathf.FloorToInt(simulationProgress * (points.Count - 1));
            float lerpValue = simulationProgress * (points.Count - 1) - index;

            if (index >= points.Count - 1)
            {
                nextIndex = index;
                return points[points.Count - 1];
            }
            nextIndex = index + 1;
            return Vector3.Lerp(points[index], points[index + 1], lerpValue);
        }

        /// <summary> Called in the editor Update. Moves the dummy along the path at the playback speed. </summary>
        public void ProgressInSimulation()
        {
            //Increase the simulation progression value
            dataset.simulationProgress += dataset.playbackSpeed * dataset.playbackMultiplier * Time.deltaTime;

            if (dataset.simulationProgress > 1f)
                dataset.simulationProgress = 1f;

            //Update the position of the dummy
            UpdateDummyPosition();
        }
    }
}