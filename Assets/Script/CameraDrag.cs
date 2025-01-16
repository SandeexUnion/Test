using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    private Vector3 dragOrigin;
    private bool isDragging = false;
    public float smoothSpeed = 0.1f; // Скорость перемещения
    public DragAndDrop dragAndDrop;
    // Границы перемещения камеры
    public float minX = -10f; // Минимальное значение по оси X
    public float maxX = 10f;  // Максимальное значение по оси X

    private void Start()
    {
        
    }
    void Update()
    {
        // Проверка нажатий мыши или касаний
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            isDragging = true;
            
            dragOrigin = Input.GetMouseButtonDown(0) ? Input.mousePosition : Input.GetTouch(0).position;
        }

        // Перетаскивание камеры
        if (isDragging && dragAndDrop.isDragging == false)
        {
            Vector3 mousePosition;
            if (Input.GetMouseButton(0))
            {
                mousePosition = Input.mousePosition;
            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                mousePosition = Input.GetTouch(0).position;
            }
            else
            {
                return; // Если нет касания или нажатия, выходим
            }

            Vector3 direction = dragOrigin - mousePosition;

            // Перемещение камеры только по оси X
            Vector3 move = new Vector3(direction.x * smoothSpeed, 0, 0);
            transform.position += move; // Обновляем позицию камеры

            // Ограничение перемещения камеры в заданных границах
            float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);

            dragOrigin = mousePosition; // Обновляем начальную позицию
        }

        // Завершение перетаскивания
        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            isDragging = false;
        }
    }
}
