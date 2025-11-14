using UnityEngine;
using UnityEngine.SceneManagement;

public class RisingDrowningZone : MonoBehaviour
{
    [Header("Camera & Players")]
    public Camera targetCamera;
    public Transform playersRoot;

    [Header("Rising Height Settings")]
    public float heightOffsetFromCameraBottom = 0f;
    public float riseSpeed = 1f;
    public float cameraRange = 30f;
    public float cameraRelocateDelay = 10f;

    [Header("Drowning Settings")]
    public GameObject drowningPanel;
    public float drowningDelay = 10f;
    public string gameOverSceneName = "GameOver";

    [Header("Debug")]
    public bool drawGizmos = true;
    public Color gizmoColor = Color.cyan;

    private float _currentHeight;
    private float _cameraOutTimer = 0f;
    private float _drowningTimer = 0f;

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera != null)
        {
            _currentHeight = GetCameraBottomY() + heightOffsetFromCameraBottom;
        }
        else
        {
            Debug.LogWarning("[RisingDrowningZone] No camera assigned and Camera.main is null. Height starts at 0.");
            _currentHeight = 0f;
        }

        if (drowningPanel != null)
        {
            drowningPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (targetCamera == null)
            return;

        UpdateRisingHeight();
        UpdateCameraRangeLogic();
        UpdateDrowningLogic();
    }

    private void UpdateRisingHeight()
    {
        _currentHeight += riseSpeed * Time.deltaTime;
    }

    private void UpdateCameraRangeLogic()
    {
        float cameraBottomY = GetCameraBottomY();
        float distance = Mathf.Abs(cameraBottomY - _currentHeight);

        if (distance > cameraRange)
        {
            _cameraOutTimer += Time.deltaTime;

            if (_cameraOutTimer >= cameraRelocateDelay)
            {
                _currentHeight = cameraBottomY + heightOffsetFromCameraBottom;
                _cameraOutTimer = 0f;
            }
        }
        else
        {
            _cameraOutTimer = 0f;
        }
    }

    private void UpdateDrowningLogic()
    {
        bool anyPlayerBelow = IsPlayerBelowHeight();

        if (anyPlayerBelow)
        {
            if (drowningPanel != null && !drowningPanel.activeSelf)
                drowningPanel.SetActive(true);

            _drowningTimer += Time.deltaTime;

            if (_drowningTimer >= drowningDelay)
                TriggerGameOver();
        }
        else
        {
            if (drowningPanel != null && drowningPanel.activeSelf)
                drowningPanel.SetActive(false);

            _drowningTimer = 0f;
        }
    }

    private bool IsPlayerBelowHeight()
    {
        if (playersRoot == null)
            return false;

        for (int i = 0; i < playersRoot.childCount; i++)
        {
            Transform child = playersRoot.GetChild(i);
            if (!child.gameObject.activeInHierarchy)
                continue;

            if (child.position.y < _currentHeight)
                return true;
        }
        return false;
    }

    private float GetCameraBottomY()
    {
        return targetCamera.transform.position.y;
    }

    private void TriggerGameOver()
    {
        if (string.IsNullOrEmpty(gameOverSceneName))
        {
            Debug.LogError("[RisingDrowningZone] Game over scene name not set.");
            return;
        }

        SceneManager.LoadScene(gameOverSceneName);
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos)
            return;

        Gizmos.color = gizmoColor;

        float y = Application.isPlaying ? _currentHeight : 0f;

        Gizmos.DrawLine(new Vector3(-100f, y, 0f), new Vector3(100f, y, 0f));
    }
}