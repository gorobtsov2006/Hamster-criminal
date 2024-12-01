using System.Collections.Generic;
using UnityEngine;

public class CellProperty : MonoBehaviour
{
    ElementTypes element;
    bool isPushable;
    bool destroysObject;
    bool isWin;
    bool isPlayer;
    bool isStop;

    int currentRow, currentCol;

    public ElementTypes Element { get { return element; } }

    public bool IsStop { get { return isStop; } }

    public bool IsPushable { get { return isPushable; } }

    public int CurrentRow {get { return currentRow; } }
    
    public int CurrentCol {get { return currentCol; } }

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
    }

    public void Initialize()
    {
        isPushable = false;
        destroysObject = false;
        isWin = false;
        isPlayer = false;
        isStop = false;

        if ((int)element >= 99)
        {
            isPushable = true;
        }
    }

    public void ChangeSprite()
    {
        Sprite s = GridMaker.instance.spriteLibrary.Find(x => x.element == element).sprite;

        spriteRenderer.sprite = s;

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

    private void Update()
    {
        
    }

    public void CheckWin()
    {
        
    }

    public void CheckDestroy()
    {
        
    }
}
