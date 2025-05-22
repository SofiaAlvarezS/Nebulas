using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeveGenerator_1 : MonoBehaviour
{
    [SerializeField] private GameObject[] levelParts;

    [SerializeField] private float minDistance;

    [SerializeField] private Transform finalPoint;

    [SerializeField] private int inicialQuantity;

    private Transform player;

    private void Start()                 //iniciates a loop counter to create random layouts of platforms
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        for (int i = 0; i < inicialQuantity; i++)
        {
            GeneratePartLevel();
        }
    }

    
    private void Update()               //player reachs the final dot to regenerate a loop again
    {
        if (Vector2.Distance(player.position, finalPoint.position) < minDistance)
        {
            GeneratePartLevel();
        }
    }

    private void GeneratePartLevel()   //distance where the new plataforms star to generate
    {
        int randonNumber = Random.Range(0, levelParts.Length);
        GameObject level = Instantiate(levelParts[randonNumber], finalPoint.position, Quaternion.identity);
        finalPoint = seekFinalPoint(level, "FinalPoint");    
        
    }

    private Transform seekFinalPoint(GameObject levelPart, string tag)       //reset the point of regeneration after being reached by the player
    {
        Transform point = null;

        foreach (Transform  location in levelPart.transform)
        {
            if(location.CompareTag(tag))
            {
                point = location;
                break;
            }
        }

        return point;
    }
}
