using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class HighwayController : MonoBehaviour
{
    public Transform trafficSpawn;
    public GameObject enemyCar;
    [SerializeField] int lanes;
    [SerializeField] int maxRows;//TBI
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
        GameObject[] newRow;
        int randomIndex;

        while (true)
        {
            newRow = new GameObject[lanes];
            randomIndex = 0;

            //spawm cars while less than max car per rows
            for (int i = 0; i < carsPerRow; i++)
            {
                do
                {
                    // get random empty index
                    randomIndex = Random.Range(0, lanes);
                
                } while (newRow[randomIndex] != null);

                // spawn car on space if not occupied
                GameObject newCar = Instantiate(enemyCar,trafficSpawn,false);
                newCar.transform.position = trafficSpawn.transform.position + transform.right * (randomIndex * 1f);
                newRow[randomIndex] = newCar;
            }

            //save cars spawned to row
            for (int i = 0; i < carGrid.GetLength(0) - 1; i++)
            {
                //if the row is not emty
                if (carGrid[i] == null)
                {
                    carGrid[i] = newRow;
                    print("row added");
                    break;
                    
                }
                else
                {
                    print("row skipped");
                    continue;
                }  
            }
            //if grid full delete first rown and move them all one back
            if (carGrid[carGrid.GetLength(0)-1] != null)
            {
                print(carGrid[carGrid.GetLength(0) - 1]);
                print("Full!");
                yield return new WaitForSeconds(spawnTimer);
                continue;
            }
           
            yield return new WaitForSeconds(spawnTimer);
        }
        


        //spawn traffic row
        
    }

}
