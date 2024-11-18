using UnityEngine;

public class BoundsCheck : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float radius;
    public bool keepOnScreen = true;

    [Header("Set Dynamicaly")]
    public bool isOnScreen = true;
    public float camWidth;
    public float camHeight;

    [HideInInspector]
    public bool offRigth, offLeft, offUp, offDown;
    
    /// <summary>
    /// Предотврвщает выход игрового обьекта зп границы экрана
    /// Важно: работает ТОЛЬКО с ортографическиой камерой Main Camera в [0, 0, 0]
    /// </summary>
    void Start()
    {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        isOnScreen = true;
        offRigth = offLeft= offUp = offDown = false;

        if (pos.x > camWidth - radius)
        {
            pos.x = camWidth - radius;
            offRigth = true;
        }
        if (pos.x < -camWidth + radius)
        {
            pos.x = -camWidth + radius;
            offLeft = true;
        }
        if (pos.y > camHeight - radius)
        {
            pos.y = camHeight - radius;
            offUp = true;
        }
        if (pos.y < -camHeight + radius)
        {
            pos.y = -camHeight + radius;
            offDown = true;
        }
        isOnScreen = !(offRigth || offLeft || offUp || offDown);
        if (keepOnScreen && !isOnScreen)
        {
            transform.position = pos;
            isOnScreen = true;
            offRigth = offLeft = offUp = offDown = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        Vector3 boundSize = new Vector3 (camWidth *2, camHeight *2, 0.1f);
        Gizmos.DrawCube(Vector3.zero, boundSize);
        
    }
}
