using UnityEngine;

public class MountainGenerator : MonoBehaviour
{
    //What I did --------------------------------

    // created a mountain lanscape, the edges have a higher survival threshold. as it gets closer to the center has lower thesholds.
    //weighted 2-cell neighborhood to smooth out the landscape, to make it look natural.
    //added higher Y cells for the edges to really empathize the look of mountians
    //created a slope so that the edges gradually taper toward the center, Like a real mountian!


    public int width = 30;
    public int height = 15;
    public int depth = 30;

    [Range(0f, 1f)]
    public float fillProb = 0.45f;// V: Random using this value to choose if cube is a 1 or 0 

    public int iterations = 4;
    public float centerDeathRatio = 0.4f;
    public float edgeDeathRatio = 0.6f;

    public float birthRatio = 0.55f; 

    public GameObject cubePrefab;

    private int[,,] grid;

    void Start()
    {
        GenerateLand();
    }

    public void GenerateLand()
    {
        // Step 1: Initialize grid
        grid = new int[width, height, depth];
        InitializeGrid();

        // Step 2: Run cellular automata
        for (int i = 0; i < iterations; i++)
            grid = RunCA(grid);

      /*  for (int i = 0; i < iterations; i++) //V: helpful debug to see how many cubes were counted each iteration
        {
            grid = RunCA(grid);
            int cubeCount = 0;

            foreach (int cell in grid)
            {
                if (cell == 1) cubeCount++;
                Debug.Log("Cubes after iteration " + i + ": " + cubeCount);
            }

        }*/

        // Step 3: Render the landscape
        RenderLand();
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
                    float neighbors = CountWeightedNeighbors(x, y, z, oldGrid);

                    float deathLimit = GetDeathLimit(x, y, z);


                    if (oldGrid[x, y, z] == 1)
                    {
                        newGrid[x, y, z] = (neighbors < deathLimit) ? 0 : 1;  //V: see if its a mountian or not
                    }
                    else // V: start of empty cell
                    {
                        newGrid[x, y, z] = (neighbors > birthRatio) ? 1 : 0;
                    }

                }
            }
        }

        return newGrid;
    }

    float GetDeathLimit(int x, int y, int z)
    {
        float dx = x - width / 2f;
        float dz = z - depth / 2f;
        float horizontalDistance = Mathf.Sqrt(dx * dx + dz * dz) / 
            (Mathf.Sqrt((width / 2f) * (width / 2f) + (depth / 2f) * (depth / 2f))); // V: Edges gradually taper down

        float fallOff = Mathf.Pow(horizontalDistance, 2f); //V: slopes toward the center

        float yFactor = y / (float)height; //V: makes the mountains taller!

        float deathRatio = Mathf.Lerp(centerDeathRatio, edgeDeathRatio, fallOff) + yFactor * 0.2f; //V: combines falloff and yfactor

        return Mathf.Clamp(deathRatio, 0f, 1f);

    }


    float CountWeightedNeighbors(int x, int y, int z, int[,,] grid)
    {
        float count = 0f;
        float maxWeight = 0f;

        for (int dx = -2; dx <= 2; dx++)
        {
            for (int dy = -2; dy <= 2; dy++)
            {
                for (int dz = -2; dz <= 2; dz++)
                {

                    if (dx == 0 && dy == 0 && dz == 0) // V: skip center
                        continue;

                    float weight = 1f / (Mathf.Abs(dx) + Mathf.Abs(dy) + Mathf.Abs(dz));
                    maxWeight += weight;

                    int xn = x + dx;
                    int yn = y + dy;
                    int zn = z + dz;

                    if (xn >= 0 && xn < width && yn >= 0 && yn < height && zn >= 0 && zn < depth) // V: makes sure we arent outside of the grid
                    {
                        count += grid[xn, yn, zn] * weight;
                    }
                    else
                    {
                        count += 1f * weight; // V: this makes sure everything outside of the grid is filled in
                    }

                }
            }
        }

        return count / maxWeight;
    }

    void RenderLand()
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
                for (int z = 0; z < depth; z++)
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
