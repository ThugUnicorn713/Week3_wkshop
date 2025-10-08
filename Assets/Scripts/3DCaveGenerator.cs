using UnityEngine;

public class CACaveGenerator3D : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public int depth = 10;

    [Range(0f, 1f)]
    public float fillProb = 0.25f;// V: Random using this value to choose if cube is a 1 or 0 

    public int iterations = 5;

    // if neighbor number > birthLimit, set the current cell as a cube
    public int birthLimit = 14;
    // if neighbor number <>> birthLimit, set the current cell as empty
    public int deathLimit = 5;

    public GameObject cubePrefab; 

    private int[,,] grid;

    void Start()
    {
        GenerateCave();
    }

    public void GenerateCave()
    {
        // Step 1: Initialize grid
        grid = new int[width, height, depth];
        InitializeGrid();

        // Step 2: Run cellular automata
        for (int i = 0; i < iterations; i++)
            grid = RunCA(grid);

        for (int i = 0; i < iterations; i++) //V: helpful debug to see how many cubes were counted each iteration
        {
            grid = RunCA(grid);
            int cubeCount = 0;
            
            foreach (int cell in grid)
            {
                if (cell == 1) cubeCount++;
                Debug.Log("Cubes after iteration " + i + ": " + cubeCount);
            }
               
        }

        // Step 3: Render the cave
        RenderCave();
    }

    void InitializeGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {

                    bool onBorder = (x == 0 || x == width - 1 || y == 0 || y == height - 1 || z == 0 || z == depth - 1);

                    if (onBorder)
                    {
                        grid[x, y, z] = 1; // V: if on border cube will ALWAYS be 1
                    }
                    else
                    {
                        grid[x, y, z] = (Random.value < fillProb) ? 1 : 0; // V: R.value picks a value (float) bwtn 0 and 1
                    }

                }

            }

        }
    }

    int[,,] RunCA(int[,,] oldGrid) //V: basically checks if a grid should be used or not
    {
        int[,,] newGrid = new int[width, height, depth];

        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                for (int z = 1; z < depth - 1; z++)
                {
                    int neighbors = CountSolidNeighbors(x, y, z, oldGrid);
                    
                    if (oldGrid[x, y, z] == 1)
                    {
                        if (neighbors < deathLimit)
                        {
                            newGrid[x, y, z] = 0; // V: it dead 

                        }
                        else
                        {
                            newGrid[x, y, z] = 1; //V: its ALIVEEEEE
                        }
                    }
                    else // V: start of empty cell
                    {
                        if (neighbors > birthLimit)
                        {
                            newGrid[x, y, z] = 1; // V: hello neighbor!
                        }
                        else
                        {
                            newGrid[x, y, z] = 0; // V: no neighbor!
                        }
                    }
                   
                }
            }
        }

        return newGrid;
    }

    int CountSolidNeighbors(int x, int y, int z, int[,,] grid)
    {
        int count = 0;

        for (int xn = x - 1; xn <= x + 1; xn++)
        {
            for (int yn = y - 1; yn <= y + 1; yn++)
            {
                for (int zn = z - 1; zn <= z + 1; zn++)
                {

                    if (xn == x && yn == y && zn == z) // V: skip center
                        continue;

                    if (xn >= 0 && xn < width &&  yn >= 0 && yn < height && zn >= 0 && zn < depth) // V: makes sure we arent outside of the grid
                    {
                        count += grid[xn, yn, zn]; 
                    }
                    else
                    {
                        count++; // V: this makes sure everything outside of the grid is filled in
                    }

                }
            }
        }

        return count;
    }

    void RenderCave()
    {
        // Clear previous cubes
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for(int z = 0; z < depth; z++)
                {

                    if (grid[x, y, z] == 1)
                    {
                        Vector3 cubePOS = new Vector3(x, y, z);
                        GameObject cube = Instantiate(cubePrefab, cubePOS, Quaternion.identity);

                        cube.transform.parent = transform; //V: keeps hierarchy clean :)
                    }
                }
            }
        }
    }
}
