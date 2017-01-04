using System;
using UnityEngine;

public class RandomScale : MonoBehaviour, IPooledMonoBehaviour
{
    private bool bGetted;
    public float maxScale = 2f;
    public float minScale = 1f;

    public void OnCreate()
    {
    }

    public void OnGet()
    {
        if (!this.bGetted)
        {
            this.bGetted = true;
            float x = UnityEngine.Random.Range(this.minScale, this.maxScale);
            base.transform.localScale = new Vector3(x, x, x);
        }
    }

    public void OnRecycle()
    {
        this.bGetted = false;
    }

    private void Start()
    {
        this.OnGet();
    }
}

