using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string scene;

    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(0.5f, 0.5f, 0.5f, 1);
    }

    private void OnMouseEnter()
    {
        sprite.color = new Color(1, 1, 1, 1);
    }

    private void OnMouseExit()
    {
        sprite.color = new Color(0.5f, 0.5f, 0.5f, 1);
    }

    private void OnMouseDown()
    {
        SceneManager.LoadScene(scene);
    }
}
