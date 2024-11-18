using UnityEngine;

// Определяем перечисление типов оружия
public enum WeaponType
{
    none,   // Нет оружия
    blaster, // Бластер
    spread,  // Разброс
    phaser,  // Фазер
    missile, // Ракета
    laser,   // Лазер
    shield   // Щит
}

/// <summary>
/// Класс WeaponDefinition позволяет настраивать свойства
/// конкретного вида оружия в инспекторе. Для этого класс Main
/// будет хранить массив элементов типа WeaponDefinition
/// </summary>
[System.Serializable]
public class WeaponeDefinition
{
    public WeaponType type = WeaponType.none; // Тип оружия
    public string letter; // Буква на кубике, изображающем бонус
    public Color color = Color.white; // Цвет ствола и кубика бонуса
    public GameObject projectilePrefab; // Префаб снаряда
    public Color projectileColor = Color.white; // Цвет снаряда
    public float damageOnHit = 0; // Урон при попадании
    public float continuousDamage = 0; // Непрерывный урон
    public float delayBetweenShots = 0; // Задержка между выстрелами
    public float velocity = 20; // Скорость снаряда
    public AudioClip shootSound; // Звук выстрела
}

public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILR_ANCHOR; // Якорь для снарядов

    [Header("Set Dynamically")]
    private WeaponType _type = WeaponType.none; // Текущий тип оружия
    public WeaponeDefinition def; // Определение типа оружия
    public GameObject collar; // Объект, задающий положение снаряда
    public float lastShotTime; // Время последнего выстрела
    private Renderer collarRend; // Рендерер "воротника" (может быть использован для визуализации)
    public ParticleSystem localMuzzleFlash;
    [SerializeField] private AudioSource _source; // Аудио источник для звуков

    private void Start()
    {
        // Ищем "воротник" и "вспышку" при старте
        collar = transform.Find("Collar").gameObject;
        localMuzzleFlash = transform.Find("MuzzleFlash").GetComponentInChildren<ParticleSystem>();
        _type = def.type;

        // Устанавливаем тип оружия
        SetType(_type);

        // Инициализируем PROJECTILR_ANCHOR, если он еще не создан
        if (PROJECTILR_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILR_ANCHOR = go.transform;
        }

        // Подписываемся на событие стрельбы на объекте Hero
        GameObject rootGO = transform.root.gameObject;
        if (rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
    }

    // Свойство для получения/установки типа оружия
    public WeaponType type
    {
        get { return (_type); }
        set { SetType(value); }
    }

    // Метод для установки типа оружия
    public void SetType(WeaponType wt)
    {
        _type = wt;
        // Если нет оружия, деактивируем объект
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true); // Активируем объект
        }

        // Получаем определение оружия из главного класса
        def = Main.GetWeaponeDefinition(_type);
        var mainModule = localMuzzleFlash.main;
        mainModule.startColor = def.color;

        lastShotTime = 0; // Сбрасываем время последнего выстрела
    }

    // Метод для стрельбы
    public void Fire()
    {
        // Если объект не активен, выходим из метода
        if (!gameObject.activeInHierarchy) return;

        // Проверяем, достаточно ли времени прошло между выстрелами
        if (Time.time - lastShotTime < def.delayBetweenShots)
        {
            return;
        }

        // Воспроизводим звук выстрела и эффект вспышки, если они заданы
        if (_source != null && def.shootSound != null)
        {
            _source.PlayOneShot(def.shootSound); // Проигрываем звук
            localMuzzleFlash.Play(); // Воспроизводим эффект вспышки
        }

        Projectile p; // Объявляем переменную для снаряда
        Vector3 vel = Vector3.up * def.velocity; // Установка скорости снаряда

        // Обработка разных типов оружия
        switch (type)
        {
            case WeaponType.blaster:
                p = MakeProjectile(); // Создаем снаряд
                p.rigid.velocity = vel; // Устанавливаем его скорость
                break;
            case WeaponType.spread:
                // Создаем несколько снарядов с разбросом
                p = MakeProjectile();
                p.rigid.velocity = vel; // Устанавливаем скорость первому снаряду
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back); // Поворачиваем второй снаряд
                p.rigid.velocity = p.transform.rotation * vel; // Устанавливаем скорость второму снаряду
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back); // Поворачиваем третий снаряд
                p.rigid.velocity = p.transform.rotation * vel; // Устанавливаем скорость третьему снаряду
                break;
        }
    }

    // Метод для создания снаряда
    public Projectile MakeProjectile()
    {
        // Создаем новый объект снаряда
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);

        // Устанавливаем тег и слой в зависимости от родительского объекта
        if (transform.parent.gameObject.tag == "Hero")
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }

        // Устанавливаем позицию снаряда на "воротник"
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILR_ANCHOR, true); // Настраиваем родителя для снаряда
        Projectile p = go.GetComponent<Projectile>(); // Получаем компонент Projectile
        p.type = type; // Устанавливаем тип снаряда
        lastShotTime = Time.time; // Обновляем время последнего выстрела
        return (p); // Возвращаем созданный снаряд
    }
}
