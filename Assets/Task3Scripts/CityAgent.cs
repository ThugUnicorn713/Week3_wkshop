using System;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CityAgent : MonoBehaviour
{
    public Vector2Int position; // Grid position
    public int steps;           // How many steps this agent can take
    public GameObject streetPrefab;
    public GameObject buildingPrefab;
    public GameObject parkPrefab;
    public TileType[,] cityGrid;

    public enum AgentType { Street, Building, Park } // V: TASK 5 - Mod 1, giving agents their own jobs
    public AgentType agentType;

    public void Initialize(Vector2Int startPos, TileType[,] grid, int num_steps)
    {
        position = startPos;
        cityGrid = grid;
        steps = num_steps; // Example: agent will move 50 steps
    }

    public void Step()
    {
        if (steps <= 0) return;

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        Vector2Int direction = directions [UnityEngine.Random.Range(0, directions.Length)];

        position += direction;

        position.x = Mathf.Clamp(position.x, 0, cityGrid.GetLength(0) - 1);
        position.y = Mathf.Clamp(position.y, 0, cityGrid.GetLength(1) - 1);


        GameObject prefabOfChoice;
        TileType tileToPlace;

        float randValue = UnityEngine.Random.value;


        //V:  ADDED MOD FOR TASK 5 : from the type the agent is, given a probability of what it places

        switch (agentType)
        {
            case AgentType.Street:
                if (randValue < 0.6f) { tileToPlace = TileType.Street; prefabOfChoice = streetPrefab; }
                else { tileToPlace = TileType.Park; prefabOfChoice = parkPrefab;}
                break;

            case AgentType.Park:
                if (randValue < 0.5f) { tileToPlace = TileType.Park; prefabOfChoice= parkPrefab; }
                else { tileToPlace = TileType.Building; prefabOfChoice = buildingPrefab; }
                break;

            case AgentType.Building:
                if(randValue < 0.4f) { tileToPlace = TileType.Building; prefabOfChoice = buildingPrefab; }
                else { tileToPlace = TileType.Street; prefabOfChoice = streetPrefab;}
                break;

            default:
                tileToPlace = TileType.Street; prefabOfChoice = streetPrefab;
                break;
        }


        PlaceTile(tileToPlace, prefabOfChoice);

        /*
         * V: OG code for TASK 4
         * 
         * 
         * 
         * if (randValue < 0.6F)
        {
            prefabOfChoice = streetPrefab;
            tileToPlace = TileType.Street;
            PlaceTile(TileType.Street, streetPrefab);
        }
        else if (randValue < 0.9f)
        {
            prefabOfChoice = buildingPrefab;
            tileToPlace = TileType.Building;
            PlaceTile(TileType.Building, buildingPrefab);
        }
        else
        {
            prefabOfChoice = parkPrefab;
            tileToPlace = TileType.Park;
            PlaceTile(TileType.Park, parkPrefab);
        }*/

        steps--;
    }

    void PlaceTile(TileType type, GameObject chosenPrefab)
    {
        if (cityGrid[position.x, position.y] != TileType.Empty) return;

        cityGrid[position.x, position.y] = type;

        Vector3 pos = new Vector3(position.x, 0, position.y);

        //V: placed in variable for TASK 5
        GameObject placedObj = Instantiate(chosenPrefab, pos, Quaternion.identity, null); // V: OG code for TASK 4 -
                                                                                          // Instantiate(chosenPrefab, pos, Quaternion.identity, null);

        //V: ADDED MOD FOR TASK 5: if the prefab is a building it randomly choses its height and color

        if (type == TileType.Building)
        {

            float randHeight = UnityEngine.Random.Range(0.5f, 0.9f);
            placedObj.transform.localScale = new Vector3 (1f, randHeight, 1f);


            Renderer rend = placedObj.GetComponent<Renderer>();

            if (rend != null)
                rend.material.color = UnityEngine.Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.6f, 1f);
        }
       
    }

}
