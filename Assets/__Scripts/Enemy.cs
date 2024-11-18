using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    public float score = 100;
    public float showDamageDuration = 0.1f;
    public float powerUpDropChance = 1f;

    [Header("Set Dynamucally: Enemy")]
    public Color[] originalColors;
    public Material[] materials; //Все материалы объекта и его потомков
    public bool showingDamage = false;
    public float damageDoneTime; //Время  прекращения отображения эфекта
    public bool notifiedOfDestruction = false;

    protected BoundsCheck bndCheck;
    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        //Получить мфтериалы и цвет этого игрового объекта и его потомков
        materials = Utils.GetMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    void Update()
    {
        Move();

        if (showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }

        if (bndCheck != null && bndCheck.offDown)
        {
            Destroy(gameObject);
        }
    }

    public Vector3 pos 
    {
        get {return (this.transform.position);}
        set {this.transform.position = value;}
    }
    public virtual void Move()
    {
        Vector3 temPos = pos;
        temPos.y -= speed * Time.deltaTime;
        pos = temPos;
    }

    private void OnCollisionEnter(Collision coll)
    {
        GameObject otherGo = coll.gameObject;
        switch (otherGo.tag)
        {
            case "ProjectileHero":
                Projectile p = otherGo.GetComponent<Projectile>();
                if (!bndCheck.isOnScreen)
                {
                    Destroy (otherGo);
                    break;
                }

                ShowDamage();
                health -= Main.GetWeaponeDefinition(p.type).damageOnHit;
                if (health <= 0)
                {
                    if (!notifiedOfDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }
                    notifiedOfDestruction = true;
                    Destroy(this.gameObject);
                }
                Destroy(otherGo); 
                break;
            default:
                print("Enemy hit by non-ProjectileHero: " + otherGo.name);
                break;  
        }
    }

    void ShowDamage()
    {
        foreach (Material m in materials)
        {
            m.color = Color.yellow;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    void UnShowDamage()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;  
    }
}
