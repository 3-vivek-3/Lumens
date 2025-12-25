using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 moveDirection;
    [SerializeField] private float speed;

    //[SerializeField] private PlayerControls playerControls;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
    }

    private void Start()
    {
        InputManager.instance.MoveEvent += HandleMoveEvent;
    }
    private void FixedUpdate()
    {
        Vector3 torque = new Vector3(moveDirection.y, 0f, -moveDirection.x);
        rb.AddTorque(torque * speed, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Floor")) return;
        //Debug.Log("test");
        rb.constraints |= RigidbodyConstraints.FreezePositionY;
    }

    private void HandleMoveEvent(Vector2 moveDirection)
    {
        this.moveDirection = moveDirection;
    }

    private void OnDestroy()
    {
        InputManager.instance.MoveEvent -= HandleMoveEvent;
    }
}
