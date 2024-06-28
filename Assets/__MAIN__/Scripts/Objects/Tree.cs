using Photon;
using UnityEngine;

namespace Objects
{
    public class Tree : PunBehaviour, IDamagable, IHarvestable, IPunObservable
    {
        [field: SerializeField]
        public float Health { get; private set; }
        public bool isCollected { get { return Health <= 0; } }

        void Start()
        {
            Health = Random.Range(50f, 100f);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
        }

        public float Collect()
        {
            Debug.Log("Collecting " + gameObject.name);

            var damage = 5f;
            GetDamage(damage);
            return damage;
        }

        public void GetDamage(float damageValue)
        {
            Health -= damageValue;

            if (Health <= 0)
                photonView.RPC("Die", PhotonTargets.AllBuffered);
        }

        [PunRPC]
        void Die()
        {
            if(photonView.isMine)
                PhotonNetwork.Destroy(gameObject);
        }

    }
}