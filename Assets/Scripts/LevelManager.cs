using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private List<Transform> obstacles = new List<Transform>();
    [SerializeField] private float levelSpeed = 1f;

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

        [Header("Pathway variables")]
        private bool[] activePathwayUp = new bool[4];
        private bool[] activePathwayDown = new bool[4];

        private bool onBrake = false;
        private bool hasPlacedDown = false;
        //Todo (on place down = true)

        [Serializefield] private float pathLengthMax = 1f;
        [Serializefield] private float pathLengthMin = 1f;

        [Serializefield] private float pauseLengthMax = 1f;
        [Serializefield] private float pauseLengthMin = 1f;

        private float pathLength = 1f;

        [Serializefield] private float placeChance = 1f; //(chance = 1/placeChance)

        [Serializefield] private bool[] pathwayStructure1 = new bool[4]; // 2 high
        [Serializefield] private bool[] pathwayStructure2 = new bool[4]; // single up
        [Serializefield] private bool[] pathwayStructure3 = new bool[4]; // single down
        [Serializefield] private bool[] pathwayStructure4 = new bool[4]; // double up
        [Serializefield] private bool[] pathwayStructure5 = new bool[4]; // double down
        //(bool array of 4 units 2 upp 2 down)


        if(!onBrake)
        {
            pathLength = /random.range pathLengthMax pathLengthMin

            for(int i = 0; i < pahts.count; i++)
            {
                if(/random.range 1 placeChance == 1)
                {
                    Switch random.range 1 5
                    case1
                    PlacePath(pathwayStructure1);
                    case2
                    PlacePath(pathwayStructure2);
                    case3
                    PlacePath(pathwayStructure3);
                    case4
                    PlacePath(pathwayStructure4);
                    case5
                    PlacePath(pathwayStructure5);

                    (if 4/5                  )
                    (switch path count       )
                    (case 1 place next square)  // on PlacePath method.
                    (case 2-3 random         )
                    (case 4 place prev square)
                }
            }
            if(!has placed down)
            {
                Switch random.range 1 3
                case1
                PlacePath(pathwayStructure1);
                case2
                PlacePath(pathwayStructure3);
                case3
                PlacePath(pathwayStructure5);

                (if 3                    )
                (switch path count       )
                (case 1 place next square)  // on PlacePath method.
                (case 2-3 random         )
                (case 4 place prev square)
            }
            onBrake = true;
        }
        else
        {
            random length

            place path on all down
            onBrake = false;
        }

         */
    }
}
