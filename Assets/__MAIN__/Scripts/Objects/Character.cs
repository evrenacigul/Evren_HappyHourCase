using Pathfinding;
using Photon;
using System.Collections;
using UnityEngine;
using Utilities;
using Controllers;

namespace Objects
{
    [RequireComponent(typeof(AnimatorController)), RequireComponent(typeof(AIPath))]
    public class Character : PunBehaviour, ISelectable, IMoveable, IHarvester, IPunObservable
    {
        [SerializeField] SkinnedMeshRenderer charRenderer;
        [SerializeField] AIPath aiPath;
        [SerializeField] AnimatorController animatorController;

        Material normalMaterial;
        Material selectedMaterial;

        PlayerDataHandler playerDataHandler;

        bool isMaster = false;

        public bool isSelected { get; private set; } = false;

        private void Start()
        {
            if (charRenderer == null) Debug.LogError("charRenderer is not assigned");

            isMaster = PhotonNetwork.isMasterClient;
            aiPath = GetComponent<AIPath>();
            animatorController = GetComponent<AnimatorController>();

            playerDataHandler = PlayerDataHandler.Instance;
        }

        public void ChangeMaterial(bool isMaster)
        {
            if (charRenderer == null) return;

            var material = LevelSpawner.Instance.GetMaterial(isMaster);
            normalMaterial = material.Item1;
            selectedMaterial = material.Item2;

            charRenderer.material = isSelected ? selectedMaterial : normalMaterial;
        }

        public void Select()
        {
            if (!photonView.isMine) return;

            if(!isSelected)
            {
                isSelected = true;
                charRenderer.material = selectedMaterial;
            }
        }

        public void Deselect()
        {
            if (!photonView.isMine) return;

            if (isSelected)
            {
                isSelected = false;
                charRenderer.material = normalMaterial;
            }
        }

        public void Move(Vector3 position)
        {
            if (!photonView.isMine) return;

            StopAllCoroutines();

            StartCoroutine(GoToPosition(position));
        }

        public void Harvest(IHarvestable harvestable, Vector3 position)
        {
            StartCoroutine(GoToHarvest(harvestable, position));
        }

        IEnumerator GoToPosition(Vector3 position)
        {
            var destination = AstarData.active.GetNearest(position);

            //aiPath.isStopped = false;

            aiPath.destination = destination.position;

            animatorController.Move();

            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => aiPath.reachedDestination || Vector3.Magnitude(aiPath.velocity) < 0.01f);

            animatorController.Idle();

            yield return null;
        }

        IEnumerator GoToHarvest(IHarvestable harvestable, Vector3 position)
        {
            var destination = AstarData.active.GetNearest(position);

            aiPath.destination = destination.position;

            animatorController.Move();

            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(() => aiPath.reachedDestination || 
            (Vector3.Magnitude(aiPath.velocity) < 0.01f && Vector3.Distance(transform.position, destination.position) <= 1f));

            var lookAtPos = position;
            lookAtPos.y = transform.position.y;
            transform.LookAt(lookAtPos);

            animatorController.Chop();

            while(!harvestable.isCollected)
            {
                harvestable.Collect();

                yield return new WaitForSeconds(0.5f);

                playerDataHandler.AddWood();
            }

            animatorController.Idle();
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                stream.SendNext(this.isMaster);

                ChangeMaterial(this.isMaster);
            }
            else
            {
                var isMaster = (bool)stream.ReceiveNext();
                
                ChangeMaterial(isMaster);
            }
        }
    }
}
