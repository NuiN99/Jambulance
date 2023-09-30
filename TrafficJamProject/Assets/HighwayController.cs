using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class HighwayController : MonoBehaviour
{
    public Transform trafficSpawn;
    public GameObject enemyCar;
    [SerializeField] int lanes;
    [SerializeField] float spawnTimer;
    [SerializeField] int carsPerRow;

    GameObject[][] carGrid;

    private void Awake()
    {
        carGrid = new GameObject[10][];
    }

    void Start()
    {
        StartCoroutine(SpawnTraffic());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnTraffic()
    {
        while (true)
        {
            
            GameObject[] newRow = new GameObject[lanes];

            //spawm cars while less than max car per rows
            for (int i = 0; i < carsPerRow; i++)
            {
                // get random index

                // spawn car on space if not occupied

                
            }




            //save cars spawned to row
            for (int i = 0; i < carGrid.GetLength(0); i++)
            {
                //if the row is not emty
                if (carGrid[i] == null)
                {
                    continue;
                }
                else
                {

                }
                
                
            }
            //if grid full delete first rown and move them all one back
            if (carGrid.GetLength(0) != null)
            {
                print("Full!");
                yield return new WaitForSeconds(spawnTimer);
                continue;
            }

           

            yield return new WaitForSeconds(spawnTimer);
        }
        


        //spawn traffic row
        
    }

}
