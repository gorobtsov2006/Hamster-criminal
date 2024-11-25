using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int initialRow, initialCol;

    private void Strat()
    {
        transform.position = new Vector3(initialCol, initialRow,0);
    }
  

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        }
    }
}
