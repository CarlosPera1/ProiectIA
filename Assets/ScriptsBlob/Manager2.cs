using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Manager2 : MonoBehaviour
{
    private int populationGreedySize;
    private int populationNonGreedySize;
    private int foodAvailable;
    private float generationDuration;

    public GameObject blobPrefab;
    public List<GameObject> population;

    private int currentGeneration;
    public Text textCurrentGeneration;
    private float generationTimeLeft;
    public Text textGenerationTimeLeft;

    public GameObject simulationInterface;
    public GameObject configuratorInterface;

    public Slider sliderNumberOfGreedyBlobs;
    public Slider sliderNumberOfGreedyNonBlobs;
    public Slider sliderFoodAvailable;
    public Slider sliderGenerationDuration;

    public Text counterSliderNumberOfGreedyBlobs;
    public Text counterSliderNumberOfNonGreedyBlobs;
    public Text counterSliderFoodAvailable;
    public Text counterSliderGenerationDuration;

    private bool configurationDone;

    public Text textBlobsCounter;

    public List<GameObject> redCandles;
    public List<GameObject> greenCandles;

    private int [] positionForXGraph = {-203,-158,-113,-68,-23,21,66,112,157,202};

    void Start()
    {
        configurationDone = false;
        configuratorInterface.SetActive(true);
        simulationInterface.SetActive(false);

        for(int i=0; i<redCandles.Count; i++)
        {
            redCandles[i].SetActive(false);
            greenCandles[i].SetActive(false);
        }
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
            counterSliderNumberOfGreedyBlobs.text = sliderNumberOfGreedyBlobs.value + "";
            counterSliderNumberOfNonGreedyBlobs.text = sliderNumberOfGreedyNonBlobs.value + "";
            counterSliderFoodAvailable.text = sliderFoodAvailable.value + "";
            counterSliderGenerationDuration.text = sliderGenerationDuration.value.ToString("F1") + "s";
        }
    }

    private void initialisePopulation()
    {
        for (int counter = 0; counter < populationGreedySize; counter++)
        {
            Vector3 randomPosition = generateRandomPosition();
            GameObject blob = Instantiate(blobPrefab, randomPosition, Quaternion.identity);
            ADN2 blobADN = blob.GetComponent<ADN2>();
            blobADN.greedy = true;
            blob.GetComponent<Renderer>().material.color = Color.red;
            population.Add(blob);
        }
        for (int counter = 0; counter < populationNonGreedySize; counter++)
        {
            Vector3 randomPosition = generateRandomPosition();
            GameObject blob = Instantiate(blobPrefab, randomPosition, Quaternion.identity);
            ADN2 blobADN = blob.GetComponent<ADN2>();
            blobADN.greedy = false;
            blob.GetComponent<Renderer>().material.color = Color.green;
            population.Add(blob);
        }
    }

    private void breedPopulation()
    {
        List<GameObject> auxiliarList = population.ToList();
        randomShuffleList(auxiliarList);

        population.Clear();

        int auxiliarFoodAvailable = foodAvailable;

        int blobsWhoNeedToFightForFood = (auxiliarList.Count - foodAvailable) * 2;
        if(blobsWhoNeedToFightForFood < 0)
        {
            blobsWhoNeedToFightForFood = 0;
        }
        if(blobsWhoNeedToFightForFood > foodAvailable*2)
        {
            blobsWhoNeedToFightForFood = foodAvailable*2;
        }
        
        for (int i = 0; i < blobsWhoNeedToFightForFood; i+=2)
        {
            if(auxiliarList[i] != null && auxiliarList[i+1] != null && auxiliarFoodAvailable >= 2)
            {
                if(auxiliarList[i].GetComponent<ADN2>().greedy == true && auxiliarList[i+1].GetComponent<ADN2>().greedy == true)
                {
                    //each blob havs 50% survival rate
                    if(Random.Range(0, 10) < 5)
                    {
                        population.Add(stayAlive(auxiliarList[i]));
                    }
                    if(Random.Range(0, 10) < 5)
                    {
                        population.Add(stayAlive(auxiliarList[i+1]));
                    }
                }
                else if(auxiliarList[i].GetComponent<ADN2>().greedy == true && auxiliarList[i+1].GetComponent<ADN2>().greedy == false)
                {
                    //greedy blob has 100% survival rate
                    population.Add(stayAlive(auxiliarList[i]));
                    //100% a random ADN blob will be born
                    population.Add(breed(auxiliarList[i],auxiliarList[i+1]));
                    //non-greedy blob has 50% survival rate
                    if(Random.Range(0, 10) < 5)
                    {
                        population.Add(stayAlive(auxiliarList[i+1]));
                    }
                }
                else if(auxiliarList[i].GetComponent<ADN2>().greedy == false && auxiliarList[i+1].GetComponent<ADN2>().greedy == true)
                {
                    //greedy blob has 100% survival rate
                    population.Add(stayAlive(auxiliarList[i+1]));
                    //100% a random ADN blob will be born
                    population.Add(breed(auxiliarList[i+1],auxiliarList[i]));
                    //non-greedy blob has 50% survival rate
                    if(Random.Range(0, 10) < 5)
                    {
                        population.Add(stayAlive(auxiliarList[i]));
                    }
                }
                else
                {
                    //both blobs have 100% survival rate
                    population.Add(stayAlive(auxiliarList[i]));
                    population.Add(stayAlive(auxiliarList[i+1]));
                    //both blobs have 50% reproduction rate
                    if(Random.Range(0, 10) < 5)
                    {
                        population.Add(breed(auxiliarList[i],auxiliarList[i+1]));
                    }
                    if(Random.Range(0, 10) < 5)
                    {
                        population.Add(breed(auxiliarList[i+1],auxiliarList[i]));
                    }
                }
                auxiliarFoodAvailable -= 2;
            }
        }
        //blobs that don't need to fight have 100% reproduction rate
        for(int i = blobsWhoNeedToFightForFood; i < auxiliarList.Count; i++)
        {
            if(auxiliarList[i] != null && auxiliarFoodAvailable >= 1)
            {
                population.Add(stayAlive(auxiliarList[i]));
                population.Add(breed(auxiliarList[i], auxiliarList[i]));
                auxiliarFoodAvailable -= 1;
            }
        }

        for(int i = 0; i < auxiliarList.Count; i++)
        {
            Destroy(auxiliarList[i]);
        }
 
        generationTimeLeft = generationDuration;
        currentGeneration++;

        updateGraph();
        textBlobsCounter.text = "Greedy blobs: " + countBlobs(true) + "\nNon-greedy blobs: " + countBlobs(false);
    }

    private GameObject stayAlive(GameObject parent)
    {
        Vector3 randomPosition = generateRandomPosition();
 
        GameObject newBlob = Instantiate(blobPrefab, randomPosition, Quaternion.identity);

        newBlob.GetComponent<ADN2>().greedy = parent.GetComponent<ADN2>().greedy;

        if(newBlob.GetComponent<ADN2>().greedy)
        {
            newBlob.GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            newBlob.GetComponent<Renderer>().material.color = Color.green;
        }

        return newBlob;
    }

    private GameObject breed(GameObject parent1, GameObject parent2)
    {
        Vector3 randomPosition = generateRandomPosition();
 
        GameObject newBlob = Instantiate(blobPrefab, randomPosition, Quaternion.identity);

        if(Random.Range(0, 10) < 5)
        {
            newBlob.GetComponent<ADN2>().greedy = parent1.GetComponent<ADN2>().greedy;
        }
        else
        {
            newBlob.GetComponent<ADN2>().greedy = parent2.GetComponent<ADN2>().greedy;
        }

        if(newBlob.GetComponent<ADN2>().greedy)
        {
            newBlob.GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            newBlob.GetComponent<Renderer>().material.color = Color.green;
        }

        return newBlob;
    }

    public void configDone()
    {
        configurationDone = true;
        configuratorInterface.SetActive(false);
        simulationInterface.SetActive(true);
        populationGreedySize = (int)sliderNumberOfGreedyBlobs.value;
        populationNonGreedySize = (int)sliderNumberOfGreedyNonBlobs.value;
        foodAvailable = (int)sliderFoodAvailable.value;
        generationDuration = (float)sliderGenerationDuration.value;
        currentGeneration = 1;
        initialisePopulation();
        InvokeRepeating("breedPopulation", generationDuration, generationDuration);
        generationTimeLeft = generationDuration;
        updateGraph();
        textBlobsCounter.text = "Greedy blobs: " + countBlobs(true) + "\nNon-greedy blobs: " + countBlobs(false);
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

    public int countBlobs(bool greedy)
    {
        int currentNumberOfGreedyBlobs = 0;
        int currentNumberOfNonGreedyBlobs = 0;

        for(int i=0; i < population.Count; i++)
        {
            if(population[i].GetComponent<ADN2>().greedy)
            {
                currentNumberOfGreedyBlobs++;
            }
            else
            {
                currentNumberOfNonGreedyBlobs++;
            }
        }

        if(greedy)
        {
            return currentNumberOfGreedyBlobs;
        }
        else
        {
            return currentNumberOfNonGreedyBlobs;
        }
    }

    public Vector3 generateRandomPosition()
    {
        Vector3 randomPosition = new Vector3(0,0,0);
        switch(Random.Range(0,4))
        {
            case 0:
                randomPosition = new Vector3(Random.Range(-14f, 14f), 1, -14);
                break;
            case 1:
                randomPosition = new Vector3(Random.Range(-14f, 14f), 1, 14);
                break;
            case 2:
                randomPosition = new Vector3(-14, 1, Random.Range(-14f, 14f));
                break;
            case 3:
                randomPosition = new Vector3(14, 1, Random.Range(-14f, 14f));
                break;
            default:
                break;
        }
        return randomPosition;
    }

    public void updateGraph()
    {
        const int candleSizeMultiplier = 2;

        if(currentGeneration-1 < 10)
        {
            greenCandles[currentGeneration-1].SetActive(true);
            redCandles[currentGeneration-1].SetActive(true);

            greenCandles[currentGeneration-1].GetComponent<RectTransform>().sizeDelta = new Vector2(40, countBlobs(false)*candleSizeMultiplier);
            redCandles[currentGeneration-1].GetComponent<RectTransform>().sizeDelta = new Vector2(40, countBlobs(true)*candleSizeMultiplier);

            greenCandles[currentGeneration-1].GetComponent<RectTransform>().anchoredPosition = new Vector3(
                positionForXGraph[currentGeneration-1],(countBlobs(false)*candleSizeMultiplier)/2,0);
            redCandles[currentGeneration-1].GetComponent<RectTransform>().anchoredPosition = new Vector3(
                positionForXGraph[currentGeneration-1],countBlobs(false)*candleSizeMultiplier+(countBlobs(true)*candleSizeMultiplier)/2,0);           
        }
        else
        {
            for(int i=0; i<9; i++)
            {
                greenCandles[i].GetComponent<RectTransform>().sizeDelta =  greenCandles[i+1].GetComponent<RectTransform>().sizeDelta;
                redCandles[i].GetComponent<RectTransform>().sizeDelta =  redCandles[i+1].GetComponent<RectTransform>().sizeDelta;
                greenCandles[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(
                    positionForXGraph[i],greenCandles[i+1].GetComponent<RectTransform>().anchoredPosition.y,0);
                redCandles[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(
                    positionForXGraph[i],redCandles[i+1].GetComponent<RectTransform>().anchoredPosition.y,0);
            }
            greenCandles[9].GetComponent<RectTransform>().sizeDelta = new Vector2(40, countBlobs(false)*candleSizeMultiplier);
            redCandles[9].GetComponent<RectTransform>().sizeDelta = new Vector2(40, countBlobs(true)*candleSizeMultiplier);
            greenCandles[9].GetComponent<RectTransform>().anchoredPosition = new Vector3(
                positionForXGraph[9],(countBlobs(false)*candleSizeMultiplier)/2,0);
            redCandles[9].GetComponent<RectTransform>().anchoredPosition = new Vector3(
                positionForXGraph[9],countBlobs(false)*candleSizeMultiplier+(countBlobs(true)*candleSizeMultiplier)/2,0);  
        }
    }
}
