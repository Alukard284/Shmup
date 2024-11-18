using UnityEngine;

// ���������� ������������ ����� ������
public enum WeaponType
{
    none,   // ��� ������
    blaster, // �������
    spread,  // �������
    phaser,  // �����
    missile, // ������
    laser,   // �����
    shield   // ���
}

/// <summary>
/// ����� WeaponDefinition ��������� ����������� ��������
/// ����������� ���� ������ � ����������. ��� ����� ����� Main
/// ����� ������� ������ ��������� ���� WeaponDefinition
/// </summary>
[System.Serializable]
public class WeaponeDefinition
{
    public WeaponType type = WeaponType.none; // ��� ������
    public string letter; // ����� �� ������, ������������ �����
    public Color color = Color.white; // ���� ������ � ������ ������
    public GameObject projectilePrefab; // ������ �������
    public Color projectileColor = Color.white; // ���� �������
    public float damageOnHit = 0; // ���� ��� ���������
    public float continuousDamage = 0; // ����������� ����
    public float delayBetweenShots = 0; // �������� ����� ����������
    public float velocity = 20; // �������� �������
    public AudioClip shootSound; // ���� ��������
}

public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILR_ANCHOR; // ����� ��� ��������

    [Header("Set Dynamically")]
    private WeaponType _type = WeaponType.none; // ������� ��� ������
    public WeaponeDefinition def; // ����������� ���� ������
    public GameObject collar; // ������, �������� ��������� �������
    public float lastShotTime; // ����� ���������� ��������
    private Renderer collarRend; // �������� "���������" (����� ���� ����������� ��� ������������)
    public ParticleSystem localMuzzleFlash;
    [SerializeField] private AudioSource _source; // ����� �������� ��� ������

    private void Start()
    {
        // ���� "��������" � "�������" ��� ������
        collar = transform.Find("Collar").gameObject;
        localMuzzleFlash = transform.Find("MuzzleFlash").GetComponentInChildren<ParticleSystem>();
        _type = def.type;

        // ������������� ��� ������
        SetType(_type);

        // �������������� PROJECTILR_ANCHOR, ���� �� ��� �� ������
        if (PROJECTILR_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILR_ANCHOR = go.transform;
        }

        // ������������� �� ������� �������� �� ������� Hero
        GameObject rootGO = transform.root.gameObject;
        if (rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
    }

    // �������� ��� ���������/��������� ���� ������
    public WeaponType type
    {
        get { return (_type); }
        set { SetType(value); }
    }

    // ����� ��� ��������� ���� ������
    public void SetType(WeaponType wt)
    {
        _type = wt;
        // ���� ��� ������, ������������ ������
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true); // ���������� ������
        }

        // �������� ����������� ������ �� �������� ������
        def = Main.GetWeaponeDefinition(_type);
        var mainModule = localMuzzleFlash.main;
        mainModule.startColor = def.color;

        lastShotTime = 0; // ���������� ����� ���������� ��������
    }

    // ����� ��� ��������
    public void Fire()
    {
        // ���� ������ �� �������, ������� �� ������
        if (!gameObject.activeInHierarchy) return;

        // ���������, ���������� �� ������� ������ ����� ����������
        if (Time.time - lastShotTime < def.delayBetweenShots)
        {
            return;
        }

        // ������������� ���� �������� � ������ �������, ���� ��� ������
        if (_source != null && def.shootSound != null)
        {
            _source.PlayOneShot(def.shootSound); // ����������� ����
            localMuzzleFlash.Play(); // ������������� ������ �������
        }

        Projectile p; // ��������� ���������� ��� �������
        Vector3 vel = Vector3.up * def.velocity; // ��������� �������� �������

        // ��������� ������ ����� ������
        switch (type)
        {
            case WeaponType.blaster:
                p = MakeProjectile(); // ������� ������
                p.rigid.velocity = vel; // ������������� ��� ��������
                break;
            case WeaponType.spread:
                // ������� ��������� �������� � ���������
                p = MakeProjectile();
                p.rigid.velocity = vel; // ������������� �������� ������� �������
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back); // ������������ ������ ������
                p.rigid.velocity = p.transform.rotation * vel; // ������������� �������� ������� �������
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back); // ������������ ������ ������
                p.rigid.velocity = p.transform.rotation * vel; // ������������� �������� �������� �������
                break;
        }
    }

    // ����� ��� �������� �������
    public Projectile MakeProjectile()
    {
        // ������� ����� ������ �������
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);

        // ������������� ��� � ���� � ����������� �� ������������� �������
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

        // ������������� ������� ������� �� "��������"
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILR_ANCHOR, true); // ����������� �������� ��� �������
        Projectile p = go.GetComponent<Projectile>(); // �������� ��������� Projectile
        p.type = type; // ������������� ��� �������
        lastShotTime = Time.time; // ��������� ����� ���������� ��������
        return (p); // ���������� ��������� ������
    }
}
