using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Darwin : MonoBehaviour
{
    [SerializeField]
    private Ghost ghostPrefab;
    [SerializeField]
    private int numberOfGenerations;
    [SerializeField]
    private int generationSize;
    [SerializeField]
    private Transform spawnTransform;
    [SerializeField]
    private Transform targetTransform;
    [SerializeField]
    private float mutabilityProbability;
    [SerializeField]
    private Text generationNumberText;
    [SerializeField]
    private Text aptitudeNumberText;

    private Ghost[] generation;
    private int currentGeneration;
    private float[] generationAptitude;

    private float[] generationsRecords;

    private int numberOfGhostsDead;

    private Vector2 spawnPoint;
    private Vector2 targetPoint;

    private void Awake()
    {
        InitializeDarwinism();
    }

    public void InitializeDarwinism()
    {
        generation = new Ghost[generationSize];
        currentGeneration = 0;
        generationAptitude = new float[generationSize];

        generationsRecords = new float[numberOfGenerations];

        numberOfGhostsDead = 0;

        spawnPoint = spawnTransform.position;
        targetPoint = targetTransform.position;

        CreateFirstGeneration();

        generationNumberText.text = (currentGeneration + 1).ToString();
    }

    private void CreateFirstGeneration()
    {
        for (int i = 0; i < generationSize; i++)
        {
            generation[i] = Instantiate(ghostPrefab, spawnPoint, ghostPrefab.transform.rotation);
            generation[i].InitializeGhost();
            generation[i].name = "Ghost_" + (i + 1);
            generation[i].GhostDeadDelegate = GhostDead;
        }
    }

    private void GhostDead()
    {
        if(++numberOfGhostsDead >= generationSize)
        {
            if(currentGeneration < numberOfGenerations)
            {
                NextGeneration();
            }
        }
    }

    private void NextGeneration()
    {
        float generationAptitudeSum = 0;

        for (int i = 0; i < generationSize; i++)
        {
            generationAptitude[i] = CalculateAptitude(generation[i]);
            generationAptitudeSum += generationAptitude[i];
        }

        generationsRecords[currentGeneration] = generationAptitudeSum / generationSize;

        aptitudeNumberText.text = generationsRecords[currentGeneration].ToString("F2");

        if(currentGeneration > 0)
        {
            if(generationsRecords[currentGeneration - 1] > generationsRecords[currentGeneration])
            {
                aptitudeNumberText.color = new Color(1, 0, 0, 1);
            }

            else
            {
                aptitudeNumberText.color = new Color(0, 1, 0, 1);
            }
        }

        else
        {
            aptitudeNumberText.color = new Color(0, 1, 0, 1);
        }

        ++currentGeneration;

        generationNumberText.text = (currentGeneration + 1).ToString();

        BubbleSortGeneration(generationAptitude);

        Ghost[] bestTwoGhosts = SelectTwoBestGhosts();

        CreateNextGeneration(bestTwoGhosts);

        numberOfGhostsDead = 0;
    }

    private float CalculateAptitude(Ghost ghost)
    {
        Vector2 ghostPosition = new Vector2(ghost.transform.position.x, ghost.transform.position.y);
        float totalDistanceOfTrack = (spawnPoint - targetPoint).magnitude;
        float distanceToTarget = (targetPoint - ghostPosition).magnitude;

        return distanceToTarget > 0 ? totalDistanceOfTrack / distanceToTarget : 1f;
    }

    private void BubbleSortGeneration(float[] generationAptitude)
    {
        int n = generationAptitude.Length;

        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (generationAptitude[j] < generationAptitude[j + 1])
                {
                    float tempAptitude = generationAptitude[j];
                    generationAptitude[j] = generationAptitude[j + 1];
                    generationAptitude[j + 1] = tempAptitude;

                    Ghost tempGhost = generation[j];
                    generation[j] = generation[j + 1];
                    generation[j + 1] = tempGhost;
                }
            }
        }

    }

    private Ghost[] SelectTwoBestGhosts()
    {
        return new Ghost[] { generation[0], generation[1] };
    }

    private void CreateNextGeneration(Ghost[] twoBestGhosts)
    {
        twoBestGhosts[0].transform.position = spawnPoint;
        twoBestGhosts[1].transform.position = spawnPoint;

        for (int i = 2; i < generation.Length; i++)
        {
            float randomNumber = UnityEngine.Random.Range(0f, 1f);

            if(randomNumber < 0.5f)
            {
                MixGhostGenes(twoBestGhosts[0], twoBestGhosts[1], generation[i]);
            }

            else
            {
                MixGhostGenes(twoBestGhosts[1], twoBestGhosts[0], generation[i]);
            }

            MutateGhostGenes(generation[i]);

            generation[i].transform.position = spawnPoint;

            generation[i].ResetGhost();
        }

        MutateGhostGenes(twoBestGhosts[0]);
        MutateGhostGenes(twoBestGhosts[1]);

        twoBestGhosts[0].ResetGhost();
        twoBestGhosts[1].ResetGhost();
    }

    private void MixGhostGenes(Ghost ghost1, Ghost ghost2, Ghost ghostToMix)
    {
        int numberOfGenes = ghostToMix.GetGenes().TimesForImpulses.Length;

        for (int i = 0; i < numberOfGenes; i++)
        {
            if(i < numberOfGenes / 2)
            {
                ghostToMix.GetGenes().TimesForImpulses[i] = ghost1.GetGenes().TimesForImpulses[i];
            }

            else
            {
                ghostToMix.GetGenes().TimesForImpulses[i] = ghost2.GetGenes().TimesForImpulses[i];
            }
        }
    }

    private void MutateGhostGenes(Ghost ghost)
    {
        int numberOfGenes = ghost.GetGenes().TimesForImpulses.Length;

        float minTimeImpulse = ghost.GetMinTimeImpulse();
        float maxTimeImpulse = ghost.GetMaxTimeImpulse();

        for (int i = 0; i < numberOfGenes; i++)
        {
            float randomNumber = UnityEngine.Random.Range(0f, 1f);

            if(randomNumber < mutabilityProbability)
            {
                ghost.GetGenes().TimesForImpulses[i] = UnityEngine.Random.Range(minTimeImpulse, maxTimeImpulse);
            }
        }
    }
}
