using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
public class Player : MonoBehaviour
{

    public float gravity;
    public Vector2 velocity;
    public float direction = 0;
    public float maxVelocity;
    public float speed = 2f;
    public float acceleration;
    public float deAcceleration;
    public float maxAcceleration;
    public float jumpVelocity =20;
    public float groundHeight = 10;
    public bool isGrounded = false;
    public bool isFacingRight = true;
    public bool isDashing = false;

    private void Awake()
    {
     
    }

    private void OnEnable()
    {
        EventManager<float>.Instance.StartListening("movement", Move);
        EventManager<bool>.Instance.StartListening("jumpMovement", Jump);
        EventManager<bool>.Instance.StartListening("isDashing", IsDashing);
    }
    private void OnDisable()
    {
        EventManager<float>.Instance.StopListening("movement", Move);
        EventManager<bool>.Instance.StopListening("jumpMovement", Jump);
        EventManager<bool>.Instance.StopListening("isDashing", IsDashing);
    }

    private void IsDashing(bool _isDashing)
    {
        isDashing = _isDashing;
    }
    private void Move(float movementDirection)
    {
        direction = movementDirection;
        Flip();
    }

    private void Jump(bool isJumping)
    {
        if (isGrounded && isJumping)
        {
            isGrounded = false;
            velocity.y = jumpVelocity;
        }
    }

    public void Flip()
    {
        if(isFacingRight && direction < 0f || !isFacingRight && direction > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void Update()
    {
        if (isDashing)
        {
            return;
        }
        if (direction == 0f)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deAcceleration * Time.deltaTime);
        }
        else
        {
            velocity.x += acceleration * direction * Time.deltaTime;
            velocity.x = Math.Clamp(velocity.x, -maxVelocity, maxVelocity);
        }
        EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", velocity.x);

    }

    private void FixedUpdate()
    {
        Vector2 pos = transform.position;


        if (!isGrounded)
        {
            pos.y += velocity.y * Time.fixedDeltaTime;
            velocity.y += gravity * Time.fixedDeltaTime;

            Vector2 raycastOrigin = new Vector2((pos.x + 0.7f) * direction, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = velocity.y * Time.fixedDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(raycastOrigin, rayDirection, rayDistance);
            if (hit2D.collider != null)
            {
                LandableGround landableGround = hit2D.collider.GetComponent<LandableGround>();
                if (landableGround != null)
                {
                    groundHeight = landableGround.groundHeight;
                    pos.y = groundHeight;
                    velocity.y = 0f;
                    isGrounded = true;
                }
            }
            Debug.DrawRay(raycastOrigin, rayDirection * rayDistance, Color.cyan);
        }
        if (isGrounded)
        {
            Vector2 raycastOrigin = new Vector2((pos.x - 0.7f) * direction, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = velocity.y * Time.fixedDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(raycastOrigin, rayDirection, rayDistance);
            if (hit2D.collider == null)
            {
                isGrounded = false;
            }
            Debug.DrawRay(raycastOrigin, rayDirection * rayDistance, Color.green);
        }
        transform.position = pos;
    }
}
