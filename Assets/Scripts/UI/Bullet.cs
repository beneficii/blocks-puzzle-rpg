using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FancyToolkit;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 10;
    public SpriteRenderer render;

    System.Action<Unit> action;
    Unit target;
    int damage;

    float launchAt = -1f;

    public Bullet Init(Unit target, System.Action<Unit> action)
    {
        this.target = target;
        this.action = action;

        return this;
    }

    public Bullet Init(Unit target, int damage)
    {
        this.target = target;
        this.damage = damage;

        return this;
    }

    public Bullet SetSprite(Sprite sprite)
    {
        render.sprite = sprite;
        return this;
    }

    public Bullet SetLaunchDelay(float delay)
    {
        launchAt = Time.time + delay;
        return this;
    }

    private void Update()
    {
        if (!target)
        {
            Destroy(gameObject);
            return;
        }

        //transform.up = transform.position - target.transform.position;
    }

    private void FixedUpdate()
    {
        if (!target || !target.gameObject.activeSelf)
        {
            Destroy(gameObject);
            return;
        }

        if (Time.time < launchAt) return;


        if (transform.MoveTowards(target.transform.position, Time.fixedDeltaTime * speed))
        {
            if (action != null)
            {
                action.Invoke(target);
            }
            else
            {
                if (damage >= 0)
                {
                    target.RemoveHp(damage);
                }
                else
                {
                    target.AddHp(-damage);
                }
                
            }
            Destroy(gameObject);
        }
    }
}