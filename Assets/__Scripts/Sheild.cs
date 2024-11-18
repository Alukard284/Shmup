using UnityEngine;

public class Sheild : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float rotationPerSecond = 0.1f;

    [Header("Set Dynamically")]
    public int LevelShown = 0;
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        int currentLevel = Mathf.FloorToInt(Hero.S.sheildLevel);
        if (LevelShown != currentLevel)
        {
            LevelShown = currentLevel;
            //—коректировать смещение в текстурк, что бы отобразить поле с другой мощьностью
            mat.mainTextureOffset = new Vector2(0.2f * LevelShown, 0);
        }
        //ѕоворачитвать поле в каждом кадре с посто€нной скоростью
        float rZ = -(rotationPerSecond * Time.time * 360) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rZ);
    }
}
