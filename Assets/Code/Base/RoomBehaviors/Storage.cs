using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    public static StorageManager instance;

    public int storageCount = 0;
    public int storageIndex = 0;
    public List<ItemGrid> storages = new List<ItemGrid>();
    public GameObject buttonRight;
    public GameObject buttonLeft;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }

    public void NextStorage()
    {
        storages[storageIndex].GetComponent<RectTransform>().localPosition = new Vector2(storages[storageIndex].GetComponent<RectTransform>().localPosition.x, storages[storageIndex].GetComponent<RectTransform>().localPosition.y - 2000);
        storageIndex++;
        if (storageIndex > storageCount - 1)
            storageIndex = 0;
        storages[storageIndex].GetComponent<RectTransform>().localPosition = new Vector2(storages[storageIndex].GetComponent<RectTransform>().localPosition.x, storages[storageIndex].GetComponent<RectTransform>().localPosition.y + 2000);
    }

    public void PreviousStorage()
    {
        storages[storageIndex].GetComponent<RectTransform>().localPosition = new Vector2(storages[storageIndex].GetComponent<RectTransform>().localPosition.x, storages[storageIndex].GetComponent<RectTransform>().localPosition.y - 2000);
        storageIndex--;
        if (storageIndex < 0)
            storageIndex = storageCount - 1;
        storages[storageIndex].GetComponent<RectTransform>().localPosition = new Vector2(storages[storageIndex].GetComponent<RectTransform>().localPosition.x, storages[storageIndex].GetComponent<RectTransform>().localPosition.y + 2000);
    }
}
