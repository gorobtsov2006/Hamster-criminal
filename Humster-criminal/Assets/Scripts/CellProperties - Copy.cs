using UnityEngine;

public class CellProperty : MonoBehaviour
{
    ElementTypes element;
    bool isPushnable;
    bool destroyPlayer;
    bool isWin;
    bool isPlayer;
    bool isStop;

    int currentRow, currentCol;

    public ElementTypes Element { get { return element; } }

    public bool IsStop { get { return isStop; } }

    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    public void AssignInfo(int r, int c, ElementTypes e)
    {
        currentRow = r;
        currentCol = c;
        element = e;

        if (e == ElementTypes.Wall)
        {
            isStop = true;
        }

        if (e == ElementTypes.Hamster)
        {
            isPlayer = true;
        }
    }

    private void Update()
    {
        /*if (isPlayer)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) && currentCol + 1 < GridMaker.instance.Cols && !GridMaker.instance.cells[currentRow][currentCol + 1].GetComponent<CellProperty>().isStop)
            {
                transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
                currentCol++;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) && currentCol - 1 >= 0 && !GridMaker.instance.Cols && !GridMaker.instance.cells[currentRow][currentCol - 1].GetComponent<CellProperty>().isStop)
            {
                transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
                currentCol--;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow)&& currentRow +1 < GridMaker.instance.Rows && !GridMaker.instance.cells[currentRow + 1][currentRow].GetComponent<CellProperty>().isStop)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                currentRow++;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && currentRow - 1 >= 0 && !GridMaker.instance.cells[currentRow - 1][currentCol].GetComponent<CellProperty>().isStop)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
                currentRow--;
            }

        }*/

    }
}
