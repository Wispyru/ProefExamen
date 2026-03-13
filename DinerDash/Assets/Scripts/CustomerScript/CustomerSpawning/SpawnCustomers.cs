using System.Collections;
using UnityEngine;

public class SpawnCustomers : MonoBehaviour
{
    [SerializeField]
    private GameObject _customerGroup;
    private float _spawnDelay = 5f;

    public bool CanSpawn = true;

    private void Update()
    {
        if (CanSpawn)
        {
            StartCoroutine(SpawnDelay());
            CanSpawn = false;
        }
    }

    /// <summary>
    /// Spawns customer group at the position of the game object
    /// </summary>
    public void SpawnCustomerGroup()
    {
        Instantiate(_customerGroup, gameObject.transform.position, gameObject.transform.rotation);
    }


    private IEnumerator SpawnDelay()
    {

        if(!CanSpawn) yield return null;
        yield return new WaitForSeconds(_spawnDelay);
        SpawnCustomerGroup();
        

    }

}
