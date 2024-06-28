using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    [RequireComponent(typeof(Animator)), RequireComponent(typeof(PhotonAnimatorView))]
    public class AnimatorController : MonoBehaviour
    {
        PhotonAnimatorView photonAnimatorView;
        Animator animator;

        [SerializeField] string idleParameter = "idle";
        [SerializeField] string moveParameter = "move";
        [SerializeField] string chopParameter = "chop";

        void Start()
        {
            animator = GetComponent<Animator>();
            photonAnimatorView = GetComponent<PhotonAnimatorView>();

            photonAnimatorView.SetParameterSynchronized(idleParameter, PhotonAnimatorView.ParameterType.Bool, PhotonAnimatorView.SynchronizeType.Continuous);
            photonAnimatorView.SetParameterSynchronized(moveParameter, PhotonAnimatorView.ParameterType.Bool, PhotonAnimatorView.SynchronizeType.Continuous);
            photonAnimatorView.SetParameterSynchronized(chopParameter, PhotonAnimatorView.ParameterType.Bool, PhotonAnimatorView.SynchronizeType.Continuous);
        }

        public void Idle() => SetParameter(AnimState.Idle);

        public void Move() => SetParameter(AnimState.Move);

        public void Chop() => SetParameter(AnimState.Chop);

        internal void SetParameter(AnimState animState)
        {
            animator.SetBool(idleParameter, false);
            animator.SetBool(moveParameter, false);
            animator.SetBool(chopParameter, false);

            switch (animState)
            {
                case AnimState.Idle:
                    animator.SetBool(idleParameter, true);
                    break;
                case AnimState.Move:
                    animator.SetBool(moveParameter, true);
                    break;
                case AnimState.Chop:
                    animator.SetBool(chopParameter, true);
                    break;
            }
        }

        public enum AnimState
        {
            Idle,
            Move,
            Chop
        }
    }
}