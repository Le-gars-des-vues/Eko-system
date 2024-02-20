using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureHealth : MonoBehaviour
{
    [SerializeField] float maxHp;
    [SerializeField] float currentHp;

    // Start is called before the first frame update
    void Start()
    {
        currentHp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHp <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void LoseHealth(float value)
    {
        currentHp -= value;
    }
}
