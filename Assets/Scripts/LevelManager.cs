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

        var
        Pathway slots:
        bool Array up
        bool Array down

        bool onBrake
        bool hasPlacedDown

        pathLengthMax
        pathLengthMin

        placeChance //(chance = 1/placeChance)

        [Serializefield] pathwayStructure x5 (bool array of 4 units 2 upp 2 down)
        [Serializefield] pathwayCount


        if(!on brake)
        {
            random length

            bool on brake = false

            has placed down = false
            (on place down = true)

            for(pahts.count)
            {
                random between 1 and x
                if 1 place
                {
                    random between 1 and 5     // Todo create vaiables from here and refine sudo code.
                    place respective path
                    if 4/5
                    switch path count
                    case 1 place next square
                    case 2-3 random
                    case 4 place prev square
                }
            }
            if !has placed down
            {
                random between 1 and 3 (down ones)
                place respective path
                if 3
                switch path count
                case 1 place next square
                case 2-3 random
                case 4 place prev square
            }
            onbrake = true
        }
        else
        {
            random length

            place path on all down
        }

         */
    }
}
