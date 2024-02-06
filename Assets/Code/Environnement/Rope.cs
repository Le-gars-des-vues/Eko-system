using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] private Rigidbody2D hook;
    [SerializeField] private GameObject [] prefabRopeSegs;
    [SerializeField] private int numLinks = 5;

    [SerializeField] private HingeJoint2D top;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        GenerateRope();
    }

    void GenerateRope()
    {
        Rigidbody2D prevBod = hook;

        //Pour chaque segment de corde, on en choisi un au hazard parmis les prefab
        for (int i = 0; i < numLinks; i++)
        {
            if (i != numLinks - 1)
            {
                int index = Random.Range(0, prefabRopeSegs.Length - 1);
                GameObject newSeg = Instantiate(prefabRopeSegs[index]);
                newSeg.name = i.ToString();

                //On le parent au reste de la corde
                newSeg.transform.parent = transform;
                newSeg.transform.position = new Vector2(transform.position.x, transform.position.y + ((newSeg.GetComponent<SpriteRenderer>().bounds.size.y * -1) * (i + 1)));

                //On le connecte au reste de la corde
                HingeJoint2D hj = newSeg.GetComponent<HingeJoint2D>();
                hj.connectedBody = prevBod;

                prevBod = newSeg.GetComponent<Rigidbody2D>();
                //Le premier segment est le top de la corde
                if (i == 0)
                    top = hj;
            }
            else if (i == numLinks - 1)
            {
                GameObject newSeg = Instantiate(prefabRopeSegs[prefabRopeSegs.Length - 1]);
                newSeg.name = i.ToString();

                //On le parent au reste de la corde
                newSeg.transform.parent = transform;
                newSeg.transform.position = new Vector2(transform.position.x, transform.position.y + ((newSeg.GetComponent<SpriteRenderer>().bounds.size.y * -1) * (i + 1)));

                //On le connecte au reste de la corde
                HingeJoint2D hj = newSeg.GetComponent<HingeJoint2D>();
                hj.connectedBody = prevBod;

                prevBod = newSeg.GetComponent<Rigidbody2D>();
            }
        }
    }

    public void AddLink()
    {
        //Choisi un segment de liane au hazard
        int index = Random.Range(0, prefabRopeSegs.Length);

        //Cree un gameobject avec le segment choisi
        GameObject newLink = Instantiate(prefabRopeSegs[index]);

        //Met l'objet corde comme parent du nouveau segment
        newLink.transform.parent = transform;
        newLink.transform.position = transform.position;

        //Store le hingejoint2D du nouveau segment dans une variable
        HingeJoint2D hj = newLink.GetComponent<HingeJoint2D>();

        //Connect le hj au hook
        hj.connectedBody = hook;

        //Connecte le code du nouveau segment au reste de la corde
        newLink.GetComponent<RopeSegment>().connectedBelow = top.gameObject;
        top.connectedBody = newLink.GetComponent<Rigidbody2D>();
        top.GetComponent<RopeSegment>().ResetAnchor();
        top = hj;
    }

    public void RemoveLink()
    {
        /*
        if (top.gameObject.GetComponent<RopeSegment>().isPlayerAttached)
            player.Slide();
        */
        //La variable qui contient le nouveau sommet de la corde, dans laquelle on store le segment en dessous du top actuelle
        HingeJoint2D newTop = top.gameObject.GetComponent<RopeSegment>().connectedBelow.GetComponent<HingeJoint2D>();

        //Connecte le nouveau top au hook
        newTop.connectedBody = hook;
        newTop.gameObject.transform.position = hook.gameObject.transform.position;

        //Reset l'ancre du noveau top 
        newTop.GetComponent<RopeSegment>().ResetAnchor();

        //Detruit le segment au top
        Destroy(top.gameObject);

        //Set le nouveau top
        top = newTop;
    }
}
