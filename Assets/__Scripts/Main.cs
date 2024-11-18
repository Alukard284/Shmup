using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main S;
    static Dictionary<WeaponType, WeaponeDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
    public GameObject[] prafabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyDefaultPadding = 1.5f;

    public WeaponeDefinition[] weaponeDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[] {WeaponType.blaster,WeaponType.blaster, WeaponType.spread, WeaponType.shield };

    private BoundsCheck bndCheck;

    private void Awake()
    {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();
        Invoke("SpawnEnemy", 1f/enemySpawnPerSecond);

        WEAP_DICT = new Dictionary<WeaponType, WeaponeDefinition>();
        foreach (WeaponeDefinition def in weaponeDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy()
    {
        int ndx = Random.Range(0, prafabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prafabEnemies[ndx]);

        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }
        Vector3 pos = Vector3.zero;
        float Xmin = -bndCheck.camWidth + enemyPadding;
        float Xmax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(Xmin, Xmax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }

    public void DelayedRestart(float delay)
    {
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        SceneManager.LoadScene("_Scene_1");
    }
    
    static public WeaponeDefinition GetWeaponeDefinition (WeaponType wt)
    {
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }
        return (new WeaponeDefinition());
    }

    public void ShipDestroyed(Enemy e)
    {
        //—генерировать бонус с заданной веро€тностью
        if (Random.value < e.powerUpDropChance)
        {
            int idx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[idx];

            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            pu.SetType(puType);

            pu.transform.position = e.transform.position;
        }
    }

}
