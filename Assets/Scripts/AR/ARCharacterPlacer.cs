using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

namespace TequilaSunrise.AR
{
    [RequireComponent(typeof(ARRaycastManager))]
    public class ARCharacterPlacer : MonoBehaviour
    {
        [Header("Character Setup")]
        [SerializeField] private GameObject characterPrefab;
        [SerializeField] private float characterScale = 1.0f;
        [SerializeField] private bool autoPlace = false;
        
        [Header("Placement Settings")]
        [SerializeField] private bool allowMultiple = false;
        [SerializeField] private float minPlacementDistance = 0.5f;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;

        private ARRaycastManager raycastManager;
        private GameObject spawnedCharacter;
        private List<ARRaycastHit> hits = new List<ARRaycastHit>();
        private Camera arCamera;

        private void Awake()
        {
            raycastManager = GetComponent<ARRaycastManager>();
            arCamera = GetComponentInChildren<Camera>();
        }

        private void Update()
        {
            if (!characterPrefab) return;

            if (autoPlace && !spawnedCharacter)
            {
                TryAutoPlacement();
                return;
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    if (allowMultiple || !spawnedCharacter)
                    {
                        PlaceCharacterAtTouch(touch.position);
                    }
                }
            }
        }

        private void TryAutoPlacement()
        {
            Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            Ray ray = arCamera.ScreenPointToRay(screenCenter);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.CompareTag("ARPlane"))
                {
                    PlaceCharacter(hit.point, Quaternion.Euler(0, arCamera.transform.eulerAngles.y, 0));
                }
            }
        }

        private void PlaceCharacterAtTouch(Vector2 touchPosition)
        {
            if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
                
                // Check minimum distance if character exists
                if (spawnedCharacter && !allowMultiple)
                {
                    float distance = Vector3.Distance(hitPose.position, spawnedCharacter.transform.position);
                    if (distance < minPlacementDistance) return;
                }

                // Calculate rotation to face camera
                Vector3 forward = arCamera.transform.position - hitPose.position;
                forward.y = 0;
                Quaternion rotation = Quaternion.LookRotation(forward);

                PlaceCharacter(hitPose.position, rotation);
            }
        }

        private void PlaceCharacter(Vector3 position, Quaternion rotation)
        {
            if (!allowMultiple && spawnedCharacter)
            {
                Destroy(spawnedCharacter);
            }

            spawnedCharacter = Instantiate(characterPrefab, position, rotation);
            spawnedCharacter.transform.localScale = Vector3.one * characterScale;

            if (showDebugInfo)
            {
                Debug.Log($"Character placed at position: {position}, rotation: {rotation.eulerAngles}");
            }

            // Enable character components
            var animator = spawnedCharacter.GetComponent<Animator>();
            if (animator) animator.enabled = true;

            var controller = spawnedCharacter.GetComponent<AvatarController>();
            if (controller) controller.enabled = true;
        }

        public void ResetCharacter()
        {
            if (spawnedCharacter)
            {
                Destroy(spawnedCharacter);
                spawnedCharacter = null;
            }
        }

        public GameObject GetSpawnedCharacter()
        {
            return spawnedCharacter;
        }
    }
}