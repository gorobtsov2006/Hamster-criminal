using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int initialRow, initialCol;
    public float moveDelay = 0.12f;

    private Stack<Vector3> positionHistory = new Stack<Vector3>();
    private bool isMoving = false;
    private bool isReturning = false;

    // Ссылка на компонент Animator
    private Animator animator;

    private void Start()
    {
        transform.position = new Vector3(initialCol, initialRow);
        positionHistory.Push(transform.position);

        // Получаем ссылку на Animator
        animator = GetComponent<Animator>();
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
            else
            {
                // Останавливаем анимацию движения
                animator.SetBool("IsMoving", false);
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

        // Устанавливаем направление анимации
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
        animator.SetBool("IsMoving", true);

        while (Input.GetKey(KeyCode.RightArrow) && direction == Vector3.right ||
               Input.GetKey(KeyCode.LeftArrow) && direction == Vector3.left ||
               Input.GetKey(KeyCode.UpArrow) && direction == Vector3.up ||
               Input.GetKey(KeyCode.DownArrow) && direction == Vector3.down)
        {
            Vector3 newPosition = transform.position + direction;
            TileObject targetTile = GetTileAtPosition(newPosition);

            if (targetTile == null || targetTile.CanMove())
            {
                if (targetTile != null)
                {
                    Vector3 nextPosition = newPosition + direction;
                    TileObject nextTile = GetTileAtPosition(nextPosition);

                    if (nextTile == null || nextTile.CanMove())
                    {
                        targetTile.Move(direction);
                    }
                    else
                    {
                        isMoving = false;
                        //Остановка анимации
                        animator.SetBool("IsMoving", false);
                        yield break;
                    }
                }

                positionHistory.Push(newPosition);
                transform.position = newPosition;
                yield return new WaitForSeconds(moveDelay);
            }
            else
            {
                isMoving = false;
                //Остановка анимации
                animator.SetBool("IsMoving", false);
                yield break;
            }
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

    private TileObject GetTileAtPosition(Vector3 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);
        if (hit.collider != null)
        {
            return hit.collider.GetComponent<TileObject>();
        }
        return null;
    }
}