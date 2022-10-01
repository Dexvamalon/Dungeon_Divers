using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private List<Transform> obstacles = new List<Transform>();
    [SerializeField] private float levelSpeed = 1f;

    [Header("Pathway variables")]
    private bool[] activePathwayUp = new bool[4];
    private bool[] activePathwayDown = new bool[4];
    private bool[] obscuredPath = new bool[8];

    private bool onBrake = false;
    private bool hasPlacedDown = false;

    [SerializeField] private float pathLengthMax = 1f;
    [SerializeField] private float pathLengthMin = 1f;

    [SerializeField] private float pauseLengthMax = 1f;
    [SerializeField] private float pauseLengthMin = 1f;

    private float pathLength = 1f;

    [SerializeField] private float placeChance = 1f; //(chance = 1/placeChance)

    [SerializeField] private bool[] pathwayStructure1 = new bool[4]; // 2 high
    [SerializeField] private bool[] pathwayStructure2 = new bool[4]; // single up
    [SerializeField] private bool[] pathwayStructure3 = new bool[4]; // single down
    [SerializeField] private bool[] pathwayStructure4 = new bool[4]; // double up
    [SerializeField] private bool[] pathwayStructure5 = new bool[4]; // double down
    //(bool array of 4 units 2 upp 2 down)

    private float[] paths = new float[4];

    [Header("Obstacle List Variables")]
    private List<bool[]> obstacleVariantBlock = new List<bool[]>();
    [SerializeField] private List<GameObject> prefabs = new List<GameObject>(); // make sure to place items in the right order // not yet decided
    private List<GameObject> temporaryPrefabVariants = new List<GameObject>();
    private List<float> temporaryPrefabLength = new List<float>();

    private List<Transform> obstacleParentsList = new List<Transform>();
    private Transform obstacleParent;
    // postion of the end of a segment, empty game object with the obstacles within, 
    [SerializeField] private float placePos = 10f;
    [SerializeField] private int maxObstacles = 5;
    [SerializeField] private int maxObstaclesPause = 3;


    private List<float>[] starts = new List<float>[8];
    private List<float>[] ends = new List<float>[8];




    private void Start()
    {
        paths = FindObjectOfType<PlayerMovement>().lanes;
        /*obstacleParent = new GameObject("Obstacle parent").transform;
        obstacleParentsList.Add(obstacleParent);
        Instantiate(prefabs[1], new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 0), obstacleParent.transform);
        */
    }

    void Update()
    {
        for(int i = 0; i < obstacleParentsList.Count; i++)
        {
            obstacleParentsList[i].position -= new Vector3(0, levelSpeed * Time.deltaTime, 0);
        }

        if(obstacleParent == null)
        {
            Debug.Log("place first");
            PlacePathway();
        }
        else if(obstacleParent.position.y < placePos)
        {
            Debug.Log("place normal" + obstacleParent.position.y);
            PlacePathway();
        }
    }



    void PlacePathway()
    {
        for(int i = 0; i < activePathwayDown.Length; i++)
        {
            activePathwayDown[i] = false;
            activePathwayUp[i] = false;
        }
        
        if(!onBrake)
        {
            hasPlacedDown = false;
            
            pathLength = Random.Range(pathLengthMax, pathLengthMin);

            for(int i = 0; i < paths.Length; i++)
            {
                if(Random.Range(0, placeChance) <= 1)
                {
                    switch (Random.Range(1, 6))
                    {
                        case 1:
                            PlacePath(pathwayStructure1, i);
                            break;
                        case 2:
                            PlacePath(pathwayStructure2, i);
                            break;
                        case 3:
                            PlacePath(pathwayStructure3, i);
                            break;
                        case 4:
                            PlacePath(pathwayStructure4, i);
                            break;
                        case 5:
                            PlacePath(pathwayStructure5, i);
                            break;
                        default:
                            break;
                    }
                }
                //Debug.Log("up" + activePathwayUp[0] + " " + activePathwayUp[1] + " " + activePathwayUp[2] + " " + activePathwayUp[3]);
                //Debug.Log("down" + activePathwayDown[0] + " " + activePathwayDown[1] + " " + activePathwayDown[2] + " " + activePathwayDown[3]);
            }
            if(!hasPlacedDown)
            {
                int row = Random.Range(0, 4);

                switch (Random.Range(1, 4))
                {
                    case 1:
                        PlacePath(pathwayStructure1, row);
                        break;
                    case 2:
                        PlacePath(pathwayStructure3, row);
                        break;
                    case 3:
                        PlacePath(pathwayStructure5, row);
                        break;
                    default:
                        break;
                }
            }
            //Debug.Log("up" + activePathwayUp[0] + " " + activePathwayUp[1] + " " + activePathwayUp[2] + " " + activePathwayUp[3]);
            //Debug.Log("down" + activePathwayDown[0] + " " + activePathwayDown[1] + " " + activePathwayDown[2] + " " + activePathwayDown[3]);

            onBrake = true;
        }
        else
        {
            pathLength = Random.Range(pauseLengthMax, pauseLengthMin);

            for(int i = 0; i < paths.Length; i++)
            {
                activePathwayDown[i] = true;
            }

            onBrake = false;
        }

        Debug.Log("up" + activePathwayUp[0] + " " + activePathwayUp[1] + " " + activePathwayUp[2] + " " + activePathwayUp[3]);
        Debug.Log("down" + activePathwayDown[0] + " " + activePathwayDown[1] + " " + activePathwayDown[2] + " " + activePathwayDown[3]);

        PlaceObstacles();
    }

    void PlaceObstacles() //todo debug
    {

        List<float>[] workingEnds = new List<float>[8];    //array of lists (working ends)
        List<float>[] workingStarts = new List<float>[8];  //array of lists (working starts)
        List<float> usedStarts = new List<float>();   //list (used starts)
        List<float> usedEnds = new List<float>();    //list(used ends)
        List<float> temporaryStarts = new List<float>();    //list(temporary starts)
        List<float> temporaryEnds = new List<float>();

        float yPos;
        bool colliding = false;

        FindWorkingObstacles();

        if (temporaryPrefabVariants.Count > 0)
        {
            //place them
            //pick random of the available ones
            int x = Random.Range(0, temporaryPrefabVariants.Count);


            obstacleParent = new GameObject("Obstacle parent").transform;
            obstacleParent.position = new Vector3(0, placePos + pathLength, 0);
            Debug.Log(obstacleParent.position.y);
            obstacleParentsList.Add(obstacleParent);
            //Debug.Log(x);
            //Debug.Log(temporaryPrefabVariants.Count);
            yPos = Random.Range(obstacleParent.position.y - pathLength + temporaryPrefabVariants[x].transform.lossyScale.y / 2,
                                obstacleParent.position.y - temporaryPrefabVariants[x].transform.lossyScale.y / 2);

            Instantiate(temporaryPrefabVariants[x], new Vector3(temporaryPrefabVariants[x].GetComponent<Info>().GetBlockPosition(), yPos, 0), Quaternion.Euler(0, 0, 0), obstacleParent.transform);

            FindAvilableYSpace(yPos - temporaryPrefabVariants[x].transform.lossyScale.y / 2, yPos + temporaryPrefabVariants[x].transform.lossyScale.y / 2, x);

            for (int i = 0; i < obscuredPath.Length; i++)
            {
                if (obstacleVariantBlock[x][i])
                {
                    obscuredPath[i] = true;
                }
            }

            temporaryPrefabVariants.RemoveAt(x);
            obstacleVariantBlock.RemoveAt(x);
            temporaryPrefabLength.RemoveAt(x);
            if(temporaryPrefabVariants.Count <= 0)
            {
                return;
            }
            // 
            int curMaxObstacles = 0;
            if (onBrake)
            {
                curMaxObstacles = maxObstaclesPause;
            }
            else
            {
                curMaxObstacles = maxObstacles;
            }
            for (int a = 0; a < curMaxObstacles; a++)
            {
                List<int> curObstacleBlock = new List<int>();

                //pick random other that would work
                x = Random.Range(0, temporaryPrefabVariants.Count);
                for (int i = 0; i < obscuredPath.Length; i++)
                {
                    //Debug.Log(x + " " + i);
                    //Debug.Log(obstacleVariantBlock.Count + " " + obscuredPath.Length);
                    if (obstacleVariantBlock[x][i] && obscuredPath[i])
                    {
                        colliding = true;
                    }
                }

                if (colliding)
                {
                    //var

                    for (int i = 0; i < workingEnds.Length; i++)
                    {
                        workingEnds[i] = new List<float>();
                        workingStarts[i] = new List<float>();
                    }

                    for (int i = 0; i < starts.Length; i++)
                    {
                        if (obstacleVariantBlock[x][i])
                        {
                            curObstacleBlock.Add(i);

                            for (int j = 0; j < curObstacleBlock.Count; j++)
                            {
                                for (int k = 0; k < ends[curObstacleBlock[j]].Count; k++)
                                {
                                    if (ends[curObstacleBlock[j]][k] - starts[curObstacleBlock[j]][k] > temporaryPrefabLength[x])
                                    {
                                        Debug.Log(j + " " + workingEnds.Length);
                                        workingEnds[j].Add(ends[curObstacleBlock[j]][k]);
                                        workingStarts[j].Add(starts[curObstacleBlock[j]][k]);
                                    }
                                }
                            }
                        }
                        if (curObstacleBlock.Count >= 2)
                        {
                            for (int j = 0; j < workingEnds[curObstacleBlock[0]].Count; j++)
                            {
                                for (int k = 0; k < workingEnds[curObstacleBlock[1]].Count; k++)
                                {
                                    Debug.Log(j + " " + k + " " + workingEnds[curObstacleBlock[0]].Count + " " + workingEnds[curObstacleBlock[1]].Count + " + " + workingStarts[curObstacleBlock[0]].Count + " " + workingStarts[curObstacleBlock[1]].Count);
                                    if (workingEnds[curObstacleBlock[1]][k] < workingEnds[curObstacleBlock[0]][j] && workingStarts[curObstacleBlock[1]][k] > workingStarts[curObstacleBlock[0]][j])
                                    {
                                        usedStarts.Add(workingStarts[curObstacleBlock[1]][k]);
                                        usedEnds.Add(workingEnds[curObstacleBlock[1]][k]);
                                    }
                                    else if (workingStarts[curObstacleBlock[1]][k] > workingStarts[curObstacleBlock[0]][j] && workingStarts[curObstacleBlock[1]][k] < workingEnds[curObstacleBlock[0]][j] && workingEnds[curObstacleBlock[1]][k] > workingEnds[curObstacleBlock[0]][j])
                                    {
                                        usedStarts.Add(workingStarts[curObstacleBlock[1]][k]);
                                        usedEnds.Add(workingEnds[curObstacleBlock[0]][j]);
                                    }
                                    else if (workingEnds[curObstacleBlock[1]][k] > workingStarts[curObstacleBlock[0]][j] && workingEnds[curObstacleBlock[1]][k] < workingEnds[curObstacleBlock[0]][j] && workingEnds[curObstacleBlock[1]][k] < workingStarts[curObstacleBlock[0]][j])
                                    {
                                        usedStarts.Add(workingStarts[curObstacleBlock[0]][j]);
                                        usedEnds.Add(workingEnds[curObstacleBlock[1]][k]);
                                    }
                                    else if (workingStarts[curObstacleBlock[1]][k] < workingStarts[curObstacleBlock[0]][j] && workingEnds[curObstacleBlock[1]][k] > workingEnds[curObstacleBlock[0]][j])
                                    {
                                        usedStarts.Add(workingStarts[curObstacleBlock[0]][j]);
                                        usedEnds.Add(workingEnds[curObstacleBlock[0]][j]);
                                    }
                                }
                            }
                        }
                        for (int j = 0; j < curObstacleBlock.Count - 2; j++)//j
                        {
                            for (int t = 0; t < usedStarts.Count; t++)//t
                            {

                                for (int k = 0; k < workingEnds[curObstacleBlock[j - 2]].Count; k++)//k
                                {

                                    if (workingEnds[curObstacleBlock[j - 2]][k] < usedEnds[k] && workingStarts[curObstacleBlock[j - 2]][k] > usedStarts[k])
                                    {
                                        temporaryStarts.Add(workingStarts[curObstacleBlock[j - 2]][k]);
                                        temporaryEnds.Add(workingEnds[curObstacleBlock[j - 2]][k]);
                                    }
                                    else if (workingStarts[curObstacleBlock[j - 2]][k] > usedStarts[k] && workingStarts[curObstacleBlock[j - 2]][k] < usedEnds[k] && workingEnds[curObstacleBlock[j - 2]][k] > usedEnds[k])
                                    {
                                        temporaryStarts.Add(workingStarts[curObstacleBlock[j - 2]][k]);
                                        temporaryEnds.Add(usedEnds[k]);
                                    }
                                    else if (workingEnds[curObstacleBlock[j - 2]][k] > usedStarts[k] && workingEnds[curObstacleBlock[j - 2]][k] < usedEnds[k] && workingStarts[curObstacleBlock[j - 2]][k] < usedStarts[k])
                                    {
                                        temporaryStarts.Add(usedStarts[k]);
                                        temporaryEnds.Add(workingEnds[curObstacleBlock[j - 2]][k]);
                                    }
                                    else if (workingStarts[curObstacleBlock[j - 2]][k] < usedStarts[k] && workingEnds[curObstacleBlock[j - 2]][k] > usedEnds[k])
                                    {
                                        temporaryStarts.Add(usedStarts[k]);
                                        temporaryEnds.Add(usedEnds[k]);
                                    }
                                }
                            }

                            usedStarts = temporaryStarts;
                            usedEnds = temporaryEnds;
                            temporaryStarts = new List<float>();
                            temporaryEnds = new List<float>();
                        }
                    }
                    for (int j = 0; j < usedStarts.Count; j++)
                    {
                        if (usedEnds[j] - usedStarts[j] < temporaryPrefabLength[x])
                        {
                            usedEnds.RemoveAt(j);
                            usedStarts.RemoveAt(j);
                        }
                    }

                    if (usedStarts.Count > 0)
                    {
                        int z = Random.Range(0, usedStarts.Count);
                        yPos = Random.Range(usedStarts[z] + temporaryPrefabVariants[x].transform.lossyScale.y / 2,
                                            usedEnds[z] - temporaryPrefabVariants[x].transform.lossyScale.y / 2);
                        Instantiate(temporaryPrefabVariants[x], new Vector3(temporaryPrefabVariants[x].GetComponent<Info>().GetBlockPosition(), yPos, 0), Quaternion.Euler(0, 0, 0), obstacleParent.transform);
                    }
                    else
                    {
                        temporaryPrefabVariants.RemoveAt(x);
                        obstacleVariantBlock.RemoveAt(x);
                        temporaryPrefabLength.RemoveAt(x);
                        if (temporaryPrefabVariants.Count <= 0)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    yPos = Random.Range(obstacleParent.position.y - pathLength + temporaryPrefabVariants[x].transform.lossyScale.y / 2,
                                    obstacleParent.position.y - temporaryPrefabVariants[x].transform.lossyScale.y / 2);
                    Instantiate(temporaryPrefabVariants[x], new Vector3(temporaryPrefabVariants[x].GetComponent<Info>().GetBlockPosition(), yPos, 0), Quaternion.Euler(0, 0, 0), obstacleParent.transform);
                }
            }
        }
    }

    void FindAvilableYSpace(float newObstacleStart, float newObstacleEnd, int x) //todo have to debug
    {

        //var
        //array of lists (ends)
        //array of lists (starts)
        bool[] workingSpace = new bool[8];

        int var = 0;


        //Set to default (when starting to place a new pathway)

        for (int j = 0; j < ends.Length; j++)
        {
            ends[j] = new List<float>();
            starts[j] = new List<float>();

            if (obstacleVariantBlock[x][j])
            {
                
                ends[j].Add(obstacleParent.transform.position.y);
                starts[j].Add(obstacleParent.transform.position.y - temporaryPrefabLength[x]);
                ////fixing arrays
                //fing biggest ends thats smaller than new obstacle start

                for (int i = 0; i < ends[j].Count; i++)
                {
                    if (i == 0)
                    {
                        var = i;
                    }
                    else if (ends[j][i] > ends[j][var] && ends[j][i] <= newObstacleStart)
                    {
                        var = i;
                    }
                }
                //if end of same index > new obstacle start
                //end = new obstacle start  
                if (starts[j][var] >= newObstacleStart)
                {
                    //Debug.Log(starts[j][var] + " " + ends[j][var] + " " + newObstacleStart);
                    ends[j].Add(ends[j][var]);
                    starts[j].Add(ends[j][var]);
                    starts[j][var] = newObstacleStart;
                }

                //find smallest starts thats bigger than new obstacle end

                for (int i = 0; i < starts[j].Count; i++)
                {
                    if (i == 0)
                    {
                        var = i;
                    }
                    else if (starts[j][i] < starts[j][var] && starts[j][i] >= newObstacleEnd)
                    {
                        var = i;
                    }
                }

                //if star of same index < new obstacle end
                //start = new obstacle end

                if (ends[j][var] >= newObstacleEnd)
                {
                    ends[j].Add(ends[j][var]);
                    starts[j].Add(ends[j][var]);
                    ends[j][var] = newObstacleEnd;
                }



                for(int i = 0; i < ends[j].Count; i++)
                {
                    if (ends[j][i] > newObstacleStart && ends[j][i] < newObstacleEnd && starts[j][i] > newObstacleStart && starts[j][i] < newObstacleEnd)
                    {
                        ends[j].RemoveAt(i);
                        starts[j].RemoveAt(i);
                        i--;
                    }
                }
            }
        }   
    }

    void FindWorkingObstacles()
    {
        for(int i = 0; i < prefabs.Count; i++)
        {
            obstacleVariantBlock.Add(prefabs[i].GetComponent<Info>().GetObstacleBlock());
            temporaryPrefabLength.Add(prefabs[i].GetComponent<Info>().GetLength());
            temporaryPrefabVariants.Add(prefabs[i]);
        }

        for(int i = 0; i < temporaryPrefabVariants.Count; i++)
        {
            if(temporaryPrefabLength[i] > pathLength)
            {
                temporaryPrefabVariants.RemoveAt(i);
                obstacleVariantBlock.RemoveAt(i);
                temporaryPrefabLength.RemoveAt(i);
            }
        }

        for(int i = 0; i < activePathwayUp.Length; i++)
        {
            if(activePathwayUp[i])
            {
                for(int x = 0; x < obstacleVariantBlock.Count; x++)
                {
                    if(obstacleVariantBlock[x][i]) //same i as in the first for loop
                    {
                        obstacleVariantBlock.RemoveAt(x);
                        temporaryPrefabVariants.RemoveAt(x);
                        temporaryPrefabLength.RemoveAt(x);
                    }
                }
            }
        }

        for (int i = 0; i < activePathwayDown.Length; i++)
        {
            if(activePathwayDown[i])
            {
                for(int x = 0; x < obstacleVariantBlock.Count; x++)
                {
                    if(obstacleVariantBlock[x][i+4]) //same i as in the first for loop
                    {
                        obstacleVariantBlock.RemoveAt(x);
                        temporaryPrefabVariants.RemoveAt(x);
                        temporaryPrefabLength.RemoveAt(x);
                    }
                }
            }
        }
        for(int i = 0; i < obstacleVariantBlock.Count; i++)
        {
            Debug.Log("i" + obstacleVariantBlock[i][0] + " " +
                      obstacleVariantBlock[i][1] + " " +
                      obstacleVariantBlock[i][2] + " " +
                      obstacleVariantBlock[i][3] + " " +
                      obstacleVariantBlock[i][4] + " " +
                      obstacleVariantBlock[i][5] + " " +
                      obstacleVariantBlock[i][6] + " " +
                      obstacleVariantBlock[i][7]);
        }
    }

    void PlacePath(bool[] pathwayStructure, int row)
    {
        
        if(pathwayStructure[0])
        {
            activePathwayUp[row] = true;
        }
        if(pathwayStructure[2])
        {
            activePathwayDown[row] = true;
            hasPlacedDown = true;
        }
        if(pathwayStructure[1])
        {
            switch (row)
            {
                case 0:
                    activePathwayUp[1] = true;
                    break;
                case 1:
                    if(Random.Range(1,3) == 1)
                    {
                        activePathwayUp[0] = true;
                    }
                    else
                    {
                        activePathwayUp[2] = true;
                    }
                    break;
                case 2:
                    if(Random.Range(1,3) == 1)
                    {
                        activePathwayUp[1] = true;
                    }
                    else
                    {
                        activePathwayUp[3] = true;
                    }
                    break;
                case 3:
                    activePathwayUp[2] = true;
                    break;
            }
        }
        if(pathwayStructure[3])
        {
            switch (row)
            {
                case 0:
                    activePathwayDown[1] = true;
                    break;
                case 1:
                    if(Random.Range(1,3) == 1)
                    {
                        activePathwayDown[0] = true;
                    }
                    else
                    {
                        activePathwayDown[2] = true;
                    }
                    break;
                case 2:
                    if(Random.Range(1,3) == 1)
                    {
                        activePathwayDown[1] = true;
                    }
                    else
                    {
                        activePathwayDown[3] = true;
                    }
                    break;
                case 3:
                    activePathwayDown[2] = true;
                    break;
            }
            hasPlacedDown = true;
        }
    }
}
