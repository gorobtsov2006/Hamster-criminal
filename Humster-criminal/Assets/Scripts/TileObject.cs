using UnityEngine;

public abstract class TileObject : MonoBehaviour
{
    public bool isMovable = false;

    public virtual bool CanMove()
    {
        return isMovable;
    }

    public virtual void Move(Vector3 direction)
    {
        transform.position += direction;
    }
}