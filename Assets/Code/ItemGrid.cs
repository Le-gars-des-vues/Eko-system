using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    [Header("Tile Format")]
    //Faut somehow mentir au code pour qu'il marche
    const float tileSizeWidth = 32;
    const float tileSizeHeight = 32;

    [Header("Transform of Grid")]
    RectTransform rectTransform;

    [Header("Mouse and individual tile positions")]
    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();

    InventoryItem[,] inventoryItemSlot;

    [SerializeField] int gridSizeWidth = 20;
    [SerializeField] int gridSizeHeight = 15;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);
    }

    //Initialise la grille a la taille desire selon les parametres gridSizeWitdh/Height
    private void Init(int width, int height)
    {
        //Cree autant de case dans le array que de case sur la grille
        inventoryItemSlot = new InventoryItem[width, height];
        //Multiplie le nombre de case par leur taille
        Vector2 size= new Vector2(width*tileSizeWidth, height*tileSizeHeight);
        rectTransform.sizeDelta = size;
    }
    
    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        //Calcule la difference entre la position de la souris et le pivot du RectTransform (la case en haut a droite devrait etre 0,0)
        positionOnTheGrid.x = mousePosition.x-rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;

        //Transform la position en Int pour identifier la bonne case. Le second diviseur est en fonction du Scale du RectTransform, pas besoin de changer le code si on change le scale!
        tileGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth) / (int)rectTransform.localScale.x;
        tileGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight) / (int)rectTransform.localScale.y;

        return tileGridPosition;
    }
}
