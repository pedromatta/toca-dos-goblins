using Assets.Scripts.Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        public float speed = 8f;
        public int facingDirection = 1;

        public Rigidbody2D rb;
        public Animator anim;

        private Vector2 moveInput;
        public static bool IsMovementBlocked = false;

        public static EnemyController nearbyEnemy;
        public static NPCController nearbyNpc;
        public static AreaChangeController nearbyAreaChanger;

        private PlayerMovement inputActions;

        public GameObject interactIcon;
        private Animator interactAnim => interactIcon.GetComponent<Animator>();

        private void Awake()
        {
            inputActions = new PlayerMovement();
        }

        private void OnEnable()
        {
            inputActions.Enable();
            inputActions.Player.Interact.performed += OnInteract;
        }

        private void OnDisable()
        {
            inputActions.Player.Interact.performed -= OnInteract;
            inputActions.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            Debug.Log("Interact action performed");
            if (nearbyEnemy != null && nearbyNpc == null)
            {
                Debug.Log("Interacting with enemy: " + nearbyEnemy.EnemyData.Nome);
                var playerPosition = transform.position;
                GameManager.Instance.StartCombat(nearbyEnemy.EnemyData, playerPosition);
            }
            else if (nearbyNpc != null && nearbyEnemy == null)
            {
                Debug.Log("Interacting with NPC: " + nearbyNpc.NpcData.Nome);
                nearbyNpc.Interact();
            }
            else if (nearbyAreaChanger != null && nearbyEnemy == null && nearbyNpc == null)
            {
                Debug.Log("Interacting with area changer.");

                if (nearbyAreaChanger.TeleporterNext != null)
                {
                    Debug.Log("Changing to next area.");
                    nearbyAreaChanger.NextArea();
                }
                else if (nearbyAreaChanger.TeleporterPrevious != null)
                {
                    Debug.Log("Changing to previous area.");
                    nearbyAreaChanger.PreviousArea();
                }
            }
            else
            {
                Debug.Log("No valid interaction target found.");
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var areaChanger = collision.GetComponent<AreaChangeController>();
            if (areaChanger != null)
            {
                nearbyAreaChanger = areaChanger;
                if (interactAnim != null)
                {
                    interactAnim.Play("NpcSpeech");
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var areaChanger = collision.GetComponent<AreaChangeController>();
            if (areaChanger != null && nearbyAreaChanger == areaChanger)
            {
                nearbyAreaChanger = null;
                if (interactAnim != null)
                {
                    interactAnim.Play("NpcSpeechClose");
                }
            }
        }

        void FixedUpdate()
        {
            if (IsMovementBlocked)
            {
                anim.SetFloat("move", 0);
                rb.linearVelocity = Vector2.zero;
                return;
            }

            if (moveInput.x > 0 && transform.localScale.x < 0 || moveInput.x < 0 && transform.localScale.x > 0)
            {
                facingDirection *= -1;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1;
                transform.localScale = localScale;
            }
            anim.SetFloat("move", moveInput.magnitude);
            rb.linearVelocity = moveInput * speed;
        }
    }
}