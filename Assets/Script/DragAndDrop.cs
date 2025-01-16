using System.Collections;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCamera;
    public bool isDragging = false;
    private Vector3 originalScale;
    private bool isOnShelf = false; // Флаг, указывающий, находится ли объект на полке
    public float cameraMoveSpeed = 0.1f;

    // Параметры фиксированного окна
    public float fixedWindowWidth = 300f; // Ширина окна в пикселях
    public float fixedWindowHeight = 200f; // Высота окна в пикселях

    // Ограничения перемещения объекта
    public float minX = -10f; // Минимальное значение по оси X
    public float maxX = 10f;  // Максимальное значение по оси X
    public float minY = -5f;  // Минимальное значение по оси Y
    public float maxY = 5f;   // Максимальное значение по оси Y

    void Start()
    {
        mainCamera = Camera.main;
        originalScale = transform.localScale; // Сохраняем оригинальный размер объекта
    }

    void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPosition();
        StartCoroutine(Grow()); // Увеличиваем размер объекта, когда берем его в руки
    }

    void OnMouseUp()
    {
        isDragging = false;

        // Проверка на столкновение с полками
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Shelf")) // Убедитесь, что у полок установлен тег "Shelf"
            {
                // Устанавливаем позицию предмета внутри полки
                transform.position = new Vector3(collider.transform.position.x, collider.transform.position.y, transform.position.z);
                isOnShelf = true; // Устанавливаем флаг, что объект на полке
                transform.localScale = originalScale * 0.5f; // Уменьшаем размер объекта
                return;
            }
        }

        // Если не столкнулись с полкой, возвращаем объект к оригинальному размеру
        StartCoroutine(ResetSize());
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 newPosition = GetMouseWorldPosition() + offset;
            newPosition.z = 0; // Убедитесь, что предмет не уходит в глубину

            // Ограничение перемещения объекта в заданных границах
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            transform.position = newPosition;

            // Определяем центр экрана
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            float halfWidth = fixedWindowWidth / 2;
            float halfHeight = fixedWindowHeight / 2;

            // Проверяем, находится ли курсор вне фиксированного окна
            if (Mathf.Abs(Input.mousePosition.x - screenCenter.x) > halfWidth ||
                Mathf.Abs(Input.mousePosition.y - screenCenter.y) > halfHeight)
            {
                // Перемещение камеры
                Vector3 cameraTargetPosition = mainCamera.transform.position;
                cameraTargetPosition.x = Mathf.Lerp(cameraTargetPosition.x, newPosition.x, cameraMoveSpeed);
                mainCamera.transform.position = cameraTargetPosition;
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = mainCamera.nearClipPlane; // Установите значение z для преобразования
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }

    private IEnumerator Grow()
    {
        // Увеличиваем размер объекта
        float growDuration = 0.2f; // Время увеличения
        Vector3 targetScale = originalScale * 1.2f; // Увеличиваем до 120% оригинального размера
        float elapsedTime = 0;

        while (elapsedTime < growDuration)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, elapsedTime / growDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Ждем следующий кадр
        }

        // Устанавливаем окончательный размер
        transform.localScale = targetScale;
    }

    private IEnumerator ResetSize()
    {
        // Возвращаем объект к оригинальному размеру
        float resetDuration = 0.2f; // Время сброса
        float elapsedTime = 0;

        while (elapsedTime < resetDuration)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, elapsedTime / resetDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Ждем следующий кадр
        }

        // Устанавливаем окончательный размер
        transform.localScale = originalScale;
    }
}
