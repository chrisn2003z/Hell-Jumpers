
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // movement modifier values
    private float playerSpeed = 3.0f;
    private float jump = 2.0f;          // 3 1/2 tiles max double jump height
    private float gravity = -9.81f;

    // player variables
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool playerGrounded;

    private float damageCooldown = 1.5f; // seconds
    private float lastDamageTime = -10f;

    
    [Header("Health Settings")]
    public int maxHealth = 5;
    private int currentHealth;

    [Header("Input Actions")]
    public InputActionReference moveAction; // vector 2
    public InputActionReference jumpAction; // spacebar

    [Header("Jump Settings")]
    public int maxJumps = 2; // double jumping
    private int jumpCount = 0;

    [Header("Sprite Settings")]
    public Transform spriteTransform; // player skin goes here
    private bool facingRight = true; 

    void Awake() {
        controller = GetComponent<CharacterController>();
        currentHealth = maxHealth; // set initial player health
    }

    public void PlayerDamage(int damage = 1)
    {
        if (Time.time - lastDamageTime < damageCooldown) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        lastDamageTime = Time.time;

        if (currentHealth <= 0) PlayerDeath();
    }

    private void PlayerDeath() // player death function
    {
        // add death animation
        // reload scene/game over menu
        // disable movement

        gameObject.SetActive(false);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Trap"))
        {
            PlayerDamage();
        }
    }

    private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("EnemyProjectile"))
    {
        PlayerDamage();
        Destroy(other.gameObject); // remove projectile
    }
}

    private void OnDisable() {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    private void OnEnable() {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    private void Flip() // flip sprite function
    {
        facingRight = !facingRight;

        Vector3 scale = spriteTransform.localScale;
        scale.x *= -1; // flip
        spriteTransform.localScale = scale;
    }   


    // Update is called once per frame
    void Update() {

        // check if player is on ground
        playerGrounded = controller.isGrounded;
        if (playerGrounded && playerVelocity.y < 0) {
            playerVelocity.y = -2f;
            jumpCount = 0;
        }

        // player input
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, 0/*input.y*/);
        move = Vector3.ClampMagnitude(move, 1f);

        if (move != Vector3.zero) {
            transform.forward = move;
        }

        if (jumpAction.action.triggered && jumpCount < maxJumps) {
            playerVelocity.y = Mathf.Sqrt(jump * -2.0f * gravity);
            jumpCount++;
        }

        playerVelocity.y += gravity * Time.deltaTime;

        // actual player movement
        Vector3 finalMove = (move * playerSpeed) + (playerVelocity.y * Vector3.up);
        controller.Move(finalMove * Time.deltaTime); 

        /*    //horizontal movement
        Vector3 horizontalMove = move * playerSpeed;
        controller.Move(horizontalMove * Time.deltaTime);

        //vertical movement
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity.y * Vector3.up * Time.deltaTime);    */

        transform.rotation = Quaternion.identity; // always facing forward?

        //lock z position
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;

        if (move.x > 0 && !facingRight) // flip sprite when moving left/right
        {
            Flip();
        }
        else if (move.x < 0 && facingRight)
        {
            Flip();
        }

    }
} 



