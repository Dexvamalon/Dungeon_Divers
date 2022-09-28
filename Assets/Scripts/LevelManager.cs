using System.Collections;
using System.Collections.Generic;
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
    private List<List<GameObject>> listsOfLists = new List<List<GameObject>>();
    private List<bool[]> obstacleVariantBlock = new List<bool[]>();
    [SerializeField] private List<GameObject> prefabs = new List<GameObject>(); // make sure to place items in the right order // not yet decided
    private List<GameObject> temporaryPrefabVariants = new List<GameObject>();
    private List<float> temporaryPrefabLength = new List<float>();

    private List<Transform> obstacleParentsList = new List<Transform>();
    private Transform obstacleParent;
    // postion of the end of a segment, empty game object with the obstacles within, 
    [SerializeField] private float placePos = 10f;

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
        for(int i = 0; i < obstacles.Count; i++)
        {
            obstacles[i].position -= new Vector3(0, levelSpeed * Time.deltaTime, 0);
        }

        if(obstacleParent == null)
        {
            PlacePathway();
            obstacleParent = transform; //remove later, for degbugging.
        }
        /*else if(obstacleParent.position.y < placePos)
        {
            PlacePathway();
        }*/
    }



    void PlacePathway()
    {
        
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
                Debug.Log("up" + activePathwayUp[0] + " " + activePathwayUp[1] + " " + activePathwayUp[2] + " " + activePathwayUp[3]);
                Debug.Log("down" + activePathwayDown[0] + " " + activePathwayDown[1] + " " + activePathwayDown[2] + " " + activePathwayDown[3]);
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
        FindWorkingObstacles(); // remove later
    }

    void PlaceObstacles()
    {
        /*

        FindWorkingObstacles();

        float yPos;
        bool colliding = false;
        bool collidingy = false;

        //place them
        pick random of the available ones
        int x = Random.Range(0, temporaryPrefabVariants.Count)
        

        yPos = Random.Range(obstacleParent.position.y - pathLength + temporaryPrefabVariants[x].transform.lossyScale.y / 2, 
                            obstacleParent.position.y - temporaryPrefabVariants[x].transform.lossyScale.y / 2);
        obstacleParent = new GameObject("Obstacle parent").transform;
        obstacleParentsList.Add(obstacleParent);
        Instantiate(temporaryPrefabVariants[x], new Vector3(temporaryPrefabVariants[x].GetComponent<Info>().GetBlockPosition(), yPos, 0), Quaternion.Euler(0, 0, 0), obstacleParent.transform);

        FindAvilableYSpace(yPos - temporaryPrefabVariants[x].transform.lossyScale.y / 2, yPos + temporaryPrefabVariants[x].transform.lossyScale.y / 2, x)

        for(obscuredPath.Lenght)
        {
            if(obstacleVariantBlock[x][i])
            {
                obscuredPath[i] = true
            }
        }

        temporaryPrefabVariants.RemoveAt(x);
        obstacleVariantBlock.RemoveAt(x);
        temporaryPrefabLength.RemoveAt(x);
        // 

        //pick random other that would work
        x = Random.Range(0, temporaryPrefabVariants.Count)
        for(obscuredPath.Length)
        {
            if(obstacleVariantBlock[x][i] && obscuredPath[i])
            {
                colliding = true;
            }
        }

        List<int> curObstacleBlock = new List<int>;

        if(colliding)
        {
        //todo fix up here
        var
        array of lists (working ends)
        array of lists (working starts)

        for(starts)
        {
        if (obstacleVariantBlock[x][i])
        {
        curObstacleBlock.Add(i)

        for(ends[i])
        {
        if ends[i][j] - starts[i][j] < length of obstacle
        {
        working ends[i].Add(ends[i][j])
        working starts[i].Add(starts[i][j])
        }
        }
        }
        for(curObstacleBlock)
        {
        check for the smallest start in all lists
        check the free space in the first list against the two others
        if both start and end is withing the first lists free space
        in a new list add the end and start.
        if only start is within the first lists free space
        in the new list add start of first list and end of comparing list // same for end
        if neither is within the first lists free space

        }
        }

        //todo to here


            if there is space left (y)
            {
                place at random working place
            }
            else
            {
                remove obsticle from list
            }
        }
        else
        {
            place at random working place
        }

        //if overlap space (x)
        //check if there is space left (y)
        //if is place at random working place
        //else remove obsticle from list
        //try again x times

        */
    }

    void FindAvilableYSpace(float newObstacleStart, float newObstacleEnd, bool x)
    {
        /*

        var
        //array of lists (ends)
        //array of lists (starts)
        List<float>[] starts = new List<float>[8];
        List<float>[] ends = new List<float>[8];
        bool[] working space = new bool[8];

        int var;


        Set to default (when starting to place a new pathway)

        for(ends)
        if(obsticleVariantBlock[x][j])
        {

        ////fixing arrays
        //fing biggest ends thats smaller than new obstacle start

        for(ends[j])
        if i = 0
        var = i
        else if (ends[j][i] > ends[j][var] && ends[j][i] <= new obstacle start)
        var = i

        //if end of same index > new obstacle start
        //end = new obstacle start

        if starts[j][var] >= new obstacle start
        ends[j].Add(ends[j][var])
        starts[j].Add(ends[j][var])
        starts[j][var] = new obstacle start


        //find smallest starts thats bigger than new obstacle end

        for(starts[j])
        if i = 0
        var = i
        else if (starts[j][i] < starts[j][var] && starts[j][i] >= new obstacle end)
        var = i

        //if star of same index < new obstacle end
        //start = new obstacle end

        if ends[j][var] >= new obstacle end
        ends[j].Add(ends[j][var])
        starts[j].Add(ends[j][var])
        ends[j][var] = new obstacle end


        for(ends[j])
        if ends[j][i] > new obstacle start && < new obstacle end && starts[j][i] > new obstacle start && < new obstacle end
        ends[j].removeAt(i)
        starts[j].removeAt(i)

        }


        */
    }

    void FindWorkingObstacles()
    {
        for(int i = 0; i < prefabs.Count; i++)
        {
            obstacleVariantBlock.Add(prefabs[i].GetComponent<Info>().GetObstacleBlock());
            temporaryPrefabLength.Add(prefabs[i].GetComponent<Info>().GetLenght());
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
