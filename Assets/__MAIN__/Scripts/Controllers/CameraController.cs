using UnityEngine;
using Managers;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] Vector3 cameraBoundsMin = Vector3.zero;
        [SerializeField] Vector3 cameraBoundsMax = Vector3.zero;

        [SerializeField] float dragSpeed = 5f;

        InputManager inputManager;

        Vector3 dragStartPos = Vector3.zero;
        Vector3 dragPos = Vector3.zero;

        Camera mainCamera;

        bool isDragging = false;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            if (inputManager == null) inputManager = InputManager.Instance;

            inputManager.OnTouchBegan.AddListener(OnTouchBegan);
            inputManager.OnTouchMoved.AddListener(TouchMove);
            inputManager.OnTouchStationary.AddListener(TouchMove);
            inputManager.OnTouchEnded.AddListener(TouchEnded);
        }

        private void OnDisable() 
        {
            if (inputManager == null) inputManager = InputManager.Instance;

            inputManager.OnTouchBegan.RemoveListener(OnTouchBegan);
            inputManager.OnTouchMoved.RemoveListener(TouchMove);
            inputManager.OnTouchStationary.RemoveListener(TouchMove);
            inputManager.OnTouchEnded.RemoveListener(TouchEnded);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 [] points = new Vector3[4];
            points[0] = new Vector3(cameraBoundsMin.x, 0, cameraBoundsMax.z);
            points[1] = new Vector3(cameraBoundsMin.x, 0, cameraBoundsMin.z);
            points[2] = new Vector3(cameraBoundsMax.x, 0, cameraBoundsMin.z);
            points[3] = new Vector3(cameraBoundsMax.x, 0, cameraBoundsMax.z);

            Gizmos.DrawLine(points[0], points[1]);
            Gizmos.DrawLine(points[1], points[2]);
            Gizmos.DrawLine(points[2], points[3]);
            Gizmos.DrawLine(points[3], points[0]);
        }

        void OnTouchBegan(Touch touch)
        {
            isDragging = true;

            var ray = mainCamera.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out var hit))
            {
                dragStartPos = hit.point;
            }
        }

        void TouchMove(Touch touch)
        {
            if (!isDragging) return;

            var ray = mainCamera.ScreenPointToRay(touch.position);

            if (Physics.Raycast(ray, out var hit))
            {
                dragPos = hit.point;
            }

            var dragDeltaPos = dragPos - dragStartPos;

            var pos = transform.position;
            pos.x -= dragDeltaPos.x * Time.deltaTime * dragSpeed;
            pos.z -= dragDeltaPos.z * Time.deltaTime * dragSpeed;

            pos.x = Mathf.Clamp(pos.x, cameraBoundsMin.x, cameraBoundsMax.x);
            pos.z = Mathf.Clamp(pos.z, cameraBoundsMin.z, cameraBoundsMax.z);

            transform.position = pos;
        }

        void TouchEnded(Touch touch)
        {
            isDragging = false;
        }
    }
}