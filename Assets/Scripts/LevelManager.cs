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

    private List<List<GameObject>> listsOfLists = new List<List<GameObject>>();

    private Transform obstacleParent;
    // postion of the end of a segment, empty game object with the obstacles within, 
    [SerializeField] private float placePos = 10f;

    private void Start()
    {
        paths = FindObjectOfType<PlayerMovement>().lanes;
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
    }


    void PlaceObstacles()
    {
        /*

        FindWorkingBastacles();

        place them
        pick random of the available ones
        place the obsticle at a random working position

        pick random other that would work
        if overlap
        check if there is space left
        if is place at random working place
        else remove obsticle from list
        try again x times

        */
    }

    //private List<bool[]> obstacleBlockVariants = new List<bool[]>(); //move up later
    //private List<GameObject> prefabVariants = new List<GameObject>(); //move up later
    //private List<GameObject> pregabObstacles = new List<GameObject>();

    void FindWorkingObstacles()
    {
        /*
        private List<bool[]> obsticleVariantBlock = new List<bool[]>();
        private List<GameObject> prefabVariants = new List<GameObject>(); // make sure to place items in the right order // not yet decided
        private List<GameObject> temporaryPrefabVariants = new List<GameObject>();
        create lists
        /temproaryPrefabVariants = prefabVariants;
        for(int i = 0; i < prefabVariants.Lenght; i++)
        {
            /obsticleVariantBlock = prefabVariants[i].GetBlockVariants();
        }



        */
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
