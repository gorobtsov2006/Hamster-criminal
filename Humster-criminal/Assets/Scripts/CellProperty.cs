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
    int currentRow, currentCol;
    Vector2 lastDirection = Vector2.right;

    // Для движения при зажатии клавиши
    private float moveDelay = 0.2f; // Задержка между шагами (в секундах)
    private float lastMoveTime; // Время последнего шага
    private bool isMovingLastFrame = false; // Флаг: двигался ли игрок в прошлом кадре

    // Для отмены действия
    private struct MoveState
    {
        public List<(GameObject obj, int row, int col)> positions; // Позиции всех объектов перед движением
        public Vector2 direction; // Направление хомяка перед движением
    }
    private Stack<MoveState> moveHistory = new Stack<MoveState>(); // История движений
    private bool isUndoing = false; // Флаг для блокировки движения во время отмены

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
    public int CurrentRow
    {
        get { return currentRow; }
    }
    public int CurrentCol
    {
        get { return currentCol; }
    }

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        if ((int)element >= 99)
        {
            isPushable = true;
        }
        if (element == ElementTypes.Rat)
        {
            isDangerous = true;
        }
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
        ChangeSprite();
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

    void Update()
    {
        CheckDestroy();
        if (isPlayer && !isUndoing)
        {
            CheckDanger();

            // Проверяем, нажата ли клавиша движения
            bool isMoving = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) ||
                            Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow);


            if (isMoving && Time.time - lastMoveTime >= moveDelay)
            {
                // Сохраняем состояние только при первом шаге движения
                if (!isMovingLastFrame)
                {
                    SaveState();
                }

                // Определяем направление движения
                Vector2 direction = Vector2.zero;
                if (Input.GetKey(KeyCode.RightArrow)) direction = Vector2.right;
                else if (Input.GetKey(KeyCode.LeftArrow)) direction = Vector2.left;
                else if (Input.GetKey(KeyCode.UpArrow)) direction = Vector2.up;
                else if (Input.GetKey(KeyCode.DownArrow)) direction = Vector2.down;

                if (CanMove(direction))
                {
                    Move(direction);
                    lastMoveTime = Time.time;
                }
            }
            else
            {
                // Сбрасываем флаг, когда игрок отпускает клавишу
                isMovingLastFrame = false;
            }

            // Отмена действия на клавишу Space (пробел)
            if (Input.GetKeyDown(KeyCode.Space) && moveHistory.Count > 0)
            {
                isUndoing = true;
                UndoMove();
                isUndoing = false;
            }

            // Обновляем флаг движения для следующего кадра
            isMovingLastFrame = isMoving;
        }
    }

    // Проверка, можно ли двигаться в направлении
    private bool CanMove(Vector2 direction)
    {
        int newRow = currentRow + (int)direction.y;
        int newCol = currentCol + (int)direction.x;
        return newRow >= 0 && newRow < GridMaker.instance.Rows &&
               newCol >= 0 && newCol < GridMaker.instance.Cols &&
               !GridMaker.instance.IsStop(newRow, newCol, direction);
    }

    // Движение игрока и толкаемых объектов
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

    // Сохранение состояния
    private void SaveState()
    {
        MoveState state = new MoveState
        {
            positions = new List<(GameObject, int, int)>(),
            direction = lastDirection
        };

        // Сохраняем позицию игрока
        state.positions.Add((this.gameObject, currentRow, currentCol));

        // Сохраняем позиции всех толкаемых объектов
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

    // Отмена движения
    private void UndoMove()
    {
        MoveState lastState = moveHistory.Pop();
        Debug.Log($"Отмена движения, осталось в истории: {moveHistory.Count}");


        // Восстанавливаем позиции объектов
        foreach (var (obj, row, col) in lastState.positions)
        {
            CellProperty cp = obj.GetComponent<CellProperty>();
            cp.currentRow = row;
            cp.currentCol = col;
            obj.transform.position = new Vector3(col, row, obj.transform.position.z);
        }

        // Восстанавливаем направление хомяка
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
                if (g.GetComponent<CellProperty>().isDangerous)
                {
                    Debug.Log("Player Died!");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    return;
                }
            }
        }
    }
}
