using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastWave : MonoBehaviour
{
    private LineRenderer lr;
    public int pointsCount;

    public float maxRadius;
    public float speed;

    public float startWidth;
   

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = pointsCount + 1;
    }
    private void Start()
    {
        StartCoroutine(Blast());
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Blast()
    {
        float currentRadius = 0f;

        while(currentRadius < maxRadius)
        {
            currentRadius += Time.deltaTime * speed;
            Draw(currentRadius);
            yield return null;
        }
    }

    private void Draw(float currentRadius)
    {
        float angleBetweenPoints = 360f / pointsCount;

        for(int i = 0; i <= pointsCount; i++)
        {
            float angle = i * angleBetweenPoints * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0f);
            Vector3 position = direction * currentRadius;

            lr.SetPosition(i, position);
        }
        lr.widthMultiplier = Mathf.Lerp(0f, startWidth, 1f - currentRadius / maxRadius);
    }
}
