using UnityEngine;

namespace Utilities
{
    public class LevelSpawner : SingletonMonoBehaviour<LevelSpawner>
    {
        [SerializeField] Transform spawnAreaMaster;
        [SerializeField] Transform spawnAreaOpponent;
        [SerializeField] Transform cameraParent;

        [SerializeField] Material masterMaterial;
        [SerializeField] Material opponentMaterial;

        [SerializeField] Material masterMaterialSelected;
        [SerializeField] Material opponentMaterialSelected;

        [SerializeField] LayerMask groundLayer;

        [SerializeField] string characterPrefabName;

        bool isMaster = false;

        void Start()
        {
            if (spawnAreaMaster == null || spawnAreaOpponent == null)
            {
                Debug.LogError("SpawnAreas are not assigned");
                return;
            }
            if (cameraParent == null)
            {
                Debug.LogError("Camera parent is not assigned");
                return;
            }

            if (PhotonNetwork.isMasterClient)
            {
                cameraParent.position = spawnAreaMaster.position;
                isMaster = true;
            }
            else
            {
                cameraParent.position = spawnAreaOpponent.position;
                var euler = cameraParent.rotation.eulerAngles;
                euler.y -= 180f;
                cameraParent.rotation = Quaternion.Euler(euler);
                isMaster = false;
            }

            SpawnCharacters();
        }

        public (Material, Material) GetMaterial(bool master)
        {
            if (master)
                return (masterMaterial, masterMaterialSelected);
            else
                return (opponentMaterial, opponentMaterialSelected);
        }

        internal void SpawnCharacters()
        {
            SphereCollider collider;
            Material mat;

            if (isMaster)
            {
                collider = spawnAreaMaster.GetComponent<SphereCollider>();
                mat = masterMaterial;
            }
            else
            {
                collider = spawnAreaOpponent.GetComponent<SphereCollider>();
                mat = opponentMaterial;
            }

            for (int i = 0; i < 3; i++)
            {
                var pos = collider.transform.position + (Random.insideUnitSphere * collider.radius);
                pos.y = 10f;
                var ray = new Ray(pos, Vector3.down);
                if (Physics.Raycast(ray, out var hit, 20f, groundLayer))
                {
                    pos = hit.point;
                }

                SpawnInArea(pos);
            }
        }

        internal void SpawnInArea(Vector3 position)
        {
            var gObj = PhotonNetwork.Instantiate(characterPrefabName, position, Quaternion.identity, 0);
        }
    }
}