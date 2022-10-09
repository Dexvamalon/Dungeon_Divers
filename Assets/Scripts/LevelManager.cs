using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private List<Transform> obstacles = new List<Transform>();
    [SerializeField] private float levelSpeed = 1f;
    public bool isDead = false;
    public int score = 0;
    private int delayedScore = 0;
    [SerializeField] private float uppdateScoreDalay = 0;
    private float temporary = 0;
    private UI ui;

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
    private List<int> amountPlacedOfObstacle = new List<int>();
    private List<float> temporaryPrefabLength = new List<float>();
    private List<int> temporaryPrefabType = new List<int>();
    private List<int> typeCount = new List<int>();

    private List<Transform> obstacleParentsList = new List<Transform>();
    private Transform obstacleParent;
    // postion of the end of a segment, empty game object with the obstacles within, 
    [SerializeField] private float placePos = 10f;
    [SerializeField] private float deletePos = -10f;
    [SerializeField] private int maxObstacles = 5;
    [SerializeField] private int maxObstaclesPause = 3;


    private List<float>[] starts = new List<float>[8];
    private List<float>[] ends = new List<float>[8];

    GameObject player;
    [SerializeField] private float seThroughOpacity = 1f;

    [SerializeField] private int maxObstacleOfType = 2;

    [SerializeField] private List<int> maxOfType = new List<int>();

    List<bool> type0 = new List<bool>();
    List<bool> type1 = new List<bool>();
    List<bool> type2 = new List<bool>();

    [SerializeField] private float levelDificulty = 1f;
    [SerializeField] private float cameraZoomSpeed = 1f;




    private void Start()
    {
        paths = FindObjectOfType<PlayerMovement>().lanes;
        player = GameObject.FindGameObjectWithTag("Player");
        ui = FindObjectOfType<UI>();
        temporary = uppdateScoreDalay;
        /*obstacleParent = new GameObject("Obstacle parent").transform;
        obstacleParentsList.Add(obstacleParent);
        Instantiate(prefabs[1], new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 0), obstacleParent.transform);
        */
        Camera.main.orthographicSize = 5;
        Camera.main.transform.position = new Vector3(0, 2, -100);

    }

    void Update()
    {
        if(score > 50)
        {
            levelDificulty = (float)score / 50;
            Debug.Log(levelDificulty + " " + ((float)score / 50));
        }

        for(int i = 0; i < obstacleParentsList.Count; i++)
        {
            if(!isDead)
            {
                obstacleParentsList[i].position -= new Vector3(0, levelSpeed * Time.deltaTime, 0);
            }

            for (int j = 0; j < obstacleParentsList[i].childCount; j++)
            {
                Transform temporaryTransform;
                temporaryTransform = obstacleParentsList[i].GetChild(j).Find("Sprite");
                //Debug.Log(temporaryTransform.gameObject.name);

                Color color = temporaryTransform.gameObject.GetComponent<SpriteRenderer>().color;
                //Debug.Log((temporaryTransform.position.y + temporaryTransform.localScale.y / 2) + " " + (temporaryTransform.position.y - temporaryTransform.localScale.y / 2));
                //Debug.Log((temporaryTransform.position.x + temporaryTransform.localScale.x / 2) + " " + (temporaryTransform.position.x - temporaryTransform.localScale.x / 2));
                if (temporaryTransform.position.y + temporaryTransform.localScale.y / 2 >= player.transform.position.y &&
                    temporaryTransform.position.x + temporaryTransform.localScale.x / 2 >= player.transform.position.x &&
                    temporaryTransform.position.y - temporaryTransform.localScale.y / 2 <= player.transform.position.y &&
                    temporaryTransform.position.x - temporaryTransform.localScale.x / 2 <= player.transform.position.x)
                {

                    color.a = seThroughOpacity;
                    temporaryTransform.gameObject.GetComponent<SpriteRenderer>().color = color;
                    //Debug.Log("happened1");
                }
                else
                {
                    color.a = 1;
                    temporaryTransform.gameObject.GetComponent<SpriteRenderer>().color = color;
                }

                temporaryTransform.parent.transform.position = new Vector3(temporaryTransform.parent.transform.position.x, 
                                                                            temporaryTransform.parent.transform.position.y, 
                                                                            obstacleParentsList[i].GetChild(j).transform.position.y);
            }
            if (obstacleParentsList[i].position.y < deletePos)
            {
                Destroy(obstacleParentsList[i].gameObject);
                obstacleParentsList.RemoveAt(i);
                i--;
            }
        }

        if(obstacleParent == null)
        {
            PlacePathway();
        }
        else if(obstacleParent.position.y < placePos)
        {
            PlacePathway();
        }

        temporary -= Time.deltaTime;
        if(temporary <= 0)
        {
            temporary += uppdateScoreDalay;
            if (score > delayedScore)
            {
                delayedScore++;
                ui.SetStats(-1, delayedScore);
            }
        }
        float cameraOrthograpicSize = 1f;
        float cameraPosition = 1f;
        bool canZoom = false;

        cameraOrthograpicSize = 4.5f + levelDificulty / 2;
        cameraPosition = 1.5f + levelDificulty / 2;
        

        if (Camera.main.orthographicSize + 1 < cameraOrthograpicSize || canZoom)
        {
            canZoom = true;
            Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, cameraOrthograpicSize, cameraZoomSpeed * Time.deltaTime);
        }
        if(Camera.main.orthographicSize >= cameraOrthograpicSize)
        {
            canZoom = false;
        }
        if (Camera.main.transform.position.y < cameraPosition)
        {
            Camera.main.transform.position = new Vector3(0, Camera.main.transform.position.y + Time.deltaTime/5, -100);
        }
        
        levelSpeed = 6 + levelDificulty;
    }



    void PlacePathway()
    {
        //score++;
        //ui.SetStats(-1, score);
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

        //Debug.Log("up" + activePathwayUp[0] + " " + activePathwayUp[1] + " " + activePathwayUp[2] + " " + activePathwayUp[3]);
        //Debug.Log("down" + activePathwayDown[0] + " " + activePathwayDown[1] + " " + activePathwayDown[2] + " " + activePathwayDown[3]);

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
        List<int> intToRemove = new List<int>();

        List<int> temp = new List<int>();
        List<int> temp1 = new List<int>();
        List<int> temp2 = new List<int>();

        for (int i = 0; i < ends.Length; i++)
        {
            ends[i] = new List<float>();
            starts[i] = new List<float>();
        }

        obscuredPath = new bool[8];

        float yPos;

        FindWorkingObstacles();

        obstacleParent = new GameObject("Obstacle parent").transform;
        obstacleParent.position = new Vector3(0, placePos + pathLength, 0);
        obstacleParentsList.Add(obstacleParent);

        //Debug.Log(pathLength);

        if (temporaryPrefabVariants.Count > 0)
        {
            //place them
            //pick random of the available ones
            int x = 0;
            temp = new List<int>();
            temp1 = new List<int>();
            temp2 = new List<int>();
            for (int b = 0; b < type0.Count; b++)
            {
                if (type0[b])
                {
                    temp.Add(b);
                }
                if (type1[b])
                {
                    temp1.Add(b);
                }
                if (type2[b])
                {
                    temp2.Add(b);
                }
            }
            if (temp1.Count > 0)
            {
                x = Random.Range(0, temp1.Count);
                x = temp1[x];
            }
            else if (temp.Count > 0)
            {
                x = Random.Range(0, temp.Count);
                x = temp[x];
            }
            else if (temp2.Count > 0)
            {
                x = Random.Range(0, temp2.Count);
                x = temp2[x];
            }
            else
            {
                //Debug.Log("x nothing");
                return;
            }

            yPos = Random.Range(obstacleParent.position.y - pathLength + temporaryPrefabVariants[x].transform.lossyScale.y / 2,
                                obstacleParent.position.y - temporaryPrefabVariants[x].transform.lossyScale.y / 2);

            Instantiate(temporaryPrefabVariants[x], new Vector3(temporaryPrefabVariants[x].GetComponent<Info>().GetBlockPosition(), yPos, 0), Quaternion.Euler(0, 0, 0), obstacleParent.transform);
            //Debug.Log("instantiate " + temporaryPrefabType[x]);
            /*Debug.Log("//// " + obstacleVariantBlock[x][0] + " " +
                                  obstacleVariantBlock[x][1] + " " +
                                  obstacleVariantBlock[x][2] + " " +
                                  obstacleVariantBlock[x][3] + " " +
                                  obstacleVariantBlock[x][4] + " " +
                                  obstacleVariantBlock[x][5] + " " +
                                  obstacleVariantBlock[x][6] + " " +
                                  obstacleVariantBlock[x][7]);*/
            score++;
            amountPlacedOfObstacle[x]++;
            typeCount[temporaryPrefabType[x]]++;
            
            FindAvilableYSpace(yPos - temporaryPrefabLength[x] / 2, yPos + temporaryPrefabLength[x] / 2, x);

            for (int i = 0; i < obscuredPath.Length; i++)
            {
                obscuredPath[i] = obstacleVariantBlock[x][i];
            }

            /*temporaryPrefabVariants.RemoveAt(x);
            obstacleVariantBlock.RemoveAt(x);
            temporaryPrefabLength.RemoveAt(x);*/

            for (int i = 0; i < obstacleVariantBlock.Count; i++)
            {
                /*Debug.Log("x" + obstacleVariantBlock[i][0] + " " +
                          obstacleVariantBlock[i][1] + " " +
                          obstacleVariantBlock[i][2] + " " +
                          obstacleVariantBlock[i][3] + " " +
                          obstacleVariantBlock[i][4] + " " +
                          obstacleVariantBlock[i][5] + " " +
                          obstacleVariantBlock[i][6] + " " +
                          obstacleVariantBlock[i][7]);*/
            }
            //Debug.Log("break");

            intToRemove = new List<int>();
            if (typeCount[temporaryPrefabType[x]] >= maxOfType[temporaryPrefabType[x]])
            {
                //Debug.Log("remove " + temporaryPrefabType[x]);
                for (int i = 0; i < obstacleVariantBlock.Count; i++)
                {
                    if (temporaryPrefabType[i] == temporaryPrefabType[x])
                    {
                        intToRemove.Add(i);
                    }
                }
            }
            else if (amountPlacedOfObstacle[x] >= maxObstacleOfType)
            {
                intToRemove.Add(x);
            }
            intToRemove.Sort();
            intToRemove.Reverse();
            for(int i = 0; i < intToRemove.Count; i++)
            {
                temporaryPrefabVariants.RemoveAt(intToRemove[i]);
                obstacleVariantBlock.RemoveAt(intToRemove[i]);
                temporaryPrefabLength.RemoveAt(intToRemove[i]);
                amountPlacedOfObstacle.RemoveAt(intToRemove[i]);
                temporaryPrefabType.RemoveAt(intToRemove[i]);
                type0.RemoveAt(intToRemove[i]);
                type1.RemoveAt(intToRemove[i]);
                type2.RemoveAt(intToRemove[i]);
            }


            if (temporaryPrefabVariants.Count <= 0)
            {
                //Debug.Log("x nothing");
                return;
            }
            // 
            int curMaxObstacles = 0;
            if (!onBrake)
            {
                curMaxObstacles = maxObstaclesPause;
            }
            else
            {
                curMaxObstacles = maxObstacles;
            }
            int y = 0;
            for (int a = 0; a < curMaxObstacles-1; a++)
            {
                List<int> curObstacleBlock = new List<int>();
                bool colliding = false;
                usedEnds = new List<float>();
                usedStarts = new List<float>();

                //pick random other that would work
                
                temp = new List<int>();
                temp1 = new List<int>();
                temp2 = new List<int>();
                for (int b = 0; b < type0.Count; b++)
                {
                    if (type0[b])
                    {
                        temp.Add(b);
                    }
                    if(type1[b])
                    {
                        temp1.Add(b);
                    }
                    if (type2[b])
                    {
                        temp2.Add(b);
                    }
                }
                if (temp1.Count > 0)
                {
                    x = Random.Range(0, temp1.Count);
                    x = temp1[x];
                }
                else if (temp.Count > 0)
                {
                    x = Random.Range(0, temp.Count);
                    x = temp[x];
                }
                else if (temp2.Count > 0)
                {
                    x = Random.Range(0, temp2.Count);
                    x = temp2[x];
                }
                else
                {
                    //Debug.Log("x nothing");
                    return;
                }
                for (int i = 0; i < obscuredPath.Length; i++)
                {
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
                        }
                    }

                    for (int j = 0; j < curObstacleBlock.Count; j++)
                    {
                        for (int k = 0; k < ends[curObstacleBlock[j]].Count; k++)
                        {
                            if (ends[curObstacleBlock[j]][k] - starts[curObstacleBlock[j]][k] > temporaryPrefabLength[x])
                            {
                                workingEnds[curObstacleBlock[j]].Add(ends[curObstacleBlock[j]][k]);
                                workingStarts[curObstacleBlock[j]].Add(starts[curObstacleBlock[j]][k]);
                                //Debug.Log("1 Removed");
                            }
                        }
                        /*Debug.Log("//// " + obstacleVariantBlock[x][0] + " " +
                                  obstacleVariantBlock[x][1] + " " +
                                  obstacleVariantBlock[x][2] + " " +
                                  obstacleVariantBlock[x][3] + " " +
                                  obstacleVariantBlock[x][4] + " " +
                                  obstacleVariantBlock[x][5] + " " +
                                  obstacleVariantBlock[x][6] + " " +
                                  obstacleVariantBlock[x][7]);*/
                        //Debug.Log(curObstacleBlock[j] + " " + workingEnds[curObstacleBlock[j]].Count);
                        //Debug.Log(curObstacleBlock[j] + " " + workingStarts[curObstacleBlock[j]].Count);
                    }

                    if (curObstacleBlock.Count >= 2)
                    {
                        for (int j = 0; j < workingEnds[curObstacleBlock[0]].Count; j++)
                        {
                            for (int k = 0; k < workingEnds[curObstacleBlock[1]].Count; k++)
                            {
                                //Debug.Log("got here");
                                if (workingEnds[curObstacleBlock[1]][k] <= workingEnds[curObstacleBlock[0]][j] && workingStarts[curObstacleBlock[1]][k] >= workingStarts[curObstacleBlock[0]][j])
                                {
                                    usedStarts.Add(workingStarts[curObstacleBlock[1]][k]);
                                    usedEnds.Add(workingEnds[curObstacleBlock[1]][k]);
                                }
                                else if (workingStarts[curObstacleBlock[1]][k] > workingStarts[curObstacleBlock[0]][j] && workingStarts[curObstacleBlock[1]][k] < workingEnds[curObstacleBlock[0]][j] && workingEnds[curObstacleBlock[1]][k] >= workingEnds[curObstacleBlock[0]][j])
                                {
                                    usedStarts.Add(workingStarts[curObstacleBlock[1]][k]);
                                    usedEnds.Add(workingEnds[curObstacleBlock[0]][j]);
                                }
                                else if (workingEnds[curObstacleBlock[1]][k] > workingStarts[curObstacleBlock[0]][j] && workingEnds[curObstacleBlock[1]][k] < workingEnds[curObstacleBlock[0]][j] && workingStarts[curObstacleBlock[1]][k] <= workingStarts[curObstacleBlock[0]][j])
                                {
                                    usedStarts.Add(workingStarts[curObstacleBlock[0]][j]);
                                    usedEnds.Add(workingEnds[curObstacleBlock[1]][k]);
                                }
                                else if (workingStarts[curObstacleBlock[1]][k] <= workingStarts[curObstacleBlock[0]][j] && workingEnds[curObstacleBlock[1]][k] >= workingEnds[curObstacleBlock[0]][j])
                                {
                                    usedStarts.Add(workingStarts[curObstacleBlock[0]][j]);
                                    usedEnds.Add(workingEnds[curObstacleBlock[0]][j]);
                                }
                            }
                        }
                        //Debug.Log("2 Removed");
                        //Debug.Log("1 " + usedStarts.Count);

                        for (int j = 0; j < curObstacleBlock.Count - 2; j++)//j
                        {
                            for (int t = 0; t < usedStarts.Count; t++)//t
                            {
                                for (int k = 0; k < workingEnds[curObstacleBlock[j + 2]].Count; k++)//k
                                {

                                    if (workingEnds[curObstacleBlock[j + 2]][k] <= usedEnds[t] && workingStarts[curObstacleBlock[j + 2]][k] >= usedStarts[t])
                                    {
                                        temporaryStarts.Add(workingStarts[curObstacleBlock[j + 2]][k]);
                                        temporaryEnds.Add(workingEnds[curObstacleBlock[j + 2]][k]);
                                    }
                                    else if (workingStarts[curObstacleBlock[j + 2]][k] > usedStarts[t] && workingStarts[curObstacleBlock[j + 2]][k] < usedEnds[t] && workingEnds[curObstacleBlock[j + 2]][k] >= usedEnds[t])
                                    {
                                        temporaryStarts.Add(workingStarts[curObstacleBlock[j + 2]][k]);
                                        temporaryEnds.Add(usedEnds[t]);
                                    }
                                    else if (workingEnds[curObstacleBlock[j + 2]][k] > usedStarts[t] && workingEnds[curObstacleBlock[j + 2]][k] < usedEnds[t] && workingStarts[curObstacleBlock[j + 2]][k] <= usedStarts[t])
                                    {
                                        temporaryStarts.Add(usedStarts[t]);
                                        temporaryEnds.Add(workingEnds[curObstacleBlock[j + 2]][k]);
                                    }
                                    else if (workingStarts[curObstacleBlock[j + 2]][k] <= usedStarts[t] && workingEnds[curObstacleBlock[j + 2]][k] >= usedEnds[t])
                                    {
                                        temporaryStarts.Add(usedStarts[t]);
                                        temporaryEnds.Add(usedEnds[t]);
                                    }
                                }
                            }
                            //Debug.Log("2 " + temporaryStarts.Count);

                            usedStarts = temporaryStarts;
                            usedEnds = temporaryEnds;
                            temporaryStarts = new List<float>();
                            temporaryEnds = new List<float>();

                            //Debug.Log("3 Removed");
                        }
                    }
                    else
                    {
                        for (int j = 0; j < workingStarts[curObstacleBlock[0]].Count; j++)
                        {
                            usedStarts.Add(workingStarts[curObstacleBlock[0]][j]);
                            usedEnds.Add(workingEnds[curObstacleBlock[0]][j]);
                        }
                    }
                    
                    for (int i = 0; i < usedStarts.Count; i++)
                    {
                        //Debug.Log(usedStarts[i]);
                        //Debug.Log(usedEnds[i]);
                    }

                    for (int j = 0; j < usedStarts.Count; j++)
                    {
                        if (usedEnds[j] - usedStarts[j] < temporaryPrefabLength[x])
                        {
                            usedEnds.RemoveAt(j);
                            usedStarts.RemoveAt(j);
                            j--;
                            //Debug.Log("4 Removed");
                        }
                    }

                    if (usedStarts.Count > 0)
                    {
                        int z = Random.Range(0, usedStarts.Count);
                        yPos = Random.Range(usedStarts[z] + temporaryPrefabVariants[x].transform.lossyScale.y / 2,
                                            usedEnds[z] - temporaryPrefabVariants[x].transform.lossyScale.y / 2);
                        Instantiate(temporaryPrefabVariants[x], new Vector3(temporaryPrefabVariants[x].GetComponent<Info>().GetBlockPosition(), yPos, 0), Quaternion.Euler(0, 0, 0), obstacleParent.transform);
                        //Debug.Log("instantiate " + temporaryPrefabType[x]);
                        /*Debug.Log("//// " + obstacleVariantBlock[x][0] + " " +
                                  obstacleVariantBlock[x][1] + " " +
                                  obstacleVariantBlock[x][2] + " " +
                                  obstacleVariantBlock[x][3] + " " +
                                  obstacleVariantBlock[x][4] + " " +
                                  obstacleVariantBlock[x][5] + " " +
                                  obstacleVariantBlock[x][6] + " " +
                                  obstacleVariantBlock[x][7]);*/
                        a++;
                        score++;
                        amountPlacedOfObstacle[x]++;
                        typeCount[temporaryPrefabType[x]]++;
                        //Debug.Log(yPos);
                        FindAvilableYSpace(yPos - temporaryPrefabLength[x] / 2, yPos + temporaryPrefabLength[x] / 2, x);

                        for (int i = 0; i < obscuredPath.Length; i++)
                        {
                            if (obstacleVariantBlock[x][i])
                            {
                                obscuredPath[i] = true;
                            }
                        }

                        intToRemove = new List<int>();
                        if (typeCount[temporaryPrefabType[x]] >= maxOfType[temporaryPrefabType[x]])
                        {
                            //Debug.Log("remove " + temporaryPrefabType[x]);
                            for (int i = 0; i < obstacleVariantBlock.Count; i++)
                            {
                                if (temporaryPrefabType[i] == temporaryPrefabType[x])
                                {
                                    intToRemove.Add(i);
                                }
                            }
                        }
                        else if (amountPlacedOfObstacle[x] >= maxObstacleOfType)
                        {
                            intToRemove.Add(x);
                        }
                        intToRemove.Sort();
                        intToRemove.Reverse();
                        for (int i = 0; i < intToRemove.Count; i++)
                        {
                            temporaryPrefabVariants.RemoveAt(intToRemove[i]);
                            obstacleVariantBlock.RemoveAt(intToRemove[i]);
                            temporaryPrefabLength.RemoveAt(intToRemove[i]);
                            amountPlacedOfObstacle.RemoveAt(intToRemove[i]);
                            temporaryPrefabType.RemoveAt(intToRemove[i]);
                            type0.RemoveAt(intToRemove[i]);
                            type1.RemoveAt(intToRemove[i]);
                            type2.RemoveAt(intToRemove[i]);
                        }

                        if (temporaryPrefabVariants.Count <= 0)
                        {
                            //Debug.Log("x nothing");
                            return;
                        }
                    }
                    else
                    {
                        temporaryPrefabVariants.RemoveAt(x);
                        obstacleVariantBlock.RemoveAt(x);
                        temporaryPrefabLength.RemoveAt(x);
                        amountPlacedOfObstacle.RemoveAt(x);
                        temporaryPrefabType.RemoveAt(x);
                        type0.RemoveAt(x);
                        type1.RemoveAt(x);
                        type2.RemoveAt(x);
                    }

                    if (temporaryPrefabVariants.Count <= 0)
                    {
                        //Debug.Log("x nothing");
                        a--;
                        //Debug.Log("//////////////////" + (curMaxObstacles - 2 - a));
                        return;
                    }
                    /*
                    temporaryPrefabVariants.RemoveAt(x);
                    obstacleVariantBlock.RemoveAt(x);
                    temporaryPrefabLength.RemoveAt(x);*/

                    for (int i = 0; i < obstacleVariantBlock.Count; i++)
                    {
                        /*Debug.Log("x" + obstacleVariantBlock[i][0] + " " +
                                  obstacleVariantBlock[i][1] + " " +
                                  obstacleVariantBlock[i][2] + " " +
                                  obstacleVariantBlock[i][3] + " " +
                                  obstacleVariantBlock[i][4] + " " +
                                  obstacleVariantBlock[i][5] + " " +
                                  obstacleVariantBlock[i][6] + " " +
                                  obstacleVariantBlock[i][7]);*/
                    }
                    //Debug.Log("break");

                    /*if (temporaryPrefabVariants.Count <= 0)
                    {
                        Debug.Log("x nothing");
                        a--;
                        Debug.Log("//////////////////" + (curMaxObstacles - 2 - a));
                        return;
                    }*/
                }
                else
                {
                    yPos = Random.Range(obstacleParent.position.y - pathLength + temporaryPrefabVariants[x].transform.lossyScale.y / 2,
                                    obstacleParent.position.y - temporaryPrefabVariants[x].transform.lossyScale.y / 2);
                    Instantiate(temporaryPrefabVariants[x], new Vector3(temporaryPrefabVariants[x].GetComponent<Info>().GetBlockPosition(), yPos, 0), Quaternion.Euler(0, 0, 0), obstacleParent.transform);
                    //Debug.Log("instantiate " + temporaryPrefabType[x]);
                    /*Debug.Log("//// " + obstacleVariantBlock[x][0] + " " +
                                  obstacleVariantBlock[x][1] + " " +
                                  obstacleVariantBlock[x][2] + " " +
                                  obstacleVariantBlock[x][3] + " " +
                                  obstacleVariantBlock[x][4] + " " +
                                  obstacleVariantBlock[x][5] + " " +
                                  obstacleVariantBlock[x][6] + " " +
                                  obstacleVariantBlock[x][7]);*/
                    a++;
                    score++;
                    //Debug.Log(yPos);
                    amountPlacedOfObstacle[x]++;
                    typeCount[temporaryPrefabType[x]]++;

                    FindAvilableYSpace(yPos - temporaryPrefabLength[x] / 2, yPos + temporaryPrefabLength[x] / 2, x);

                    for (int i = 0; i < obscuredPath.Length; i++)
                    {
                        if (obstacleVariantBlock[x][i])
                        {
                            obscuredPath[i] = true;
                        }
                    }

                    intToRemove = new List<int>();
                    if (typeCount[temporaryPrefabType[x]] >= maxOfType[temporaryPrefabType[x]])
                    {
                        //Debug.Log("remove " + temporaryPrefabType[x]);
                        for (int i = 0; i < obstacleVariantBlock.Count; i++)
                        {
                            if (temporaryPrefabType[i] == temporaryPrefabType[x])
                            {
                                intToRemove.Add(i);
                            }
                        }
                    }
                    else if (amountPlacedOfObstacle[x] >= maxObstacleOfType)
                    {
                        intToRemove.Add(x);
                    }
                    intToRemove.Sort();
                    intToRemove.Reverse();
                    for (int i = 0; i < intToRemove.Count; i++)
                    {
                        temporaryPrefabVariants.RemoveAt(intToRemove[i]);
                        obstacleVariantBlock.RemoveAt(intToRemove[i]);
                        temporaryPrefabLength.RemoveAt(intToRemove[i]);
                        amountPlacedOfObstacle.RemoveAt(intToRemove[i]);
                        temporaryPrefabType.RemoveAt(intToRemove[i]);
                        type0.RemoveAt(intToRemove[i]);
                        type1.RemoveAt(intToRemove[i]);
                        type2.RemoveAt(intToRemove[i]);
                    }

                    if (temporaryPrefabVariants.Count <= 0)
                    {
                        //Debug.Log("x nothing");
                        return;
                    }
                    //a++;

                    /*temporaryPrefabVariants.RemoveAt(x);
                    obstacleVariantBlock.RemoveAt(x);
                    temporaryPrefabLength.RemoveAt(x);*/

                    for (int i = 0; i < obstacleVariantBlock.Count; i++)
                    {
                        /*Debug.Log("x" + obstacleVariantBlock[i][0] + " " +
                                  obstacleVariantBlock[i][1] + " " +
                                  obstacleVariantBlock[i][2] + " " +
                                  obstacleVariantBlock[i][3] + " " +
                                  obstacleVariantBlock[i][4] + " " +
                                  obstacleVariantBlock[i][5] + " " +
                                  obstacleVariantBlock[i][6] + " " +
                                  obstacleVariantBlock[i][7]);*/
                    }
                    //Debug.Log("break");
                }

                a--;
                y++;
                if (y > 50)
                {
                    Debug.Break(); //todo There's a infinite loop somewhere in the max obstacle loop.
                    return;
                }
                //Debug.Log("//////////////////" + (curMaxObstacles - 2 - a));
            }
        }
        for (int i = 0; i < obstacleVariantBlock.Count; i++)
        {
            /*Debug.Log("x" + obstacleVariantBlock[i][0] + " " +
                      obstacleVariantBlock[i][1] + " " +
                      obstacleVariantBlock[i][2] + " " +
                      obstacleVariantBlock[i][3] + " " +
                      obstacleVariantBlock[i][4] + " " +
                      obstacleVariantBlock[i][5] + " " +
                      obstacleVariantBlock[i][6] + " " +
                      obstacleVariantBlock[i][7]);*/
        }
    }

    void FindAvilableYSpace(float newObstacleStart, float newObstacleEnd, int x)
    {
        //Debug.Log("new obstacle start " + newObstacleStart + " new obstacle end " + newObstacleEnd);
        //var
        //array of lists (ends)
        //array of lists (starts)

        int var = 0;


        //Set to default (when starting to place a new pathway)

        for (int j = 0; j < ends.Length; j++)
        {
            if (ends[j].Count <= 0)
            {
                ends[j].Add(obstacleParent.position.y);
                starts[j].Add(obstacleParent.position.y - pathLength);
            }

            if (obstacleVariantBlock[x][j])
            {
                ////fixing arrays
                //fing biggest ends thats smaller than new obstacle start
                var = 0;
                for (int i = 0; i < ends[j].Count; i++)
                {
                    if (starts[j][var] > newObstacleStart && starts[j][i] < newObstacleStart)
                    {
                        var = i;
                    }
                    else if (starts[j][i] > starts[j][var] && starts[j][i] <= newObstacleStart)
                    {
                        var = i;
                    }
                }
                //Debug.Log("biggest start smaller than new start " + starts[j][var]);
                //if end of same index > new obstacle start
                //end = new obstacle start  
                if (ends[j][var] >= newObstacleStart)
                {
                    ends[j].Add(ends[j][var]);
                    starts[j].Add(newObstacleEnd);
                    ends[j][var] = newObstacleStart;
                }

                //find smallest starts thats bigger than new obstacle end
                var = 0;
                for (int i = 0; i < starts[j].Count; i++)
                {
                    if (ends[j][var] < newObstacleEnd && ends[j][i] > newObstacleEnd)
                    {
                        var = i;
                    }
                    else if (ends[j][i] < ends[j][var] && ends[j][i] >= newObstacleEnd)
                    {
                        var = i;
                    }
                }

                //if star of same index < new obstacle end
                //start = new obstacle end

                if (starts[j][var] <= newObstacleEnd)
                {
                    ends[j].Add(newObstacleStart);
                    starts[j].Add(starts[j][var]);
                    starts[j][var] = newObstacleEnd;
                }



                for(int i = 0; i < ends[j].Count; i++)
                {
                    if (ends[j][i] >= newObstacleStart && ends[j][i] <= newObstacleEnd && starts[j][i] >= newObstacleStart && starts[j][i] <= newObstacleEnd)
                    {
                        ends[j].RemoveAt(i);
                        starts[j].RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        for (int i = 0; i < ends.Length; i++)
        {
            for (int j = 0; j < ends[i].Count; j++)
            {
                if (obstacleVariantBlock[x][i])
                {
                    //Debug.Log("starts " + i + " " + j + " " + starts[i][j]);
                    //Debug.Log("ends " + i + " " + j + " " + ends[i][j]);
                }
            }
        }
    }

    void FindWorkingObstacles()
    {
        obstacleVariantBlock = new List<bool[]>();
        temporaryPrefabLength = new List<float>();
        temporaryPrefabVariants = new List<GameObject>();
        amountPlacedOfObstacle = new List<int>();
        temporaryPrefabType = new List<int>();
        typeCount = new List<int>() { 0, 0, 0 };
        type0 = new List<bool>();
        type1 = new List<bool>();
        type2 = new List<bool>();

        for (int i = 0; i < prefabs.Count; i++)
        {
            obstacleVariantBlock.Add(prefabs[i].GetComponent<Info>().GetObstacleBlock());
            temporaryPrefabLength.Add(prefabs[i].GetComponent<Info>().GetLength());
            temporaryPrefabVariants.Add(prefabs[i]);
            amountPlacedOfObstacle.Add(0);
            temporaryPrefabType.Add(prefabs[i].GetComponent<Info>().GetObstacleType());
            type0.Add(false);
            type1.Add(false);
            type2.Add(false);
        }
        for(int i = 0; i < temporaryPrefabType.Count; i++)
        {
            switch (temporaryPrefabType[i])
            {
                case 0:
                    type0[i] = true;
                    break;
                case 1:
                    type1[i] = true;
                    break;
                case 2:
                    type2[i] = true;
                    break;
                default:
                    break;

            }
        }
        for (int i = 0; i < temporaryPrefabVariants.Count; i++)
        {
            if(temporaryPrefabLength[i] > pathLength)
            {
                temporaryPrefabVariants.RemoveAt(i);
                obstacleVariantBlock.RemoveAt(i);
                temporaryPrefabLength.RemoveAt(i);
                amountPlacedOfObstacle.RemoveAt(i);
                temporaryPrefabType.RemoveAt(i);
                type0.RemoveAt(i);
                type1.RemoveAt(i);
                type2.RemoveAt(i);
                i--;
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
                        amountPlacedOfObstacle.RemoveAt(x);
                        temporaryPrefabType.RemoveAt(x);
                        type0.RemoveAt(x);
                        type1.RemoveAt(x);
                        type2.RemoveAt(x);
                        x--;
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
                        amountPlacedOfObstacle.RemoveAt(x);
                        temporaryPrefabType.RemoveAt(x);
                        type0.RemoveAt(x);
                        type1.RemoveAt(x);
                        type2.RemoveAt(x);
                        x--;
                    }
                }
            }
        }
        for(int i = 0; i < obstacleVariantBlock.Count; i++)
        {
            /*Debug.Log("i" + obstacleVariantBlock[i][0] + " " +
                      obstacleVariantBlock[i][1] + " " +
                      obstacleVariantBlock[i][2] + " " +
                      obstacleVariantBlock[i][3] + " " +
                      obstacleVariantBlock[i][4] + " " +
                      obstacleVariantBlock[i][5] + " " +
                      obstacleVariantBlock[i][6] + " " +
                      obstacleVariantBlock[i][7]);*/
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
