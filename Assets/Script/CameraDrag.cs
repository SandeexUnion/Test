using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    private Vector3 dragOrigin;
    private bool isDragging = false;
    public float smoothSpeed = 0.1f; // �������� �����������
    public DragAndDrop dragAndDrop;
    // ������� ����������� ������
    public float minX = -10f; // ����������� �������� �� ��� X
    public float maxX = 10f;  // ������������ �������� �� ��� X

    private void Start()
    {
        
    }
    void Update()
    {
        // �������� ������� ���� ��� �������
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            isDragging = true;
            
            dragOrigin = Input.GetMouseButtonDown(0) ? Input.mousePosition : Input.GetTouch(0).position;
        }

        // �������������� ������
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
                return; // ���� ��� ������� ��� �������, �������
            }

            Vector3 direction = dragOrigin - mousePosition;

            // ����������� ������ ������ �� ��� X
            Vector3 move = new Vector3(direction.x * smoothSpeed, 0, 0);
            transform.position += move; // ��������� ������� ������

            // ����������� ����������� ������ � �������� ��������
            float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);

            dragOrigin = mousePosition; // ��������� ��������� �������
        }

        // ���������� ��������������
        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            isDragging = false;
        }
    }
}
