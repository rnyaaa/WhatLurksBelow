using UnityEngine;
using System.Collections.Generic;

public class AnimalManager : MonoBehaviour
{
    public GameObject animalPrefab; // Assign the animal prefab in the inspector
    public float activationRadius = 50f;
    public float deactivationRadius = 60f;
    public Transform player;

    private List<Vector3> animalOrigins = new List<Vector3>();
    private List<GameObject> activeAnimals = new List<GameObject>();
    private Queue<GameObject> animalPool = new Queue<GameObject>();

    void Start()
    {
        // Collect all animal origins
        foreach (Transform child in transform)
        {
            animalOrigins.Add(child.position);
            child.gameObject.SetActive(false); // Hide original animals in the editor
        }
    }

    void Update()
    {
        ManageAnimals();
    }

    void ManageAnimals()
    {
        // Activate or deactivate animals based on distance to player
        for (int i = 0; i < animalOrigins.Count; i++)
        {
            float distance = Vector3.Distance(player.position, animalOrigins[i]);

            if (distance < activationRadius && !IsAnimalActiveAt(animalOrigins[i]))
            {
                ActivateAnimal(animalOrigins[i]);
            }
            else if (distance > deactivationRadius && IsAnimalActiveAt(animalOrigins[i]))
            {
                DeactivateAnimal(animalOrigins[i]);
            }
        }
    }

    bool IsAnimalActiveAt(Vector3 position)
    {
        return activeAnimals.Exists(a => a.transform.position == position);
    }

    void ActivateAnimal(Vector3 position)
    {
        GameObject animal = animalPool.Count > 0 ? animalPool.Dequeue() : Instantiate(animalPrefab);
        animal.transform.position = position;
        animal.SetActive(true);
        activeAnimals.Add(animal);
    }

    void DeactivateAnimal(Vector3 position)
    {
        GameObject animal = activeAnimals.Find(a => a.transform.position == position);
        if (animal != null)
        {
            animal.SetActive(false);
            animalPool.Enqueue(animal);
            activeAnimals.Remove(animal);
        }
    }
}
