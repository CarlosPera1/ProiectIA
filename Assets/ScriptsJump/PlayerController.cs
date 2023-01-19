using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Transform obstacoleTransform;
    private Rigidbody rigidbodyPlayer;
    private Transform transformPlayer;
    private Vector3 playerInitialPosition;
    public bool grounded;
    public int[,] playerDecision;

    public bool jumpedThisRound;
    private int maxDistanceForJump;

    public TextMesh textInfoDecision;

    private ObstacoleController obstacole;

    //Config screen
    public Slider sliderSpeedOfThePlayer;
    public Slider sliderMaxDistanceForJump;
    public Slider sliderLearningSpeed;
    public Text counterSliderSpeedOfThePlayer;
    public Text counterSliderMaxDistanceForJump;
    public Text counterSliderLearningSpeed;
    public bool configurationDone;
    public GameObject simulationInterface;
    public GameObject configuratorInterface;

    public Transform mainCamera;
    private Vector3 cameraPosition1 = new Vector3(6.22f,2.21f,-9.83f);
    private Vector3 cameraPosition2 = new Vector3(-27f,5f,52f);

    // Start is called before the first frame update
    void Start()
    {
        mainCamera.position = cameraPosition1;
        obstacole = obstacoleTransform.GetComponent<ObstacoleController>();
        configurationDone = false;
        configuratorInterface.SetActive(true);
        simulationInterface.SetActive(false);
    }

    public int thisRoundDistanceForJump;
    public bool newRound;

    void FixedUpdate()
    {
        if(configurationDone)
        {
            int distanceFromObstacole = (int)obstacoleTransform.position.z;
            int obstacoleSpeed = obstacole.obstacoleSpeed;
            if(newRound)
            {
                bool solutionFound = false;
                for(int i=0; i<maxDistanceForJump; i++)
                {
                    if(playerDecision[obstacoleSpeed,i] == 1)
                    {
                        solutionFound = true;
                        thisRoundDistanceForJump = i;
                    }
                }
                if(!solutionFound)
                {
                    thisRoundDistanceForJump = Random.Range(1,maxDistanceForJump);
                    if(playerDecision[obstacoleSpeed,thisRoundDistanceForJump] == -1)
                    {
                        int numberOfTries = 0;
                        while(playerDecision[obstacoleSpeed,thisRoundDistanceForJump] == -1 && numberOfTries<5)
                        {
                            thisRoundDistanceForJump = Random.Range(1,maxDistanceForJump);
                            numberOfTries++;
                        }
                    }
                }
                newRound = false;
                if(solutionFound)
                {
                    textInfoDecision.text = "Ball speed is " +  obstacoleSpeed + " km/h\nI will jump when it will be " + thisRoundDistanceForJump + "m distance from me\nI'm pretty sure about my move";
                }
                else
                {
                    textInfoDecision.text = "Ball speed is " +  obstacoleSpeed + " km/h\nI will jump when it will be " + thisRoundDistanceForJump + "m distance from me\nI'm not sure about my move";
                }
            }
            if(thisRoundDistanceForJump >= distanceFromObstacole && !jumpedThisRound)
            {
                jumpedThisRound = true;
                jump();
            }
            if(transformPlayer.position.y < -1)
            {
                Restart();
            }
        }
        else
        {
            counterSliderSpeedOfThePlayer.text = sliderSpeedOfThePlayer.value + "";
            counterSliderMaxDistanceForJump.text = sliderMaxDistanceForJump.value + "";
            counterSliderLearningSpeed.text = sliderLearningSpeed.value + "";
        }
        
    }

    public void jump()
    {
        if(grounded)
        {
            rigidbodyPlayer.AddForce(new Vector3(0, 7, 0), ForceMode.VelocityChange);
            grounded = false;
        }
    }

    public void Restart()
    {
        rigidbodyPlayer.velocity = Vector3.zero;
        rigidbodyPlayer.angularVelocity = Vector3.zero; 
        transformPlayer.position = playerInitialPosition;
        grounded = true;
        thisRoundDistanceForJump = -int.MaxValue;
    }

    private void OnCollisionEnter(Collision colliderObject)
    {
        if (colliderObject.gameObject.CompareTag("Platform"))
        {
            grounded = true;
        }
        else if (colliderObject.gameObject.CompareTag("Obstacole"))
        {
            Restart();
        }
    }

    public void configDone()
    {
        configurationDone = true;
        configuratorInterface.SetActive(false);
        simulationInterface.SetActive(true);
        obstacole.maxSpeed = (int)sliderSpeedOfThePlayer.value;
        obstacole.obstacoleSpeed = Random.Range(obstacole.minSpeed, obstacole.maxSpeed);
        maxDistanceForJump = (int)sliderMaxDistanceForJump.value;
        Time.timeScale = (int)sliderLearningSpeed.value;
        rigidbodyPlayer = GetComponent<Rigidbody>();
        transformPlayer = GetComponent<Transform>();
        playerInitialPosition = transformPlayer.position;
        grounded = true;
        jumpedThisRound = false;
        playerDecision = new int[obstacole.maxSpeed, maxDistanceForJump];
        newRound = true;
    }

    public void changeCamera()
    {
        if(mainCamera.position == cameraPosition1)
        {
            mainCamera.position = cameraPosition2;
        }
        else
        {
            mainCamera.position = cameraPosition1;
        }
    }

}
