using TMPro;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Set in Inspector")]
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 6f; // Время жизни объекта
    public float fadeTime = 4f; // Время, за которое объект станет полностью прозрачным

    [Header("Set Dynamically")]
    public WeaponType type;
    public GameObject cube;
    public TMP_Text letter;
    public Vector3 rotPerSecond;
    public float birthTime;

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Renderer cubeRend;

    private void Awake()
    {
        cube = transform.Find("Cube").gameObject;
        letter = GetComponent<TMP_Text>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeRend = cube.GetComponent<Renderer>();

        Vector3 vel = Random.onUnitSphere;
        vel.z = 0;
        vel.Normalize();
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        transform.rotation = Quaternion.identity;

        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y));

        birthTime = Time.time;
    }

    private void Update()
    {
        // Поворот объекта
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        // Определяем текущий прогресс времени
        float lifeProgress = (Time.time - birthTime) / lifeTime;

        // Проверяем, не истекло ли время жизни
        if (lifeProgress >= 1)
        {
            Destroy(this.gameObject);
            return;
        }

        // Логика изменения прозрачности
        float fadeProgress = (Time.time - (birthTime + lifeTime)) / fadeTime;
        fadeProgress = Mathf.Clamp01(fadeProgress); // Ограничиваем от 0 до 1

        // Изменяем прозрачность
        if (fadeProgress > 0)
        {
            Color c = cubeRend.material.color;
            c.a = 1f - fadeProgress; // Снижаем прозрачность
            cubeRend.material.color = c;

            // Изменяем прозрачность текста
            Color textColor = letter.color;
            textColor.a = 1f - fadeProgress * 0.5f; // Текст исчезает медленнее
            letter.color = textColor;
        }

        // Если объект вышел за пределы экрана, уничтожаем его
        if (!bndCheck.isOnScreen)
        {
            Destroy(gameObject);
        }
    }

    public void SetType(WeaponType wt)
    {
        WeaponeDefinition def = Main.GetWeaponeDefinition(wt);
        cubeRend.material.color = def.color;
        letter.text = def.letter;
        type = wt;
    }

    public void AbsorberedBy(GameObject target)
    {
        Destroy(this.gameObject);
    }
}