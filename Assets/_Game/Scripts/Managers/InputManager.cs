using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    private Vector2 firstClickPos = Vector2.zero;
    private Vector2 lastClickPos = Vector2.zero;
    private float delta_x, delta_y;
    
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void SetLastClickPos()
    {
        lastClickPos.x = Input.mousePosition.x;
        lastClickPos.y = Input.mousePosition.y;
    }

    private void SetFirstClickPos()
    {
        firstClickPos.x = Input.mousePosition.x;
        firstClickPos.y = Input.mousePosition.y;
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetLastClickPos();
        }

        if (Input.GetMouseButton(0))
        {
            delta_x = (Input.mousePosition.x - lastClickPos.x) / Screen.width;
            delta_y = (Input.mousePosition.y - lastClickPos.y) /Screen.width;

            SetLastClickPos();
        }

        if (Input.GetMouseButtonUp(0))
        {
            firstClickPos = Vector2.zero;
            lastClickPos = Vector2.zero;
            delta_x = 0;
            delta_y = 0;
        }
    }

    public float GetDeltaX()
    {
        return delta_x;
    }


    public float GetDeltaY()
    {
        return delta_y;
    }
}