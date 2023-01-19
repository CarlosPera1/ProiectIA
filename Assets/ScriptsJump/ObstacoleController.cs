using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObstacoleController : MonoBehaviour
{
    public GameObject playerGameobject;
    private PlayerController player;

    private Rigidbody rigidbodyObstacole;
    private Transform obstacoleTransform;
    public int obstacoleSpeed;
    public int maxSpeed;
    public int minSpeed;

    private Vector3 obstacoleInitialPosition;
    private int currentScore;
    private int bestScore;
    public TextMesh textCurrentScore;
    public TextMesh textBestScore;
    public TextMesh textNumberOfTrainings;
    private int numberOfTrainings;

    private bool configurationDone;

    public GameObject NeuralNetworkLayer1;
    public GameObject NeuralNetworkLayer2;
    public GameObject NeuralNetworkLayer3;

    public GameObject neuralConnection1;
    public GameObject neuralConnection2;

    void Start()
    {
        configurationDone = false;
        player = playerGameobject.GetComponent<PlayerController>();
    }

    public void configDone()
    {
        rigidbodyObstacole = GetComponent<Rigidbody>();
        obstacoleTransform = GetComponent<Transform>();
        obstacoleInitialPosition = obstacoleTransform.transform.position;
        currentScore = 0;
        bestScore = 0;
        textCurrentScore.text = "Current score: " + currentScore;
        textBestScore.text = "Best score: " + bestScore;
        configurationDone = true;
        numberOfTrainings = 0;
        textNumberOfTrainings.text = "Number of trainings: " + numberOfTrainings;
    }

    void FixedUpdate()
    {
        if(player.configurationDone)
        {
            if(configurationDone)
            {
                rigidbodyObstacole.AddForce(new Vector3(0, 0, -obstacoleSpeed));
                if(obstacoleTransform.position.y < -1)
                {
                    Restart();
                }
            }
            else
            {
                configDone();
            }
            
        }
    }

    private void OnCollisionEnter(Collision colliderObject)
    {
        if (colliderObject.gameObject.CompareTag("Respawner"))
        {
            currentScore++;
            if(player.thisRoundDistanceForJump > 0)
            {
                player.playerDecision[obstacoleSpeed,player.thisRoundDistanceForJump] = 1;
                create_vessel(NeuralNetworkLayer1.transform.GetChild(obstacoleSpeed).transform.position, NeuralNetworkLayer2.transform.GetChild(5).GetChild(player.thisRoundDistanceForJump).transform.position, Color.green, neuralConnection1);
                create_vessel(NeuralNetworkLayer2.transform.GetChild(5).GetChild(player.thisRoundDistanceForJump).transform.position, NeuralNetworkLayer3.transform.GetChild(1).transform.position ,Color.green, neuralConnection2);

            }
            if(currentScore > bestScore)
            {
                bestScore = currentScore;
            }
            Restart();           
        }
        else if(colliderObject.gameObject.CompareTag("Player"))
        {
            if(player.thisRoundDistanceForJump > 0)
            {
                player.playerDecision[obstacoleSpeed,player.thisRoundDistanceForJump] = -1;
                create_vessel(NeuralNetworkLayer1.transform.GetChild(obstacoleSpeed).transform.position, NeuralNetworkLayer2.transform.GetChild(5).GetChild(player.thisRoundDistanceForJump).transform.position, Color.red, neuralConnection1);
                create_vessel(NeuralNetworkLayer2.transform.GetChild(5).GetChild(player.thisRoundDistanceForJump).transform.position, NeuralNetworkLayer3.transform.GetChild(0).transform.position ,Color.red, neuralConnection2);

            }
            Restart();
            currentScore = 0;
        }
        textCurrentScore.text = "Current score: " + currentScore;
        textBestScore.text = "Best score: " + bestScore;
    }

    public void Restart()
    {
        rigidbodyObstacole.velocity = Vector3.zero;
        rigidbodyObstacole.angularVelocity = Vector3.zero;
        obstacoleSpeed = Random.Range(minSpeed, maxSpeed);
        obstacoleTransform.position = obstacoleInitialPosition;
        player.newRound = true;
        player.jumpedThisRound = false;
        numberOfTrainings++;
        textNumberOfTrainings.text = "Number of trainings: " + numberOfTrainings;
    }

    void create_vessel(Vector3 p1, Vector3 p2, Color color, GameObject neuralConnection) {
        float size = Vector3.Distance (p1, p2);
        Vector3 pos = Vector3.Lerp(p1,p2,(float)0.5);
        neuralConnection.transform.position = pos;
        neuralConnection.transform.up = p2-p1;
        neuralConnection.transform.localScale = new Vector3(0.2f,size/2,0.2f);
        neuralConnection.GetComponent<Renderer>().material.color = color;
    }
}
