using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLegTarget : MonoBehaviour
{
    float desiredYPosition;

    // Update is called once per frame
    void Update()
    {
        //Raycast qui entre en collision avec le sol
        RaycastHit2D hit = Physics2D.Raycast(new
        Vector2(transform.position.x, transform.position.y + 5),
        -Vector2.up, 12f, LayerMask.GetMask("Ground"));

        //Si on touche le sol, on met la position en Y de la prochaine cible a celle de la collision (on colle la cible suivant au sol en gros)     
        if (hit.collider != null)
        {
            desiredYPosition = hit.point.y;
        }
        else
        {
            desiredYPosition = transform.position.y;
        }

        transform.position = new Vector2(transform.position.x,
        desiredYPosition);
    }
}
