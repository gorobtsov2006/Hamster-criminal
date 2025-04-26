using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CellProperty : MonoBehaviour
{
    ElementTypes element;
    bool isPushable;
    bool destroysObject;
    bool isWin;
    bool isPlayer;
    bool isStop;
    bool isDangerous;
    bool isKill; 
    int currentRow, currentCol;
    Vector2 lastDirection = Vector2.right;

    
    private float moveDelay = 0.2f; 
    private float lastMoveTime; 
    private bool isMovingLastFrame = false; 

    
    private struct MoveState
    {
        public List<(GameObject obj, int row, int col)> positions; 
        public Vector2 direction; 
    }
    private Stack<MoveState> moveHistory = new Stack<MoveState>(); 
    private bool isUndoing = false;

    
    private List<GameObject> dangerIndicators = new List<GameObject>(); 
    public GameObject dangerZonePrefab; 
    public ElementTypes Element
    {
        get { return element; }
    }
    public bool IsStop
    {
        get { return isStop; }
    }
    public bool IsPushable
    {
        get { return isPushable; }
    }
    public bool IsKill
    {
        get { return isKill; }
    }
    public int CurrentRow
    {
        get { return currentRow; }
        set { currentRow = value; }
    }

    public int CurrentCol
    {
        get { return currentCol; }
        set { currentCol = value; }
    }

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (dangerZonePrefab == null)
        {
            Debug.LogError("DangerZonePrefab не назначен в CellProperty!");
        }
        ResetState();
    }



    public void AssignInfo(int r, int c, ElementTypes e)
    {
        currentRow = r;
        currentCol = c;
        element = e;
        ChangeSprite();
        if (e == ElementTypes.Wall)
        {
            isStop = true;
        }
        if (e == ElementTypes.Hamster)
        {
            isPlayer = true;
            spriteRenderer.sortingOrder = 100;
        }
        if (e == ElementTypes.Rat)
        {
            isDangerous = true;
            isKill = false; 
        }
    }

    public void Initialize()
    {
        isPushable = false;
        destroysObject = false;
        isWin = false;
        isPlayer = false;
        isStop = false;
        isDangerous = false;
        isKill = false; 

        if ((int)element >= 99)
        {
            isPushable = true;
        }
        if (element == ElementTypes.Rat)
        {
            isDangerous = true;
        }
        ClearDangerIndicators(); 
    }




    private void ResetState()
    {
        moveHistory.Clear();
        lastMoveTime = 0f;
        isMovingLastFrame = false;
        isUndoing = false;
        lastDirection = Vector2.right;
        ClearDangerIndicators();
    }


    public void ChangeSprite()
    {
        if (element == ElementTypes.Hamster)
            spriteRenderer.sprite = GridMaker.instance.ReturnSpriteOf(element, lastDirection);
        else
            spriteRenderer.sprite = GridMaker.instance.ReturnSpriteOf(element, Vector2.zero);

        if (isPlayer || isPushable)
        {
            spriteRenderer.sortingOrder = 100;
        }
        else
        {
            spriteRenderer.sortingOrder = 10;
        }
    }

    public void ChangeObject(CellProperty c)
    {
        element = c.element;
        isPushable = c.isPushable;
        destroysObject = c.destroysObject;
        isWin = c.isWin;
        isPlayer = c.isPlayer;
        isStop = c.IsStop;
        isDangerous = c.isDangerous;
        isKill = c.isKill;
        ChangeSprite();
        UpdateDangerIndicators(); 
    }

    public void IsPlayer(bool isP)
    {
        isPlayer = isP;
    }

    public void IsItStop(bool isS)
    {
        isStop = isS;
    }

    public void IsItWin(bool isW)
    {
        isWin = isW;
    }
    public void IsItPushable(bool isPush)
    {
        isPushable = isPush;
    }
    public void IsItDestroy(bool isD)
    {
        destroysObject = isD;
    }
    public void IsItDangerous(bool isDang)
    {
        isDangerous = isDang;
    }
    public void IsItKill(bool isK)
    {
        Debug.Log($"IsItKill вызван для {element} на позиции ({currentRow}, {currentCol}): isKill = {isK}");
        isKill = isK;
        UpdateDangerIndicators();
    }

    void Update()
    {
        CheckDestroy();
        if (isPlayer && !isUndoing)
        {
            CheckDanger();

            
            bool isMoving = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) ||
                            Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) ||
                            Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                            Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

            if (isMoving && Time.time - lastMoveTime >= moveDelay)
            {
                if (!isMovingLastFrame)
                {
                    SaveState();
                }

                Vector2 direction = Vector2.zero;
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) direction = Vector2.right;
                else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) direction = Vector2.left;
                else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) direction = Vector2.up;
                else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) direction = Vector2.down;

                if (CanMove(direction))
                {
                    Move(direction);
                    lastMoveTime = Time.time;
                }
            }
            else
            {
                isMovingLastFrame = false;
            }

            if (Input.GetKeyDown(KeyCode.Space) && moveHistory.Count > 0)
            {
                isUndoing = true;
                UndoMove();
                isUndoing = false;
            }

            isMovingLastFrame = isMoving;
        }
    }

    private bool CanMove(Vector2 direction)
    {
        int newRow = currentRow + (int)direction.y;
        int newCol = currentCol + (int)direction.x;
        return newRow >= 0 && newRow < GridMaker.instance.Rows &&
               newCol >= 0 && newCol < GridMaker.instance.Cols &&
               !GridMaker.instance.IsStop(newRow, newCol, direction);
    }

    private void Move(Vector2 direction)
    {
        lastDirection = direction;
        ChangeSprite();

        List<GameObject> movingObjects = new List<GameObject> { this.gameObject };
        int stepRow = (int)direction.y;
        int stepCol = (int)direction.x;
        int nextRow = currentRow + stepRow;
        int nextCol = currentCol + stepCol;

        while (GridMaker.instance.IsTherePushableObjectAt(nextRow, nextCol))
        {
            movingObjects.Add(GridMaker.instance.GetPushableObjectAt(nextRow, nextCol));
            nextRow += stepRow;
            nextCol += stepCol;
            if (nextRow < 0 || nextRow >= GridMaker.instance.Rows || nextCol < 0 || nextCol >= GridMaker.instance.Cols) break;
        }

        foreach (GameObject obj in movingObjects)
        {
            CellProperty cp = obj.GetComponent<CellProperty>();
            cp.currentRow += stepRow;
            cp.currentCol += stepCol;
            obj.transform.position += new Vector3(stepCol, stepRow, 0);
        }

        GridMaker.instance.CompileRules();
        CheckWin();
    }

    private void SaveState()
    {
        MoveState state = new MoveState
        {
            positions = new List<(GameObject, int, int)>(),
            direction = lastDirection
        };

        state.positions.Add((this.gameObject, currentRow, currentCol));

        foreach (GameObject obj in GridMaker.instance.cells)
        {
            if (obj != null && obj.GetComponent<CellProperty>().IsPushable)
            {
                CellProperty cp = obj.GetComponent<CellProperty>();
                state.positions.Add((obj, cp.currentRow, cp.currentCol));
            }
        }

        moveHistory.Push(state);
        Debug.Log($"Состояние сохранено: {moveHistory.Count}");
    }

    private void UndoMove()
    {
        MoveState lastState = moveHistory.Pop();
        Debug.Log($"Отмена движения, осталось в истории: {moveHistory.Count}");

        foreach (var (obj, row, col) in lastState.positions)
        {
            CellProperty cp = obj.GetComponent<CellProperty>();
            cp.currentRow = row;
            cp.currentCol = col;
            obj.transform.position = new Vector3(col, row, obj.transform.position.z);
        }

        lastDirection = lastState.direction;
        ChangeSprite();

        GridMaker.instance.CompileRules();
    }

    public void CheckWin()
    {
        List<GameObject> objectsAtPlayerPosition = GridMaker.instance.FindObjectsAt(currentRow, currentCol);
        foreach (GameObject g in objectsAtPlayerPosition)
        {
            if (g.GetComponent<CellProperty>().isWin)
            {
                Debug.Log("Player Won!");
                PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
                GridMaker.instance.NextLevel();
            }
        }
    }

    public void CheckDestroy()
    {
        List<GameObject> objectsAtPosition = GridMaker.instance.FindObjectsAt(currentRow, currentCol);
        bool destroys = false;
        bool normalObject = false;
        foreach (GameObject g in objectsAtPosition)
        {
            if (!g.GetComponent<CellProperty>().destroysObject)
                normalObject = true;
            if (g.GetComponent<CellProperty>().destroysObject)
                destroys = true;
        }

        if (destroys && normalObject)
        {
            foreach (GameObject g in objectsAtPosition)
                Destroy(g);
        }
    }

    public void CheckDanger()
    {
        List<Vector2> directions = new List<Vector2> { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        foreach (Vector2 dir in directions)
        {
            int checkRow = currentRow + (int)dir.y;
            int checkCol = currentCol + (int)dir.x;
            List<GameObject> objectsAtPosition = GridMaker.instance.FindObjectsAt(checkRow, checkCol);
            foreach (GameObject g in objectsAtPosition)
            {
                if (g.GetComponent<CellProperty>().IsKill) // Проверяем isKill вместо isDangerous
                {
                    Debug.Log("Player Died by Kill!");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    return;
                }
            }
        }
    }

    private void UpdateDangerIndicators()
    {
        Debug.Log($"UpdateDangerIndicators вызван для {element} на позиции ({currentRow}, {currentCol})");
        ClearDangerIndicators();

        if (isKill && element == ElementTypes.Rat)
        {
            Debug.Log($"Создаю индикаторы для крысы на позиции ({currentRow}, {currentCol})");
            List<Vector2> directions = new List<Vector2> { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
            foreach (Vector2 dir in directions)
            {
                int checkRow = currentRow + (int)dir.y;
                int checkCol = currentCol + (int)dir.x;
                if (checkRow >= 0 && checkRow < GridMaker.instance.Rows && checkCol >= 0 && checkCol < GridMaker.instance.Cols)
                {
                    Debug.Log($"Создаю индикатор на позиции ({checkRow}, {checkCol})");
                    GameObject indicator = Instantiate(dangerZonePrefab, new Vector3(checkCol, checkRow, 0), Quaternion.identity);
                    indicator.transform.SetParent(transform.parent); // Привязываем к родителю
                    dangerIndicators.Add(indicator);
                }
            }
        }
    }

    private void ClearDangerIndicators()
    {
        foreach (GameObject indicator in dangerIndicators)
        {
            if (indicator != null)
            {
                Destroy(indicator);
            }
        }
        dangerIndicators.Clear();
    }

    private void OnDestroy()
    {
        ClearDangerIndicators();
    }
}