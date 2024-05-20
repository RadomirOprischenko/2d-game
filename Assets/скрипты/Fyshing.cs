using UnityEngine;

public class Fishing : MonoBehaviour
{
    public GameObject bobberPrefab; // Префаб поплавка

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SpawnBobber();
        }
    }

    void SpawnBobber()
    {
        // Создаем экземпляр поплавка в текущих координатах и повороте
        Instantiate(bobberPrefab, transform.position, transform.rotation);
    }
}
