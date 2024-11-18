using UnityEngine;

public class Enemy_1 : Enemy
{
    //Enemy_1 расширяет класс Enemy
    [Header("Set in Inspector")]
    public float waveFrequency = 2;
    //Ширина синусоиды
    public float waveWidth = 4;
    public float waveRotY = 45;

    private float x0;
    private float birthTime;

    private void Start()
    {
        x0 = pos.x;

        birthTime = Time.time;  
    }

    public override void Move()
    {
        Vector3 tempPos = pos;  
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;  

        //Повернуть немного относительно оси Y
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        base.Move();
    }

}
