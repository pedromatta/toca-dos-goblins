using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class CombatAnimationController : MonoBehaviour
    {
        private Animator animator;
        private void Awake() => animator = GetComponent<Animator>();
        public void PlayAttack() => animator.SetTrigger("attack");
        public void PlayHit() => animator.SetTrigger("hit");
    }
}