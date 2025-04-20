using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MultipleImagesTrackingManager : MonoBehaviour
{
    // Prefabs to Spawn
    [SerializeField] List<GameObject> prefabsToSpawn = new List<GameObject>();

    // ARTrackedImageManager reference
    private ARTrackedImageManager _trackedImageManager;

    // Dictionary to reference spawned prefabs with tracked images
    private Dictionary<string, GameObject> _arObjects;

    // Initialization and references assigning
    private void Start()
    {
        _trackedImageManager = GetComponent<ARTrackedImageManager>();
        if (_trackedImageManager == null)
        {
            // Debug.LogError("ARTrackedImageManager not found on this GameObject.");
            return;
        }
        _trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
        _arObjects = new Dictionary<string, GameObject>();
        SetupSceneElements(); // Call to setup prefabs
    }

    private void OnDestroy()
    {
         _trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
    }

    // Setup Scene Elements
    private void SetupSceneElements(){
        foreach (var prefab in prefabsToSpawn)
        {

         var arObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
         arObject.name = prefab.name; // Set the name of the prefab to match the original prefab name
            arObject.gameObject.SetActive(false); // Initially hide the prefab
            _arObjects.Add(prefab.name, arObject); // Add to dictionary with prefab name as key

        }
    }

    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateTrackedImages(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateTrackedImages(trackedImage);

        }

        foreach (var trackedImage in eventArgs.removed)
        {
            UpdateTrackedImages(trackedImage.Value);
        }
    }

    // Update is called once per frame
    private void UpdateTrackedImages(ARTrackedImage trackedImage)
    {
        if (trackedImage == null)
        {
            // Debug.LogError("TrackedImage is null.");
            return;
        }
        if (trackedImage.trackingState is TrackingState.Limited or TrackingState.None)
        {
            if (_arObjects.TryGetValue(trackedImage.referenceImage.name, out var arObject))
            {
                _arObjects[trackedImage.referenceImage.name].SetActive(false); // Hide the prefab when not tracking
                return;
            }
        }
        _arObjects[trackedImage.referenceImage.name].SetActive(true); // Show the prefab when tracking
        _arObjects[trackedImage.referenceImage.name].transform.position = trackedImage.transform.position; // Update position
        _arObjects[trackedImage.referenceImage.name].transform.rotation = trackedImage.transform.rotation; // Update rotation
    }
}
