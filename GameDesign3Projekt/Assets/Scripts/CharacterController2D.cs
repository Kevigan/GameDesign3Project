using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float jumpForce = 5;
    public float JumpForce { get => jumpForce; set { jumpForce = value; } }

    [SerializeField] private int speed = 5;
    [SerializeField] private float gravityMultipier = 5f;

    [SerializeField] private int groundRays = 5;
    [SerializeField] private bool debugRays = false;
    [SerializeField] private float sideRayLength = .02f;
    [SerializeField] private float bottomRayLength = .01f;

    [SerializeField] private LayerMask wallJumpLayer;
    [SerializeField] private LayerMask ceilingLayer;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private GameObject inventoryToggle;
    [SerializeField] private GameObject equipmentPanelToggle;
    [SerializeField] private GameObject statsTooltipToggle;
    [SerializeField] private GameObject itemsTooltipToggle;

    [SerializeField] private Inventory inventory;
    [SerializeField] private InventoryManager inventoryManager;

    private Vector2 cloudVelocity;
    public Vector2 CloudVelocity { get => cloudVelocity; set { cloudVelocity = value; } }
    public bool isOnCloud;
    private IInteractable currentInteractable;
    private Rigidbody2D rigid;
    protected BoxCollider2D col;

    public bool YVelocityIsActive { get; private set; } = true;
    private Vector2 moveInput = Vector2.zero;
    private Vector2 velocity = Vector2.zero;
    public Vector2 Velocity
    {
        get => new Vector2(xForceSet ? setForce.x : velocity.x, yForceSet ? setForce.y : velocity.y);
        set => SetForce(value);
    }
    //Ist nur dann nicht == Vector2.zero, wenn SetYForce in diesem Frame gecalled wurde
    private bool yForceSet, xForceSet = false;
    private Vector2 setForce = new Vector2(0, 0);
    private Vector2 addForce = new Vector2(0, 0);

    public CharacterCollision collision;


    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        HideMouseCursor();
        inventoryToggle.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        HandleCollision();
    }

    private void FixedUpdate()
    {
        CalculateXVelocity(moveInput.x);
        CalculateYVelocity();
        ApplyVelocity();
    }

    private void ApplyVelocity()
    {
        velocity += addForce;
        addForce = Vector2.zero;

        velocity.x = xForceSet ? setForce.x : velocity.x;
        velocity.y = yForceSet ? setForce.y : velocity.y;

        xForceSet = yForceSet = false;

        velocity.y = YVelocityIsActive ? velocity.y : 0f;
        rigid.MovePosition(rigid.position + (velocity * Time.fixedDeltaTime) + (cloudVelocity * Time.fixedDeltaTime));
    }
    public void AddForce(Vector2 value)
    {
        addForce += value;
        
    }
    public void SetForce(Vector2 value) //F?r Walljump
    {
        setForce = value;
        yForceSet = xForceSet = true;
    }
    public void SetXForce(float newXForce)
    {
        setForce.x = newXForce;
        xForceSet = true;
    }
    public void SetYForce(float newYForce)
    {
        setForce.y = newYForce;
        yForceSet = true;
    }
    public void CalculateXVelocity(float currentInput)
    {
        velocity.x = currentInput * speed;
    }

    public void CalculateYVelocity()
    {
        if (!collision.Grounded)
        {
            velocity.y -= 9.81f * Time.fixedDeltaTime * gravityMultipier;
            if (velocity.y < -15f) velocity.y = -15f;
        }
        else if (velocity.y < 0 || !isOnCloud)
        {
            velocity.y = 0;
        }
    }
    
    private void HandleCollision()
    {
        CheckGrounded();
        CheckWalls();
        //CheckHeightToDash();
    }
    private void CheckGrounded()
    {
        Vector2 bottomLeft = col.bounds.center + new Vector3(-col.bounds.extents.x, -col.bounds.extents.y, 0f);
        Vector2 bottomRight = col.bounds.center + new Vector3(col.bounds.extents.x, -col.bounds.extents.y, 0f);

        collision.Grounded = CheckForCollision(bottomLeft, bottomRight, Vector2.down, bottomRayLength, groundLayer);
    }
    private void CheckWalls()
    {
        Vector2 bottomLeft = col.bounds.center + new Vector3(-col.bounds.extents.x, -col.bounds.extents.y, 0f);//left collision
        Vector2 topLeft = col.bounds.center + new Vector3(-col.bounds.extents.x, col.bounds.extents.y, 0f);

        Vector2 topRight = col.bounds.center + new Vector3(col.bounds.extents.x, col.bounds.extents.y, 0f);//right collision
        Vector2 bottomRight = col.bounds.center + new Vector3(col.bounds.extents.x, -col.bounds.extents.y, 0f);

        collision.rightCollision = CheckForCollision(topRight, bottomRight, Vector2.right, sideRayLength, wallJumpLayer);
        collision.leftCollision = CheckForCollision(bottomLeft, topLeft, Vector2.left, sideRayLength, wallJumpLayer);
        //collision.topCollision = CheckForCollision(topLeft, topRight, Vector2.up, sideRayLength);
        //collision.dashCollisionLeft = CheckForCollision(bottomLeft, topLeft, Vector2.left, .1f, dashCollisionLayer);
        //collision.dashCollisionRight = CheckForCollision(topRight, bottomRight, Vector2.right, .1f, dashCollisionLayer);
        collision.ceilingCollision = CheckForCollision(topLeft, topRight, Vector2.up, 0.06f, ceilingLayer);

        //collision.canWallHang = false;

        //if (collision.rightCollision) collision.canWallHang = Physics2D.Raycast(topRight, Vector2.right, sideRayLength, wallJumpLayer)
        //                                                        && Physics2D.Raycast(bottomRight, Vector2.right, sideRayLength, wallJumpLayer);
        //if (collision.leftCollision) collision.canWallHang = Physics2D.Raycast(topLeft, Vector2.left, sideRayLength, wallJumpLayer)
        //                                                        && Physics2D.Raycast(bottomLeft, Vector2.left, sideRayLength, wallJumpLayer);
    }
    private bool CheckForCollision(Vector2 point1, Vector2 point2, Vector2 direction, float rayLength, LayerMask mask)
    {
        bool hasHit = false;

        for (int i = 0; i < groundRays; i++)
        {
            if (hasHit) break;

            Vector2 origin = point1 + i * ((point2 - point1) / (groundRays - 1));
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, rayLength, mask);

            if (debugRays)
            {
                Debug.DrawRay(new Vector3(origin.x, origin.y, 0f), direction * rayLength, Color.red, .1f);
            }
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider == col || hit.collider.isTrigger) continue;

                hasHit = true;
            }
        }
        return hasHit;
    }

    public void ShowMouseCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void HideMouseCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    #region Input
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    public void JumpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            yForceSet = xForceSet = false;
            SetYForce(inventoryManager.Strength.Value);
            Debug.Log(inventoryManager.Strength.Value);
        }
    }
    public void OpenCloseInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            inventoryToggle.SetActive(!inventoryToggle.activeSelf);
            statsTooltipToggle.SetActive(false);
            itemsTooltipToggle.SetActive(false);
            if (inventoryToggle.activeSelf)
                ShowMouseCursor();
            else
            {
                HideMouseCursor();
            }
        }
    }

    public void ToggleEquipmentPanel(InputAction.CallbackContext context)
    {
        if (context.performed && inventoryToggle.activeSelf)
        {
            equipmentPanelToggle.SetActive(!equipmentPanelToggle.activeSelf);
        }
    }
    public void ToogleEquipmentButton()
    {
        if (inventoryToggle.activeSelf)
        {
            equipmentPanelToggle.SetActive(!equipmentPanelToggle.activeSelf);
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentInteractable is GroundItem groundItem)
            {
                groundItem.AddItemToInventory(inventory);
            }
        }

    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IInteractable>() is GroundItem groundItem)
        {
            currentInteractable = groundItem;
            //groundItem.AddItemToInventory(inventory);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        currentInteractable = null;
    }
}

public struct CharacterCollision
{
    public delegate void ColEvent();
    public ColEvent OnLandedEvent;
    public ColEvent OnGroundLeftEvent;

    public bool Grounded
    {
        get => grounded; set
        {
            if (grounded != value)
            {
                grounded = value;
                if (grounded)
                {
                    OnLandedEvent?.Invoke();
                }
                else
                {
                    OnGroundLeftEvent?.Invoke();
                }
            }
        }
    }
    private bool grounded;
    //public bool dashHeightCollision;
    //public bool dashCollisionLeft;
    //public bool dashCollisionRight;
    public bool rightCollision;
    public bool leftCollision;
    public bool topCollision;
    //public GameObject topCollisionObject;
    public bool ceilingCollision;
    //public bool canWallHang;
}