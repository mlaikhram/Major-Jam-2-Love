using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstructionButton : MonoBehaviour
{
    public Obstruction obstruction;

    private SpriteRenderer sprite;
    private Color defaultColor;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        defaultColor = new Color(0.5f, 0.5f, 0.5f, 1);
    }

    private void FixedUpdate()
    {
        if (Player.instance.MouseState == obstruction)
        {
            sprite.color = new Color(1, 1, 1, 0.8f - 0.6f * Mathf.Sin(2.5f * Mathf.PI * Time.time));
        }
        else
        {
            sprite.color = defaultColor;
        }
    }

    private void OnMouseEnter()
    {
        if (Player.instance.MouseState != obstruction)
        {
            defaultColor = new Color(1, 1, 1, 1);
        }
    }

    private void OnMouseExit()
    {
        defaultColor = new Color(0.5f, 0.5f, 0.5f, 1);
    }

    private void OnMouseDown()
    {
        Player.instance.SelectObstruction(Player.instance.MouseState == obstruction ? Obstruction.NONE : obstruction);
        defaultColor = new Color(1, 1, 1, 1);
    }
}
