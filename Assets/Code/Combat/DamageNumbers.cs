using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumbers : MonoBehaviour
{
    [SerializeField] float timeBeforeDespawn = 2f;
    RectTransform rectTransform;
    TMPro.TextMeshPro textMeshPro;
    float timer;

    [SerializeField] float scaleUp;
    [SerializeField] float scaleUpThreshold;
    [SerializeField] float scaleDown;
    [SerializeField] float flyUpOffset;

    private void OnEnable()
    {
        timer = 0;
        rectTransform = GetComponent<RectTransform>();
        textMeshPro = GetComponent<TMPro.TextMeshPro>();
        rectTransform.localScale = Vector3.one;
        textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, 100);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, transform.position.y + flyUpOffset), Time.deltaTime);

        timer += Time.deltaTime;
        if (timer < scaleUpThreshold)
        {
            rectTransform.localScale = Vector3.Lerp(Vector3.one, new Vector3(scaleUp, scaleUp, scaleUp), timer / scaleUpThreshold);
        }
        else
        {
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, new Vector3(scaleDown, scaleDown, scaleDown), Time.deltaTime);
            textMeshPro.color = Color.Lerp(textMeshPro.color, new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, 0), Time.deltaTime * 2);
        }
        if (timer >= timeBeforeDespawn)
            gameObject.SetActive(false);
    }
}
