
using System.Collections;
using UnityEngine;

public class Fishing : MonoBehaviour
{
    public GameObject bobberPrefab; // Префаб поплавка
    public Vector3 bobberSpawnPosition; // Координаты появления поплавка
    private GameObject currentBobber; // Текущий поплавок
    private bool isFishBiting = false; // Флаг поклевки
    private float timeToBite; // Время до поклевки
    private float reactionTime = 2f; // Время для реакции

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && currentBobber == null)
        {
            SpawnBobber(bobberSpawnPosition);
        }

        if (isFishBiting && Input.GetKeyDown(KeyCode.Space))
        {
            CatchFish();
        }
    }

    void SpawnBobber(Vector3 position)
    {
        currentBobber = Instantiate(bobberPrefab, position, Quaternion.identity);
        StartCoroutine(FishBiteCoroutine());
    }

    IEnumerator FishBiteCoroutine()
    {
        // Случайное время до поклевки (от 1 до 5 секунд)
        timeToBite = Random.Range(1f, 5f);
        yield return new WaitForSeconds(timeToBite);

        // Дергание поплавка
        isFishBiting = true;
        Debug.Log("Fish is biting! Press Space to catch!");

        // Ждем 2 секунды для реакции
        yield return new WaitForSeconds(reactionTime);

        if (isFishBiting)
        {
            // Если игрок не успел среагировать, рыба уплывает
            Debug.Log("Fish got away...");
            isFishBiting = false;
            Destroy(currentBobber);
            currentBobber = null;
        }
    }

    void CatchFish()
    {
        if (isFishBiting)
        {
            Debug.Log("Fish caught!");
            isFishBiting = false;
            Destroy(currentBobber);
            currentBobber = null;
            // Здесь можно добавить логику для добавления рыбы в инвентарь и т.д.
        }
    }
}

