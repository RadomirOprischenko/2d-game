
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishing : MonoBehaviour
{
    public List<Fish> fishList; // Список рыб
    private GameObject currentBobber; // Текущий поплавок
    private bool isFishBiting = false; // Флаг поклевки
    private bool isReelingIn = false; // Флаг вытаскивания рыбы
    private float timeToBite; // Время до поклевки
    private float reactionTime = 2f; // Время для реакции
    private float holdTime = 2f; // Время удержания для ловли рыбы
    private Fish currentFish; // Текущая рыба

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentBobber == null)
        {
            SpawnBobber();
        }

        if (isFishBiting && Input.GetKeyDown(KeyCode.E))
        {
            isReelingIn = true;
            StartCoroutine(HoldToCatchFish());
        }

        if (isFishBiting && Input.GetKeyUp(KeyCode.E))
        {
            isReelingIn = false;
        }
    }

    void SpawnBobber()
    {
        currentFish = SelectRandomFish();
        if (currentFish != null)
        {
            Vector3 bobberSpawnPosition = transform.position; // Начальная позиция поплавка
            currentBobber = Instantiate(currentFish.prefab, bobberSpawnPosition, Quaternion.identity);
            StartCoroutine(FishBiteCoroutine());
        }
    }

    Fish SelectRandomFish()
    {
        float totalChance = 0f;
        foreach (Fish fish in fishList)
        {
            totalChance += fish.spawnChance;
        }

        float randomPoint = Random.value * totalChance;

        foreach (Fish fish in fishList)
        {
            if (randomPoint < fish.spawnChance)
            {
                return fish;
            }
            else
            {
                randomPoint -= fish.spawnChance;
            }
        }

        return null;
    }

    IEnumerator FishBiteCoroutine()
    {
        // Случайное время до поклевки (от 1 до 5 секунд)
        timeToBite = Random.Range(1f, 5f);
        yield return new WaitForSeconds(timeToBite);

        // Дергание поплавка
        isFishBiting = true;
        Debug.Log("Fish is biting! Press and hold E to catch!");

        // Ждем 2 секунды для реакции
        yield return new WaitForSeconds(reactionTime);

        if (isFishBiting && !isReelingIn)
        {
            // Если игрок не успел среагировать, рыба уплывает
            Debug.Log("Fish got away...");
            isFishBiting = false;
            Destroy(currentBobber);
            currentBobber = null;
        }
    }

    IEnumerator HoldToCatchFish()
    {
        float holdTimer = 0f;
        Vector3 startPosition = currentBobber.transform.position;
        Vector3 endPosition = new Vector3(transform.position.x, startPosition.y, startPosition.z);

        while (isReelingIn && holdTimer < holdTime)
        {
            holdTimer += Time.deltaTime;

            // Перемещаем поплавок к игроку только по горизонтали
            if (currentBobber != null)
            {
                currentBobber.transform.position = Vector3.MoveTowards(
                    currentBobber.transform.position,
                    endPosition,
                    Time.deltaTime * (1f / holdTime) * Vector3.Distance(startPosition, endPosition)
                );
            }

            yield return null;
        }

        if (holdTimer >= holdTime)
        {
            Debug.Log("Fish caught! Caught: " + currentFish.name);
            isFishBiting = false;
            isReelingIn = false;
            Destroy(currentBobber);
            currentBobber = null;
            // Здесь можно добавить логику для добавления рыбы в инвентарь и т.д.
        }
        else if (!isReelingIn)
        {
            Debug.Log("Stopped reeling in.");
        }
    }
}
