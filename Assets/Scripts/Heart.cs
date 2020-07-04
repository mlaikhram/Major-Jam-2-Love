using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public float growRate;
    public float floatRate;
    public float fadeRate;
    public float wobbleWidth;
    public float wobbleRate;

    private Vector3 spawnCoordinate;
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        spawnCoordinate = transform.position;
        sprite = GetComponent<SpriteRenderer>();
        ResetPosition();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localScale = new Vector3(transform.localScale.x + growRate * Time.deltaTime, transform.localScale.y + growRate * Time.deltaTime, 1);
        transform.position = new Vector3(spawnCoordinate.x + wobbleWidth * Mathf.Sin(2 * Mathf.PI * wobbleRate * Time.time), transform.position.y + floatRate * Time.deltaTime, 0);
        sprite.color = new Color(1, 1, 1, sprite.color.a - fadeRate * Time.deltaTime);
        if (sprite.color.a <= 0)
        {
            ResetPosition();
        }
    }

    private void ResetPosition()
    {
        transform.position = spawnCoordinate;
        transform.localScale = new Vector3(0, 0, 1);
        sprite.color = new Color(1, 1, 1, 1);
    }
}
