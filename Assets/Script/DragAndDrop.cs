using System.Collections;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCamera;
    public bool isDragging = false;
    private Vector3 originalScale;
    private bool isOnShelf = false; // ����, �����������, ��������� �� ������ �� �����
    public float cameraMoveSpeed = 0.1f;

    // ��������� �������������� ����
    public float fixedWindowWidth = 300f; // ������ ���� � ��������
    public float fixedWindowHeight = 200f; // ������ ���� � ��������

    // ����������� ����������� �������
    public float minX = -10f; // ����������� �������� �� ��� X
    public float maxX = 10f;  // ������������ �������� �� ��� X
    public float minY = -5f;  // ����������� �������� �� ��� Y
    public float maxY = 5f;   // ������������ �������� �� ��� Y

    void Start()
    {
        mainCamera = Camera.main;
        originalScale = transform.localScale; // ��������� ������������ ������ �������
    }

    void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPosition();
        StartCoroutine(Grow()); // ����������� ������ �������, ����� ����� ��� � ����
    }

    void OnMouseUp()
    {
        isDragging = false;

        // �������� �� ������������ � �������
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Shelf")) // ���������, ��� � ����� ���������� ��� "Shelf"
            {
                // ������������� ������� �������� ������ �����
                transform.position = new Vector3(collider.transform.position.x, collider.transform.position.y, transform.position.z);
                isOnShelf = true; // ������������� ����, ��� ������ �� �����
                transform.localScale = originalScale * 0.5f; // ��������� ������ �������
                return;
            }
        }

        // ���� �� ����������� � ������, ���������� ������ � ������������� �������
        StartCoroutine(ResetSize());
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 newPosition = GetMouseWorldPosition() + offset;
            newPosition.z = 0; // ���������, ��� ������� �� ������ � �������

            // ����������� ����������� ������� � �������� ��������
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            transform.position = newPosition;

            // ���������� ����� ������
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            float halfWidth = fixedWindowWidth / 2;
            float halfHeight = fixedWindowHeight / 2;

            // ���������, ��������� �� ������ ��� �������������� ����
            if (Mathf.Abs(Input.mousePosition.x - screenCenter.x) > halfWidth ||
                Mathf.Abs(Input.mousePosition.y - screenCenter.y) > halfHeight)
            {
                // ����������� ������
                Vector3 cameraTargetPosition = mainCamera.transform.position;
                cameraTargetPosition.x = Mathf.Lerp(cameraTargetPosition.x, newPosition.x, cameraMoveSpeed);
                mainCamera.transform.position = cameraTargetPosition;
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = mainCamera.nearClipPlane; // ���������� �������� z ��� ��������������
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }

    private IEnumerator Grow()
    {
        // ����������� ������ �������
        float growDuration = 0.2f; // ����� ����������
        Vector3 targetScale = originalScale * 1.2f; // ����������� �� 120% ������������� �������
        float elapsedTime = 0;

        while (elapsedTime < growDuration)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, elapsedTime / growDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // ���� ��������� ����
        }

        // ������������� ������������� ������
        transform.localScale = targetScale;
    }

    private IEnumerator ResetSize()
    {
        // ���������� ������ � ������������� �������
        float resetDuration = 0.2f; // ����� ������
        float elapsedTime = 0;

        while (elapsedTime < resetDuration)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, elapsedTime / resetDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // ���� ��������� ����
        }

        // ������������� ������������� ������
        transform.localScale = originalScale;
    }
}
