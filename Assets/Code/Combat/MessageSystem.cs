using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageSystem : MonoBehaviour
{
    [SerializeField] GameObject damageNumber;
    public static MessageSystem instance;

    [SerializeField] List<TMPro.TextMeshPro> messagePool = new List<TMPro.TextMeshPro>();
    [SerializeField] int objectCount;

    int index = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < objectCount; i++)
        {
            Populate();
        }
    }

    void Populate()
    {
        GameObject txt = Instantiate(damageNumber, transform);
        messagePool.Add(txt.GetComponent<TMPro.TextMeshPro>());
        txt.SetActive(false);
    }

    public void WriteMessage(string text, Vector3 worldPosition, int color)
    {
        messagePool[index].gameObject.SetActive(true);
        messagePool[index].transform.position = worldPosition;
        messagePool[index].text = text;
        if (color == 0)
            messagePool[index].color = Color.white;
        else if (color == 1)
            messagePool[index].color = Color.yellow;
        else if (color == 2)
            messagePool[index].color = Color.red;

        index++;

        if (index >= objectCount)
            index = 0;

        /*
        GameObject txt = Instantiate(damageNumber, transform);
        txt.transform.position = worldPosition;
        txt.GetComponent<TMPro.TextMeshPro>().text = text;
        */
    }
}
