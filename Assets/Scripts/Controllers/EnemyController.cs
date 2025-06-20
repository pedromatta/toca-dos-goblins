using UnityEngine;
using Assets.Scripts.Entities;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Controllers
{
    public class EnemyController : MonoBehaviour
    {
        private Inimigo enemyData;

        public Inimigo EnemyData => enemyData;
        public GameObject interactIcon;
        private Animator interactAnim => interactIcon.GetComponent<Animator>();

        public void Setup(Inimigo enemy)
        {
            enemyData = enemy;
        }
        public void EngageCombat()
        {
            Debug.Log($"Engaging combat with {enemyData.Nome}");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

            interactAnim.Play("EnemySpeech");

            if (collision.CompareTag("Player"))
            {
                PlayerController.nearbyEnemy = this;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            interactAnim.Play("EnemySpeechClose");
            if (collision.CompareTag("Player"))
            {
                PlayerController.nearbyEnemy = null;
            }
        }
    }
}