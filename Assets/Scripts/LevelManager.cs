using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        
    }

    void Update()
    {
        for(int i = 0; i < obstacles.Count; i++)
        {
            obstacles[i].position -= new Vector3(0, levelSpeed * Time.deltaTime, 0);
        }
    }

    void PlacePathway()
    {
        /*
        
        if(!onBrake)
        {
            hasPlacedDown = false;
            
            pathLength = /random.range pathLengthMax pathLengthMin

            for(int i = 0; i < pahts.count; i++)
            {
                if(Random.Range(1, placeChance) == 1)
                {
                    Switch (Random.Range(1, 5))
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
            }
            if(!hasPlacedDown)
            {
                int row = Random.Range(0, 3);

                Switch (Random.Range(1, 3))
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

            for(int i = 0; i < path.count; i++)
            {
                activePathwayDown[i] = true;
            }

            onBrake = false;
        }
         */
    }

    void PlacePath(bool[] pathwayStructure, int row)
    {
        /* 
        //Todo Maby make this a switch statement.
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
           switch row  //Todo all here
           case1
           upp 1 = true
           case2
           if(random.range 1 2 == 1)
           {
              upp 0 = true
           }
           else
           {
              upp 2 = true
           }
           case3
           if(random.range 1 2 == 1)
           {
              upp 1 = true
           }
           else
           {
              upp 3 = true
           }
           case4
           upp 2 = true
        }

        if(pathwayStructure[3])
        {
           switch row     //Todo all here
           case1
           down 1 = true
           case2
           if(random.range 1 2 == 1)
           {
              down 0 = true
           }
           else
           {
              down 2 = true
           }
           case3
           if(random.range 1 2 == 1)
           {
              down 1 = true
           }
           else
           {
              down 3 = true
           }
           case4
           down 2 = true

           hasPlacedDown = true
        }
        */
    }
}
