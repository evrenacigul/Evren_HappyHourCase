using UnityEngine;
using Managers;

namespace Controllers
{
    public class SelectionController : MonoBehaviour
    {
        InputManager inputManager;
        Camera mainCamera;
        GameObject selectedObject;

        [SerializeField] LayerMask selectionLayers;

        void Start()
        {
            mainCamera = Camera.main;
        }

        void OnEnable()
        {
            if (inputManager == null) inputManager = InputManager.Instance;

            inputManager.OnTouchBegan.AddListener(OnTouchBegan);
        }

        void OnDisable()
        {
            if (inputManager == null) inputManager = InputManager.Instance;

            inputManager.OnTouchBegan.RemoveListener(OnTouchBegan);
        }

        private void OnTouchBegan(Touch touch)
        {
            var ray = mainCamera.ScreenPointToRay(touch.position);
            
            if(Physics.Raycast(ray, out var hit, 100f, selectionLayers))
            {
                var selectable = hit.collider.GetComponent<ISelectable>();
                if (selectable != null)
                {
                    var gObj = hit.collider.gameObject;

                    if (selectedObject != gObj)
                    {
                        if (selectedObject != null)
                        {
                            var selectedBefore = selectedObject.GetComponent<ISelectable>();
                            if (selectedBefore != null) selectedBefore.Deselect();
                        }

                        selectedObject = gObj;
                        selectable.Select();
                        Debug.Log("Character selected");
                    }
                    else
                    {
                        selectedObject = null;
                        selectable.Deselect();
                        Debug.Log("Character deselected");
                    }
                }
                else if (hit.collider.CompareTag("Ground") && selectedObject != null)
                {
                    var moveable = selectedObject.GetComponent<IMoveable>();
                    
                    if(moveable != null)
                    {
                        Debug.Log("Ground moving");

                        moveable.Move(hit.point);
                    }
                }
                else if (hit.collider.CompareTag("Tree") && selectedObject != null)
                {
                    var harvestable = hit.collider.GetComponent<IHarvestable>();
                    var harvester = selectedObject.GetComponent<IHarvester>();

                    if (harvestable != null && harvester != null)
                    {
                        Debug.Log("Harvesting tree");

                        harvester.Harvest(harvestable, hit.point);
                    }
                }
            }
        }
    }
}