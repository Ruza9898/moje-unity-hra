using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StepCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text stepsText;
    [SerializeField] private float minMoveDistance = 0.5f;

    public int Steps { get; private set; }

    private ScriptMachine scriptMachine;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 lastCountedPosition;
    private bool gameEnded;

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        lastCountedPosition = startPosition;
        Steps = 0;
        scriptMachine = GetComponent<ScriptMachine>();

        if (stepsText == null)
        {
            Debug.LogError("StepCounter requires a TMP_Text reference.", this);
        }

        if (scriptMachine == null)
        {
            Debug.LogError("StepCounter requires a ScriptMachine on the same GameObject.", this);
        }

        UpdateStepsText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
            return;
        }

        if (gameEnded)
        {
            return;
        }

        Vector3 movement = transform.position - lastCountedPosition;
        movement.y = 0f;

        if (movement.magnitude <= minMoveDistance)
        {
            return;
        }

        lastCountedPosition = transform.position;

        switch (GetTileTypeBelowPlayer())
        {
            case TileType.Banana:
                Steps += 2;
                UpdateStepsText();
                break;
            case TileType.Radiation:
                EndGame("KONEC HRY");
                break;
            case TileType.Finish:
                Steps++;
                EndGame($"CÍL\nKroky: {Steps}");
                break;
            default:
                Steps++;
                UpdateStepsText();
                break;
        }
    }

    private TileType GetTileTypeBelowPlayer()
    {
        Vector3 rayOrigin = transform.position + Vector3.up;
        RaycastHit[] hits = Physics.RaycastAll(rayOrigin, Vector3.down, Mathf.Infinity);

        Tile nearestTile = null;
        float nearestDistance = Mathf.Infinity;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null || IsPlayerCollider(hit.collider))
            {
                continue;
            }

            Tile tile = hit.collider.GetComponentInParent<Tile>();
            if (tile != null && hit.distance < nearestDistance)
            {
                nearestTile = tile;
                nearestDistance = hit.distance;
            }
        }

        return nearestTile != null ? nearestTile.Type : TileType.Normal;
    }

    private bool IsPlayerCollider(Collider collider)
    {
        Transform colliderTransform = collider.transform;
        return colliderTransform == transform || colliderTransform.IsChildOf(transform);
    }

    private void EndGame(string resultText)
    {
        gameEnded = true;

        if (scriptMachine != null)
        {
            scriptMachine.enabled = false;
        }

        if (stepsText != null)
        {
            stepsText.text = resultText;
        }
    }

    private void RestartGame()
    {
        transform.SetPositionAndRotation(startPosition, startRotation);
        lastCountedPosition = startPosition;
        Steps = 0;
        gameEnded = false;

        if (scriptMachine != null)
        {
            scriptMachine.enabled = true;
        }

        UpdateStepsText();
    }

    private void UpdateStepsText()
    {
        if (stepsText != null)
        {
            stepsText.text = $"Kroky: {Steps}";
        }
    }
}
