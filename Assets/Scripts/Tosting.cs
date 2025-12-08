using System.Collections;
using UnityEngine;

public class Tosting : MonoBehaviour
{
    public GameObject itemA;
    public GameObject itemB;
    public float showSeconds = 3f;
    public bool oneShot = true;

    bool playerInRange = false;

    bool busy = false;
    bool done = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (busy || (oneShot && done)) return;
        StartCoroutine(Run());
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

    IEnumerator Run()
    {
        busy = true;

        // 나타남
        if (itemA != null)
        {
            itemA.SetActive(true);
        }
           

        // 5초 대기
        yield return new WaitForSeconds(showSeconds);

        // 숨기고 나타남
        if (itemA)
        {
            itemA.SetActive(false);
        }
        if (itemB)
        {
            itemB.SetActive(true);
        } 

        done = true;
        busy = false;
    }
}
