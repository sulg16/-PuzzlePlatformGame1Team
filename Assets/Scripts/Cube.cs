using UnityEngine;

public class Cube : MonoBehaviour
{
    public GameObject itemA;
    public bool oneShot = true;

    bool playerInRange = false;
    bool used = false;

    private void Update()
    {
        if (!playerInRange)
        {
            return;
        }

        if (used && oneShot)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            OnInteract();
        }
    }

    void OnInteract()
    {
        if (itemA != null)
        {
            itemA.SetActive(true);
        }

        used = true;

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        { 
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }


}
