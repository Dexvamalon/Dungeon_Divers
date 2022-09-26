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
        
        for(obscuredPath.Lenght)
        {
            if(obstacleVariantBlock[x][i] == true)
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
            if(obstacleVariantBlock[x][i] == true && obscuredPath[i] == true)
            {
                colliding = true;
            }
        }

        if(colliding)
        {

        //todo instead of this add a function that determines the working spaces every time a obsticle is placed.
        /////////////////////////////////// Might not do

            list[] starts = new list[8] // list of starts of placed obstacles sorted by their lanes
            list[] ends = new list[8]   // list of ends of placed obstacles sorted by their lanes
            list<float> rellevant starts = new List<float> // list of starts that will affect current obstacle that is being placed
            list<float> rellevant ends = new List<float>   // list of ends that will affect current obstacle that is being placed
            

            for(starts)
            {
                if(obstacleVariantBlock[x][i] == true && obscuredPath[i] == true)
                {
                    rellevantStarts.Add(starts[i])
                    rellevantStarts.Add(End of pathway)
                    rellevantEnds.Add(ends[i])
                    rellevantStarts.Add(Start of pathway)
                }
            }

            go throgh rellevant starts and ends, remove the ones that make up a smaller space than the length of the obstacle

            float smallest start
            float smallest end

            for(rellevant starts)
            {
                for(rellevat starts)
                {
                    if(j = 0)
                    smallest starts = rellevant starts[i]
                    if(rellevant starts[i] < smallest start)
                    smallest starts = rellevant starts[i]
                }
                for(rellevat ends)
                {
                    if(j = 0)
                    smallest ends = rellevant ends[i]
                    if(rellevant ends[i] < smallest ends)
                    smallest ends = rellevant ends[i]
                }

                if(smallest start - smallest end > length of obstacle)
                {
                    collidingy = 
                }

        ///////////////////////// Might not do
                
            }

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
