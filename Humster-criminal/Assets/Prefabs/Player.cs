using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int initialRow, initialCol;
    public float moveDelay = 0.2f;

    private Stack<Vector3> positionHistory = new Stack<Vector3>();
    private bool isMoving = false;
    private bool isReturning = false;

    private void Start()
    {
        transform.position = new Vector3(initialCol, initialRow, 0);
        positionHistory.Push(transform.position);
    }

    private void Update()
    {
        if (!isMoving && !isReturning)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                StartCoroutine(Move(Vector3.right));
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                StartCoroutine(Move(Vector3.left));
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                StartCoroutine(Move(Vector3.up));
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                StartCoroutine(Move(Vector3.down));
            }
        }

        if (Input.GetKey(KeyCode.Z))
        {
            if (!isReturning)
            {
                StartCoroutine(ReturnToPreviousPosition());
            }
        }
    }

    private IEnumerator Move(Vector3 direction)
    {
        isMoving = true;

        while (Input.GetKey(KeyCode.RightArrow) && direction == Vector3.right ||
               Input.GetKey(KeyCode.LeftArrow) && direction == Vector3.left ||
               Input.GetKey(KeyCode.UpArrow) && direction == Vector3.up ||
               Input.GetKey(KeyCode.DownArrow) && direction == Vector3.down)
        {
            Vector3 newPosition = transform.position + direction;
            positionHistory.Push(newPosition);
            transform.position = newPosition;
            yield return new WaitForSeconds(moveDelay);
        }

        isMoving = false;
    }

    private IEnumerator ReturnToPreviousPosition()
    {
        isReturning = true;

        while (Input.GetKey(KeyCode.Z) && positionHistory.Count > 1)
        {
            positionHistory.Pop();
            transform.position = positionHistory.Peek();
            yield return new WaitForSeconds(moveDelay);
        }

        isReturning = false;
    }
}