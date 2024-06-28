using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Utilities;

namespace Managers
{
    public class InputManager : SingletonMonoBehaviour<InputManager>
    {
        public UnityEvent<Touch> OnTouchBegan;
        public UnityEvent<Touch> OnTouchStationary;
        public UnityEvent<Touch> OnTouchMoved;
        public UnityEvent<Touch> OnTouchEnded;

        private void Update()
        {
            if (Input.touchCount == 0) return;

            Touch touch = Input.GetTouch(0);

            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnTouchBegan?.Invoke(touch);
                    break;
                case TouchPhase.Stationary:
                    OnTouchStationary?.Invoke(touch);
                    break;
                case TouchPhase.Moved:
                    OnTouchMoved?.Invoke(touch);
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    OnTouchEnded?.Invoke(touch);
                    break;
            }
        }
    }
}