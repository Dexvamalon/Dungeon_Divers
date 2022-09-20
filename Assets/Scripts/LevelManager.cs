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
            hasPlacedDown = false;
            
            pathLength = /random.range pathLengthMax pathLengthMin

            for(int i = 0; i < pahts.count; i++)
            {
                if(/random.range 1 placeChance == 1)
                {
                    /Switch random.range 1 5
                    case1
                    PlacePath(pathwayStructure1, i);
                    case2
                    PlacePath(pathwayStructure2, i);
                    case3
                    PlacePath(pathwayStructure3, i);
                    case4
                    PlacePath(pathwayStructure4, i);
                    case5
                    PlacePath(pathwayStructure5, i);
                }
            }
            if(!has placed down)
            {
                int row = random.range 0 3;

                /Switch random.range 1 3
                case1
                PlacePath(pathwayStructure1, row);
                case2
                PlacePath(pathwayStructure3, row);
                case3
                PlacePath(pathwayStructure5, row);
            }
            onBrake = true;
        }
        else
        {
            pathLength = /random.range pauseLengthMax pauseLengthMin

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
        if(0)
        {
           upp 0+(row) = true
        }
        if(2)
        {
           down 0+(row) = true
           hasPlacedDown = true
        }
        if(1)
        {
           switch row
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
        if(3)
        {
           switch row
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
