using System.Collections;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float moveDistance = 5f;  // How high the elevator should move
    public float moveSpeed = 2f;     // Speed of movement
    public float waitTime = 2f;      // Time before returning to original position

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isMovingUp = false;

    private void Start()
    {
        originalPosition = transform.position;
        targetPosition = originalPosition + Vector3.up * moveDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isMovingUp)
        {
            StartCoroutine(MoveElevator());
        }
    }

    private IEnumerator MoveElevator()
    {
        isMovingUp = true;

        // Move Up
        yield return StartCoroutine(MoveToPosition(targetPosition));

        // Wait at the top
        yield return new WaitForSeconds(waitTime);

        // Move Down
        yield return StartCoroutine(MoveToPosition(originalPosition));

        isMovingUp = false;
    }

    private IEnumerator MoveToPosition(Vector3 destination)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < moveSpeed)
        {
            transform.position = Vector3.Lerp(startPosition, destination, elapsedTime / moveSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = destination; // Ensure it reaches exactly the target position
    }
}

