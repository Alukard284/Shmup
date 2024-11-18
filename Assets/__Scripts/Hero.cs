using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S;
    [Header("Set in Inspector")]
    public float speed = 30f;
    public float rollMult = -45f;
    public float pitchMult = 30f;
    public float gameRestartDlay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;

    [Header("Set Dynamicaly")]
    [SerializeField] private float _sheildLevel = 1;

    //эта переменная хранит ссылку на последний столкнувшийся игровой обьект
    private GameObject lastTriggerGo = null;

    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;

    private void Start()
    {
        if (S == null)
        {
            S= this;
        }
        else
        {
            Debug.Log("Hero.Awake() - Attemted to assigin second Hero.S!");
        }
    }

    private void Update()
    {
        PlayerInput();

        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {
            fireDelegate();
        }
    }

    private void PlayerInput()
    {   //Получаем Ввод игрока
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");
        //Изменить transform, опираясь на иформацию по осям
        Vector3 pos = transform.position;   
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;
        //Поверншть корабль, что бы придать динамизма
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        print("Triggered: " + go.name);

        //Гарантировать невозможность повторного столкновения с тем же обектом
        if (go == lastTriggerGo)
        {
            return;
        }
        lastTriggerGo = go;

        if (go.tag == "Enemy")
        {
            sheildLevel--;
            Destroy(go);
        }
        else if (go.tag == "PowerUp")
        {
            AbsorbPowerUp(go);
        }
        else
        {
            print("Triggered by non-Enemy: " + go.name);
        }
    }

    public float sheildLevel
    {
        get { 
            return (_sheildLevel); 
        }
        set { 
            _sheildLevel = Mathf.Min(value, 4);
            if (value < 0)
            {
                Destroy(this.gameObject);
                Main.S.DelayedRestart(gameRestartDlay);
            }
        }
    }

    Weapon GetWeaponSlot()
    {
        for (int i =0; i < weapons.Length; i++)
        {
            if (weapons[i].type == WeaponType.none)
            {
                return (weapons[i]);
            }
        }
        return (null);
    }

    void ClearWeapons()
    {
        foreach (Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }

    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield:
                sheildLevel++;
                break;

            default:
                if (pu.type == weapons[0].type)
                {
                    Weapon w = GetWeaponSlot();
                    if (w != null)
                    {
                        w.SetType(pu.type);
                    }
                }
                else
                {
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }

        pu.AbsorberedBy(this.gameObject);
    }
}
