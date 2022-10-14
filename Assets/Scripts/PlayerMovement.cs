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
    //[SerializeField] private float speedDecelerator = 1.2f;
    //[SerializeField] private float speedCap = 2f;
    [SerializeField] private float uppdateFrequency = 0.02f;
    private float temporaryUppdateFrequency;
    private int currentLane = 1;
    [SerializeField] public float bounceLeeway = 0.1f;
    [SerializeField] private float invicibility = 1f;
    private float temporaryInvicibility = 1f;
    public bool invicible = false;

    [Header("Jumping")]
    private bool isInAir = false;
    [SerializeField] private int groundInt = 6;
    [SerializeField] private int airInt = 9;
    //[SerializeField] private float jumpPressDelay = 1f;
    private float[] temporaryJumpPressDelay = new float[4];
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float startHeight = 0f;
    [SerializeField] private float jumpSpeed = 1f;
    [SerializeField] private float dropSpeed = 1f;
    private float tempJumpSpeed;
    private bool goingUpp = false;
    [SerializeField] private float uppTimer = 1f;
    [SerializeField] private float downTimer = 1f;
    private float tempTimer;
    private bool[] newInput = new bool[4];
    [SerializeField] private float jumpSpeedDecelerator = 0.5f;
    [SerializeField] private float jumpSpeedAccelerator = 1.5f;
    [SerializeField] private float fastFallMultiplyer = 1f;
    //private bool jumpBrake = false;

    [Header("Dashing")]
    //private bool shouldDash = false;
    //[SerializeField] private float dashSpeed = 1f;
    //private float tempDashSpeed = 1f;
    //[SerializeField] private float dashCap = 2f;
    //[SerializeField] private float dashAccelerator = 1.5f;
    //[SerializeField] private float dashDecelerator = 0.5f;
    //private bool canDashAgain = true;
    public bool isDead = false;

    [Header("References")]
    private Rigidbody2D rb2d;
    private PlayerHealth playerHealth;


    //todo fix jump and dropp



    private bool goingLeft;

    bool first = true;

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
        lanes[0] = -2.7f;
        lanes[1] = -0.9f;
        lanes[2] = 0.9f;
        lanes[3] = 2.7f;
        //Debug.Log(lanes[0] + "; " + lanes[1] + "; " + lanes[2] + "; " + lanes[3]);
    }

    private void Update()
    {
        if (!isDead)
        {
            CheckForInput();
            ExecuteInput();
            //Jump();
            if (temporaryInvicibility > 0)
            {
                temporaryInvicibility -= Time.deltaTime;
            }
            else
            {
                invicible = false;
            }
            
        }
    }
    private void FixedUpdate()
    {
        Move();
    }

    void CheckForInput()
    {
        pressedButtons[0] = Input.GetButtonDown("Left");
        pressedButtons[1] = Input.GetButtonDown("Right");
        pressedButtons[2] = Input.GetButtonDown("Up");
        pressedButtons[3] = Input.GetButtonDown("Down");
        /*Debug.Log(pressedButtons[0] + " " +
            pressedButtons[1] + " " +
            pressedButtons[2] + " " +
            pressedButtons[3]);*/
    }

    void ExecuteInput()
    {
        //left
        if (pressedButtons[0] && currentLane > 0)
        {
            _targetLocation.x = lanes[currentLane-1];
            currentLane--;
            if(first)
            {
                _targetLocation.x = lanes[1];
                currentLane = 1;
            }
            rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);
            goingLeft = true;
            Debug.Log(currentLane);
            Debug.Log(_targetLocation.x);
            first = false;
        }
        //right
        if (pressedButtons[1] && currentLane < 3)
        {
            _targetLocation.x = lanes[currentLane + 1];
            currentLane++;
            rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
            goingLeft = false;
            Debug.Log(currentLane);
            Debug.Log(_targetLocation.x);
            first = false;
        }
        
        if (goingLeft && transform.position.x < _targetLocation.x)
        {
            transform.position = new Vector3(_targetLocation.x, transform.position.y, transform.position.z);
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        }
        else if (!goingLeft && transform.position.x > _targetLocation.x)
        {
            transform.position = new Vector3(_targetLocation.x, transform.position.y, transform.position.z);
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        }

        //jump
        if (pressedButtons[2] && !isInAir)
        {
            _targetLocation.y = jumpHeight;
            isInAir = true;
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
            goingUpp = true;
            tempTimer = uppTimer;
            gameObject.layer = airInt;
        }
        //Debug.Log(isInAir);
        if (isInAir)
        {
            tempTimer -= Time.deltaTime;
            if (tempTimer <= 0)
            {
                transform.position = new Vector3(transform.position.x, _targetLocation.y, transform.position.z);

                if (goingUpp)
                {
                    _targetLocation.y = startHeight;
                    rb2d.velocity = new Vector2(rb2d.velocity.x, -dropSpeed);
                    goingUpp = false;
                    tempTimer = downTimer;
                }
            }
        }

        if(transform.position.y > jumpHeight || transform.position.y < startHeight)
        {
            transform.position = new Vector3(transform.position.x, _targetLocation.y, transform.position.z);
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
            if(!goingUpp)
            {
                isInAir = false;
                gameObject.layer = groundInt;
            }
        }
        //fast fall
        if (pressedButtons[3])
        {
            if(goingUpp)
            {
                _targetLocation.y = startHeight;
                rb2d.velocity = new Vector2(rb2d.velocity.x, -dropSpeed);
                goingUpp = false;
                tempTimer = downTimer;
            }
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * fastFallMultiplyer);
        }






        /*for (int i = 0; i < pressedButtons.Length; i++)
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
                //case 2:
                //    DashBeweenLanes();
                //    Debug.Log("Dashing");
                //    break;
                default:
                    break;
            }
        }*/
    }

    void MoveBetweenLanes()
    {
        /*
        for (int i = 0; i < pressedButtons.Length; i++)
        {
            if (pressedButtons[i] && !shouldDash && lanes[i] != _targetLocation.x)
            {
                _targetLocation.x = lanes[i];
                tempMoveSpeed = moveSpeed;
                //Debug.Log("Moving");
            }
        }*/
        
    }

    void DashBeweenLanes()
    {
        /*
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
        }*/
    }

    private void Move()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x * speedAccelerator, rb2d.velocity.y);

        if(goingUpp)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * jumpSpeedDecelerator);
        }
        else
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * jumpSpeedAccelerator);
        }
        

        

        
        


        /*temporaryUppdateFrequency -= Time.deltaTime;
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
            //currentLane = _targetLocation.x;
            shouldDash = false;
            jumpBrake = false;
        }*/
    }

    private void Jump()
    {
        /*
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
        }*/
    }

    public void BouncePlayer(List<GameObject> obstacles)
    {
        if(!invicible)
        {
        
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

                //canDashAgain = false;
                if(transform.position.x < lanes[y])
                {
                    _targetLocation.x = lanes[y];
                    currentLane = y;
                    rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
                    goingLeft = false;
                }
                else if (transform.position.x > lanes[y])
                {
                    _targetLocation.x = lanes[y];
                    currentLane = y;
                    rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);
                    goingLeft = true;
                }
            //tempMoveSpeed = moveSpeed;
            }
        }
        



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
