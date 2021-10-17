using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float addJumpForce = 0;
    [SerializeField] private bool moveVertical;
    [SerializeField] private bool moveHorizontal;

    List<Transform> passengers = new List<Transform>();
    List<CharacterController2D> charPassengers = new List<CharacterController2D>();

    private Vector2 moveDelta;


    private void Start()
    {
       if(moveHorizontal) moveDelta = Vector2.right;
       if(moveVertical) moveDelta = Vector2.up;
    }

    private void FixedUpdate()
    {
        Move();
        //Debug.Log(moveDelta);

        foreach (CharacterController2D chars in charPassengers)
        {
            // chars.AddForce(moveDelta.normalized * speed);
            chars.CloudVelocity = moveDelta * speed;
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
