using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Demo_CharacterController : MonoBehaviour
    {
        private Rigidbody2D myRigidbody;
        private Collider2D myCollider;

        private int score = 0;

        private float MovementSpeed = 8f;
        private float RotationSpeed = 480f;
        private float JumpHeight = 10f;
        private bool isGrounded = false;

        private float maxFallingSpeed = 30f;

        public GameObject BombPrefab;
        private float bombDropDelay = 1;
        private float previousBombDrop = -100;

        // Start is called before the first frame update
        void Start()
        {
            myRigidbody = GetComponent<Rigidbody2D>();
            myCollider = GetComponent<Collider2D>();
        }

        private void FixedUpdate()
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, Mathf.Clamp(myRigidbody.velocity.y, -maxFallingSpeed, maxFallingSpeed));

            if (myRigidbody.velocity.y <= 0)
            {
                if (Physics2D.Raycast(transform.position - new Vector3(myCollider.bounds.extents.x, myCollider.bounds.extents.y, 0), new Vector2(0, -1), 0.05f, 1 << 8) || Physics2D.Raycast(transform.position - new Vector3(-myCollider.bounds.extents.x, myCollider.bounds.extents.y, 0), new Vector2(0, -1), 0.05f, 1 << 8))
                {
                    isGrounded = true;
                }
                else
                {
                    isGrounded = false;
                }
            }
        }

        private void HandleMovement(Vector2 direction)
        {
            myRigidbody.velocity = new Vector2((Vector2.ClampMagnitude(direction, 1.0f) * MovementSpeed).x, myRigidbody.velocity.y);

            myRigidbody.rotation -= Mathf.Clamp(direction.x, -1, 1) * RotationSpeed * Time.deltaTime;
        }

        private void Jump()
        {
            if (isGrounded)
            {
                myRigidbody.AddForce(new Vector2(0, JumpHeight), ForceMode2D.Impulse);
                isGrounded = false;
            }
        }

        private void DropBomb()
        {
            if (Time.time - previousBombDrop >= bombDropDelay)
            {
                Instantiate(BombPrefab, transform.position, Quaternion.identity);

                previousBombDrop = Time.time;
            }
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 direction = new Vector2(0,0);

            if (Input.GetKey(KeyCode.A))
            {
                direction.x -= 1;
            }

            if (Input.GetKey(KeyCode.D))
            {
                direction.x += 1;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }

            if (direction.magnitude > 0)
            {
                HandleMovement(direction);
            }
            else
            {
                myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
                myRigidbody.angularVelocity = 0;
            }

            if (Input.GetMouseButtonDown(0))
            {
                DropBomb();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<Treasure_Tile>() != null)
            {
                score += collision.GetComponent<Treasure_Tile>().score;

                Destroy(collision.gameObject);
            }

            EventManager.GetInstance().RaiseEvent(new Demo_EarnedScoreEvent().Init(score));
        }
    }
}