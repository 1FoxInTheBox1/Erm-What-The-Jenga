using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildArea : MonoBehaviour
{
    private Collider2D area;
    private SpriteRenderer sprite;

    private void Start()
    {
        area = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Show()
    {
        area.enabled = true;
        sprite.enabled = true;
    }

    public void Hide()
    {
        area.enabled = false;
        sprite.enabled = false;
    }
}
