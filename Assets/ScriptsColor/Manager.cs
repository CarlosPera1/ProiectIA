using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    private int populationSize;
    private float generationDuration;
    private int mutationRate;

    public GameObject ballPrefab;
    public List<GameObject> population;
    
    private int currentGeneration;
    public Text textCurrentGeneration;
    private float generationTimeLeft;
    public Text textGenerationTimeLeft;
    
    public GameObject simulationInterface;
    public GameObject configuratorInterface;

    public List<Image> averageColor;
    private int averageColorCounter;

    public Slider sliderNumberOfBalls;
    public Slider sliderGenerationDuration;
    public Slider sliderMutationRate;

    public Text counterSliderNumberOfBalls;
    public Text counterSliderGenerationDuration;
    public Text counterSliderMutationRate;

    private bool configurationDone;

    void Start()
    {
        configurationDone = false;
        configuratorInterface.SetActive(true);
        simulationInterface.SetActive(false);
    }
 
    void FixedUpdate()
    {
        if(configurationDone)
        {
            textCurrentGeneration.text = "Generation " + currentGeneration;
            generationTimeLeft -= Time.deltaTime;
            textGenerationTimeLeft.text = "Time left: " + (generationTimeLeft).ToString("0") + "s";
        }
        else
        {
            counterSliderNumberOfBalls.text = sliderNumberOfBalls.value + "";
            counterSliderGenerationDuration.text = sliderGenerationDuration.value.ToString("F1") + "s";
            counterSliderMutationRate.text = sliderMutationRate.value + "%";
        }
    }
 
    private void initialisePopulation()
    {
        for (int counter = 0; counter < populationSize; counter++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-14, 14), 1, Random.Range(-14, 14));
 
            GameObject ball = Instantiate(ballPrefab, randomPosition, Quaternion.identity);
 
            ADN ballADN = ball.GetComponent<ADN>();
            ballADN.r = Random.Range(0f, 1f);
            ballADN.g = Random.Range(0f, 1f);
            ballADN.b = Random.Range(0f, 1f);

            ball.GetComponent<Renderer>().material.color = new Color(ballADN.r, ballADN.g, ballADN.b);
 
            population.Add(ball);
        }
    }
 
    private void breedPopulation()
    {
        List<GameObject> auxiliarList = population.ToList();
        randomShuffleList(auxiliarList);

        population.Clear();
 
        for (int i = 0; i < auxiliarList.Count - 1; i+=2)
        {
            population.Add(breed(auxiliarList[i], auxiliarList[i + 1]));
            population.Add(breed(auxiliarList[i + 1], auxiliarList[i]));
        }

        drawHistogram();

        for(int i = 0; i < auxiliarList.Count; i++)
        {
            Destroy(auxiliarList[i]);
        }
 
        generationTimeLeft = generationDuration;
        currentGeneration++;
    }
 
    private GameObject breed(GameObject parent1, GameObject parent2)
    {
        Vector3 randomPosition = new Vector3(Random.Range(-14, 14), 1, Random.Range(-14, 14));
 
        GameObject newBall = Instantiate(ballPrefab, randomPosition, Quaternion.identity);
        ADN newBallADN = newBall.GetComponent<ADN>();
 
        ADN ADN1 = parent1.GetComponent<ADN>();
        ADN ADN2 = parent2.GetComponent<ADN>();

        //No mutation case
        if (mutationRate <= Random.Range(0, 101))
        {
            newBallADN.r = Random.Range(0, 10) < 5 ? ADN1.r : ADN2.r;
            newBallADN.g = Random.Range(0, 10) < 5 ? ADN1.g : ADN2.g;
            newBallADN.b = Random.Range(0, 10) < 5 ? ADN1.b : ADN2.b;
        }
        //Mutation case
        else
        {
            int random = Random.Range(0, 7);
            if (random == 0)
            {
                newBallADN.r = Random.Range(0.0f, 1.0f);
                newBallADN.g = Random.Range(0, 10) < 5 ? ADN1.g : ADN2.g;
                newBallADN.b = Random.Range(0, 10) < 5 ? ADN1.b : ADN2.b;
            }
            else if (random == 1)
            {
                newBallADN.r = Random.Range(0, 10) < 5 ? ADN1.r : ADN2.r;
                newBallADN.g = Random.Range(0.0f, 1.0f);
                newBallADN.b = Random.Range(0, 10) < 5 ? ADN1.b : ADN2.b;
            }
            else if (random == 2)
            {
                newBallADN.r = Random.Range(0, 10) < 5 ? ADN1.r : ADN2.r;
                newBallADN.g = Random.Range(0, 10) < 5 ? ADN1.g : ADN2.g;
                newBallADN.b = Random.Range(0.0f, 1.0f);
            }
            else if (random == 3)
            {
                newBallADN.r = Random.Range(0.0f, 1.0f);
                newBallADN.g = Random.Range(0.0f, 1.0f);
                newBallADN.b = Random.Range(0, 10) < 5 ? ADN1.b : ADN2.b;
            }
            else if (random == 4)
            {
                newBallADN.r = Random.Range(0.0f, 1.0f);
                newBallADN.g = Random.Range(0, 10) < 5 ? ADN1.g : ADN2.g;
                newBallADN.b = Random.Range(0.0f, 1.0f);
            }
            else if (random == 5)
            {
                newBallADN.r = Random.Range(0, 10) < 5 ? ADN1.r : ADN2.r;
                newBallADN.g = Random.Range(0.0f, 1.0f);
                newBallADN.b = Random.Range(0.0f, 1.0f);
            }
            else
            {
                newBallADN.r = Random.Range(0.0f, 1.0f);
                newBallADN.g = Random.Range(0.0f, 1.0f);
                newBallADN.b = Random.Range(0.0f, 1.0f);
            }
        }
        newBall.GetComponent<Renderer>().material.color = new Color(newBallADN.r, newBallADN.g, newBallADN.b);
 
        return newBall;
    }

    private void drawHistogram()
    {
        float r = 0;
        float g = 0;
        float b = 0;
        Color averageCurrentColor;

        for(int i = 0; i < population.Count; i++)
        {
            r += population[i].GetComponent<Renderer>().material.color.r;
            g += population[i].GetComponent<Renderer>().material.color.g;
            b += population[i].GetComponent<Renderer>().material.color.b;
        }
        averageCurrentColor = new Color(r/population.Count, g/population.Count, b/population.Count);

        if(averageColorCounter >= 0 && averageColorCounter <= 4)
        {
            averageColor[averageColorCounter].color = averageCurrentColor;
            for(int i=averageColorCounter + 1; i<5; i++)
            {
                averageColor[i].color = Color.black;
            }
        }
        else
        {
            averageColor[0].color = averageColor[1].color;
            averageColor[1].color = averageColor[2].color;
            averageColor[2].color = averageColor[3].color;
            averageColor[3].color = averageColor[4].color;
            averageColor[4].color = averageCurrentColor;
        }
        averageColorCounter++;
    }

    public void configDone()
    {
        configurationDone = true;
        configuratorInterface.SetActive(false);
        simulationInterface.SetActive(true);
        populationSize = (int)sliderNumberOfBalls.value;
        generationDuration = (float)sliderGenerationDuration.value;
        mutationRate = (int)sliderMutationRate.value;
        currentGeneration = 1;
        initialisePopulation();
        InvokeRepeating("breedPopulation", generationDuration, generationDuration);
        generationTimeLeft = generationDuration;
        averageColorCounter = 0;
        drawHistogram();
    }

    public List<GameObject> randomShuffleList(List<GameObject> auxiliarList)
    {
        for (int i = 0; i < auxiliarList.Count; i++) 
        {
            GameObject temp = auxiliarList[i];
            int randomIndex = Random.Range(i, auxiliarList.Count);
            auxiliarList[i] = auxiliarList[randomIndex];
            auxiliarList[randomIndex] = temp;
        }
        return auxiliarList;
    }
}
