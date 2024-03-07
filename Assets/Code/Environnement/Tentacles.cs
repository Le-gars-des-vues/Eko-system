using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacles : MonoBehaviour
{
    [SerializeField] private int length;
    [SerializeField] private LineRenderer lineRend;
    public Vector3[] segmentPoses;
    private Vector3[] segmentV;

    //Position initiale
    [SerializeField] private Transform targetDir;
    //Distance entre les points
    [SerializeField] private float targetDist;
    //Vitesse de mouvement des points
    [SerializeField] private float smoothSpeed;
    //Vitesse du bout de la queue
    [SerializeField] private float trailSpeed;

    //Vitesse du wiggle
    [SerializeField] private float wiggleSpeed;
    //Amplitude du wiggle
    [SerializeField] private float wiggleMagnitude;
    [SerializeField] private Transform wiggleDir;

    public bool canShorten = true;

    [SerializeField] bool useBodyParts;
    [SerializeField] Transform[] bodyParts;
    [SerializeField] List<int> bodyPartPoses = new List<int>();

    private void Start()
    {
        //Initialise le line renderer avec le nombre de points
        lineRend.positionCount = length;
        segmentPoses = new Vector3[length];
        segmentV = new Vector3[length];
        ResetPos();
    }

    private void Update()
    {
        //Utilise une fonction sin pour wiggle le tentacule
        wiggleDir.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * wiggleSpeed) * wiggleMagnitude);

        //Met la base du tentacule au target
        segmentPoses[0] = targetDir.position;
        if (useBodyParts)
            bodyParts[0].position = targetDir.position;

        //Si la tentacule peut changer de taille
        if (canShorten)
        {
            int index = 1;
            //Pour chaque tentacule, on le fait bouger vers le bout precedent jusqu'a ce qu'il atteingne une certaine distance
            for (int i = 1; i < segmentPoses.Length; i++)
            {
                segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], segmentPoses[i - 1] + targetDir.right * targetDist, ref segmentV[i], smoothSpeed + (i / trailSpeed));
                if (useBodyParts)
                {
                    foreach (int p in bodyPartPoses)
                    {
                        if (i == p)
                        {
                            bodyParts[index].transform.position = segmentPoses[i];
                            index++;
                        }
                    }
                }
            }
        }
        //Sinon, les morceau de tentacule sont a une distance fixe les uns des autres
        else
        {
            int index = 1;
            for (int i = 1; i < segmentPoses.Length; i++)
            {
                Vector3 targetPos = segmentPoses[i - 1] + (segmentPoses[i] - segmentPoses[i - 1]).normalized * targetDist;
                segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], targetPos, ref segmentV[i], smoothSpeed);
                if (useBodyParts)
                {
                    foreach (int p in bodyPartPoses)
                    {
                        if (i == p)
                        {
                            bodyParts[index].transform.position = segmentPoses[i];
                            index++;
                        }
                    }
                }
            }
        }

        lineRend.SetPositions(segmentPoses);
    }

    private void ResetPos()
    {
        segmentPoses[0] = targetDir.position;
        for (int i = 1; i < length; i++)
        {
            segmentPoses[i] = segmentPoses[i - 1] + targetDir.right * targetDist;
        }
        lineRend.SetPositions(segmentPoses);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (segmentPoses != null)
        {
            for (int i = 0; i < segmentPoses.Length; i++)
            {
                Gizmos.DrawSphere(segmentPoses[i], 0.01f);
            }
        }
    }
}
