using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [Header("Lanes")]
    public float[] lanes = new float[4];

    [Header("Input")]
    private bool[] pressedButtons = new bool[4];

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    private float tempMoveSpeed;
    private Vector2 _targetLocation = new Vector2();
    [SerializeField] private float speedAccelerator = 1.2f;
    [SerializeField] private float speedDecelerator = 1.2f;
    [SerializeField] private float speedCap = 2f;
    [SerializeField] private float uppdateFrequency = 0.02f;
    private float temporaryUppdateFrequency;
    private float currentLane = 0f;
    [SerializeField] public float bounceLeeway = 0.1f;
    [SerializeField] private float invicibility = 1f;
    private float temporaryInvicibility = 1f;
    public bool invicible = false;

    [Header("Jumping")]
    private bool isInAir = false;
    [SerializeField] private int groundInt = 6;
    [SerializeField] private int airInt = 9;
    [SerializeField] private float jumpPressDelay = 1f;
    private float[] temporaryJumpPressDelay = new float[4];
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float startHeight = 0f;
    [SerializeField] private float jumpSpeed = 1f;
    private float tempJumpSpeed;
    private bool goingUpp = false;
    private bool[] newInput = new bool[4];
    [SerializeField] private float jumpSpeedDecelerator = 0.5f;
    [SerializeField] private float jumpSpeedAccelerator = 1.5f;
    private bool jumpBrake = false;

    [Header("Dashing")]
    private bool shouldDash = false;
    [SerializeField] private float dashSpeed = 1f;
    private float tempDashSpeed = 1f;
    [SerializeField] private float dashCap = 2f;
    [SerializeField] private float dashAccelerator = 1.5f;
    [SerializeField] private float dashDecelerator = 0.5f;
    private bool canDashAgain = true;

    [Header("References")]
    private Rigidbody2D rb2d;
    private PlayerHealth playerHealth;

    private void Start()
    {
        GetLanes();
        rb2d = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();
        _targetLocation = transform.position;
        temporaryUppdateFrequency = uppdateFrequency;
    }

    void GetLanes()
    {
        GameObject[] lanesGameObject;
        lanesGameObject = GameObject.FindGameObjectsWithTag("Lane");
        for (int i = 0; i < lanesGameObject.Length; i++)
        {
            lanes[i] = lanesGameObject[i].GetComponent<Transform>().position.x;
        }
        Debug.Log(lanes[0] + "; " + lanes[1] + "; " + lanes[2] + "; " + lanes[3]);
    }

    private void Update()
    {
        CheckForInput();
        ExecuteInput();
        Move();
        Jump();
        if(temporaryInvicibility > 0)
        {
            temporaryInvicibility -= Time.deltaTime;
        }
        else
        {
            invicible = false;
        }

    }

    void CheckForInput()
    {
        pressedButtons[0] = Input.GetButton("Left");
        pressedButtons[1] = Input.GetButton("MidLeft");
        pressedButtons[2] = Input.GetButton("MidRight");
        pressedButtons[3] = Input.GetButton("Right");
    }

    void ExecuteInput()
    {
        for (int i = 0; i < pressedButtons.Length; i++)
        {
            if(temporaryJumpPressDelay[i] > 0)
            {
                temporaryJumpPressDelay[i] -= Time.deltaTime;
            }
        }

        int pressed = 0;
        for (int i = 0; i < pressedButtons.Length; i++)
        {
            if (pressedButtons[i])
            {
                pressed++;

                if (temporaryJumpPressDelay[i] > 0 && newInput[i] && !isInAir)
                {
                    isInAir = true;
                    gameObject.layer = airInt;

                    goingUpp = true;
                    tempJumpSpeed = jumpSpeed;
                    Debug.Log("Jumping");
                }

                newInput[i] = false;
                temporaryJumpPressDelay[i] = jumpPressDelay;
            }
            else
            {
                if(temporaryJumpPressDelay[i] > 0)
                {
                    newInput[i] = true;
                }
            }
        }

        if(pressed == 0)
        {
            canDashAgain = true;
        }

        if(canDashAgain)
        {
            switch (pressed)
            {
                case 1:
                    MoveBetweenLanes();
                    break;
                /*case 2:
                    DashBeweenLanes();
                    Debug.Log("Dashing");
                    break;*/
                default:
                    break;
            }
        }
    }

    void MoveBetweenLanes()
    {

        for (int i = 0; i < pressedButtons.Length; i++)
        {
            if (pressedButtons[i] && !shouldDash && lanes[i] != _targetLocation.x)
            {
                _targetLocation.x = lanes[i];
                tempMoveSpeed = moveSpeed;
                Debug.Log("Moving");
            }
        }
        
    }

    void DashBeweenLanes()
    {
        if(!shouldDash)
        {
            for (int i = 0; i < pressedButtons.Length; i++)
            {
                if (currentLane == lanes[i] && pressedButtons[i])
                {
                    shouldDash = true;
                }
            }


            for (int i = 0; i < pressedButtons.Length; i++)
            {
                if (currentLane != lanes[i] && pressedButtons[i] && shouldDash)
                {
                    _targetLocation.x = lanes[i];
                    tempDashSpeed = dashSpeed;
                    canDashAgain = false;

                    if(isInAir)
                    {
                        jumpBrake = true;
                        goingUpp = false;
                    }
                }
            }
        }
    }

    private void Move()
    {
        temporaryUppdateFrequency -= Time.deltaTime;
        if(temporaryUppdateFrequency < 0)
        {
            temporaryUppdateFrequency += uppdateFrequency;
            //Moving
            if (tempMoveSpeed < moveSpeed * speedCap)
            {
                tempMoveSpeed = tempMoveSpeed * speedAccelerator;
            }
            else
            {
                tempMoveSpeed = tempMoveSpeed * speedDecelerator;
            }
            //Jumping
            if(goingUpp && !jumpBrake)
            {
                tempJumpSpeed *= jumpSpeedDecelerator;
            }
            else if(!jumpBrake)
            {
                tempJumpSpeed *= jumpSpeedAccelerator;
            }
            //Dashing
            if (tempDashSpeed < dashSpeed * dashCap)
            {
                tempDashSpeed = tempDashSpeed * dashAccelerator;
            }
            else
            {
                tempDashSpeed = tempDashSpeed * dashDecelerator;
            }
        }

        if(!shouldDash)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(_targetLocation.x, transform.position.y), tempMoveSpeed * Time.deltaTime);
        }
        else if (shouldDash)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(_targetLocation.x, transform.position.y), tempDashSpeed * Time.deltaTime);
        }

        if(transform.position.x == _targetLocation.x)
        {
            currentLane = _targetLocation.x;
            shouldDash = false;
            jumpBrake = false;
        }
    }

    private void Jump()
    {
        if(isInAir && !jumpBrake)
        {
            if(transform.position.y != jumpHeight && goingUpp)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, jumpHeight), tempJumpSpeed * Time.deltaTime);
            }
            else
            {
                goingUpp = false;
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, startHeight), tempJumpSpeed * Time.deltaTime);
            }
            if (transform.position.y == startHeight)
            {
                isInAir = false;
                gameObject.layer = groundInt;
            }
        }
    }

    public void BouncePlayer(List<GameObject> obstacles)
    {
        //if(!invicible)
        //{
            List<float> rellevantStarts = new List<float>();
            List<float> rellevantEnds = new List<float>();
            List<int> x = new List<int>();
            bool[] obscuredRows = new bool[4];

            for (int i = 0; i < obstacles.Count; i++)
            {
                if(obstacles[i].GetComponent<Info>() == null)
                {
                    Debug.Log("oki");
                }
                rellevantStarts.Add(obstacles[i].transform.position.y - obstacles[i].GetComponent<Info>().GetLength() / 2);
                if(gameObject.layer == groundInt)
                {
                    rellevantEnds.Add(obstacles[i].transform.position.y + obstacles[i].GetComponent<Info>().GetLength() / 2);
                }
                else
                {
                    rellevantEnds.Add(obstacles[i].GetComponentInChildren<Transform>().position.y + obstacles[i].GetComponentInChildren<BoxCollider2D>().size.y / 2);
                    Debug.Log(obstacles[i].GetComponentInChildren<Obstacle>().gameObject.transform.position.y);
                    Debug.Log(obstacles[i].GetComponentInChildren<BoxCollider2D>().size.y);
                }
                x.Add(i);
            }
            Debug.Log(x.Count);
            Debug.Log(rellevantStarts[0]);
            Debug.Log(rellevantEnds[0]);

            for (int i = 0; i < rellevantStarts.Count; i++)
            {
                if (rellevantStarts[i] >= transform.position.y + bounceLeeway || rellevantEnds[i] <= transform.position.y - bounceLeeway)
                {
                    rellevantStarts.RemoveAt(i);
                    rellevantEnds.RemoveAt(i);
                    x.RemoveAt(i);
                    i--;
                }
            }
            Debug.Log(x.Count);

            if(rellevantStarts != null)
            {
                for (int i = 0; i < rellevantStarts.Count; i++)
                {
                    bool[] temporaryBoolArray = new bool[8];
                    temporaryBoolArray = obstacles[x[i]].GetComponent<Info>().GetObstacleBlock();

                    for (int j = 0; j < obscuredRows.Length; j++)
                    {
                        if (temporaryBoolArray[j + 4])
                        {
                            obscuredRows[j] = true;
                        }
                    }
                }
                Debug.Log(obscuredRows[0] + " " +
                        obscuredRows[1] + " " +
                        obscuredRows[2] + " " +
                        obscuredRows[3]);

                int y = 0;
                for (int i = 0; i < obscuredRows.Length; i++)
                {
                    if (!obscuredRows[i])
                    {
                        if (y == 0)
                        {
                            y = i;
                        }
                        else if (Mathf.Abs(transform.position.x - lanes[i]) < Mathf.Abs(transform.position.x - lanes[y]))
                        {
                            y = i;
                        }
                    }
                }
                Debug.Log(lanes[y]);

                canDashAgain = false;
                _targetLocation.x = lanes[y];
                tempMoveSpeed = moveSpeed;
            }
        //}
        



        /*float newLocation = 0;
        bool firstRight = true;

        if (_targetLocation.x > transform.position.x)
        {
            for (int i = 0; i < lanes.Length; i++)
            {

                if (lanes[i] < transform.position.x)
                {
                    if (firstRight)
                    {
                        newLocation = lanes[i];
                        firstRight = false;
                    }
                    else if (lanes[i] > newLocation)
                    {
                        newLocation = lanes[i];
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < lanes.Length; i++)
            {
                if (lanes[i] > transform.position.x)
                {
                    if (firstRight)
                    {
                        newLocation = lanes[i];
                        firstRight = false;
                    }
                    else if (lanes[i] < newLocation)
                    {
                        newLocation = lanes[i];
                    }
                }
            }
        }

        canDashAgain = false;
        _targetLocation.x = newLocation;*/
    }

    public void StartInvicibility()
    {
        temporaryInvicibility = invicibility;
        invicible = true;
    }
}
