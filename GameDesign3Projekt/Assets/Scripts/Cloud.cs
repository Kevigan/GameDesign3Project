using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum cloudColor
{
    red,
    blue,
    green,
    yellow,
    white,
    black
}
public class Cloud : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float addJumpForce = 0;
    [SerializeField] private bool moveVertical;
    [SerializeField] private bool moveHorizontal;

    [SerializeField] private cloudColor color;
    [SerializeField] private int jumpAmount = 1;

    [SerializeField] private Text text;

    List<CharacterController2D> charPassengers = new List<CharacterController2D>();

    private Vector2 moveDelta;
    private SpriteRenderer spriteRenderer;

    private void OnValidate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateColor();
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (moveHorizontal) moveDelta = Vector2.right;
        if (moveVertical) moveDelta = Vector2.up;
        UpdateText();
    }

    private void FixedUpdate()
    {
        Move();

        foreach (CharacterController2D chars in charPassengers)
        {
            // chars.AddForce(moveDelta.normalized * speed);
            chars.CloudVelocity = moveDelta * speed;
        }
    }
    //green,
    //yellow,
    //white,
    //black
    private void UpdateColor()
    {
        switch (color)
        {
            case cloudColor.red:
                spriteRenderer.color = new Color(1, 0, 0, 1);
                break;
            case cloudColor.blue:
                spriteRenderer.color = new Color(0, 0, 1, 1);
                break;
            case cloudColor.green:
                spriteRenderer.color = new Color(0, 1, 0, 1);
                break;
            case cloudColor.yellow:
                spriteRenderer.color = new Color(1, 0.92f, 0.016f, 1);
                break;
            case cloudColor.white:
                spriteRenderer.color = new Color(1, 1, 1, 1);
                break;
            case cloudColor.black:
                spriteRenderer.color = new Color(0, 0, 0, 1);
                break;
        }
    }

    private void UpdateText()
    {
        text.text = jumpAmount.ToString();
    }

    private void CheckJumps()
    {
        if (jumpAmount <= 0)
        {
            //BoxCollider2D[] box = GetComponents<BoxCollider2D>();
            //foreach (BoxCollider2D col in box)
            //{
            //     col.enabled = false;
            //}
            //spriteRenderer.enabled = false;
            Destroy(gameObject);
        }
    }

    private void Move()
    {
        if (transform.localPosition.x < -10) moveDelta = Vector2.right;
        if (transform.localPosition.x > 10) moveDelta = Vector2.left;
        if (transform.localPosition.y < -5) moveDelta = Vector2.up;
        if (transform.localPosition.y > 5) moveDelta = Vector2.down;
        transform.localPosition += (Vector3)moveDelta * Time.fixedDeltaTime * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterController2D>() is CharacterController2D character)
        {
            character.isOnCloud = true;

            charPassengers.Add(character);

            foreach (CharacterController2D chars in charPassengers)
            {
                chars.JumpForce += addJumpForce;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterController2D>() is CharacterController2D character && charPassengers.Contains(character))
        {
            jumpAmount--;
            UpdateText();
            CheckJumps();
            character.isOnCloud = false;
            character.CloudVelocity = Vector2.zero;
            foreach (CharacterController2D chars in charPassengers)
            {
                chars.JumpForce -= addJumpForce;
            }

            charPassengers.Remove(character);
        }
    }
}
