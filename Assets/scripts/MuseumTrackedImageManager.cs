using System.Collections.Generic;
using MuseumAR.Data;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace MuseumAR.AR
{
    [System.Serializable]
    public struct ExhibitMapping
    {
        public string referenceImageName;
        public ExhibitData exhibitData;
        public GameObject exhibitPrefab;
    }

    [RequireComponent(typeof(ARTrackedImageManager))]
    public class MuseumTrackedImageManager : MonoBehaviour
    {
        [SerializeField] private List<ExhibitMapping> mappings = new();

        private ARTrackedImageManager _trackedImageManager;
        private readonly Dictionary<string, ExhibitMapping> _mappingByName = new();
        private readonly Dictionary<string, GameObject> _spawnedByName = new();

        private void Awake()
        {
            _trackedImageManager = GetComponent<ARTrackedImageManager>();
            foreach (var mapping in mappings)
            {
                if (!string.IsNullOrWhiteSpace(mapping.referenceImageName) && mapping.exhibitPrefab != null)
                {
                    _mappingByName[mapping.referenceImageName] = mapping;
                }
            }
        }

        private void OnEnable()
        {
            _trackedImageManager.trackablesChanged.AddListener(OnTrackablesChanged);
        }

        private void OnDisable()
        {
            _trackedImageManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
        }

        private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
        {
            foreach (var added in args.added)
            {
                HandleTrackedImage(added);
            }

            foreach (var updated in args.updated)
            {
                HandleTrackedImage(updated);
            }

            foreach (var removed in args.removed)
            {
                string key = removed.Value.referenceImage.name;
                if (_spawnedByName.TryGetValue(key, out var obj) && obj != null)
                {
                    Destroy(obj);
                    _spawnedByName.Remove(key);
                }
            }
        }

        private void HandleTrackedImage(ARTrackedImage trackedImage)
        {
            string imageName = trackedImage.referenceImage.name;
            if (!_mappingByName.TryGetValue(imageName, out var mapping)) return;

            bool isTracking = trackedImage.trackingState == TrackingState.Tracking;

            if (!_spawnedByName.TryGetValue(imageName, out var instance) || instance == null)
            {
                if (!isTracking) return;

                instance = Instantiate(mapping.exhibitPrefab, trackedImage.transform);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.identity;
                _spawnedByName[imageName] = instance;

                if (instance.TryGetComponent<ExhibitObjectController>(out var controller))
                {
                    controller.PlaySpawnSequence();
                }
            }

            bool wasActive = instance.activeSelf;
            instance.SetActive(isTracking);

            if (!wasActive && isTracking && instance.TryGetComponent<ExhibitObjectController>(out var ctrl))
            {
                ctrl.PlaySpawnSequence();
            }
        }
    }
}