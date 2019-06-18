using System;
using System.Collections;
using System.Collections.Generic;
using SW;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    public Image[] bgImages;

    public GameManager manager;

    private float _opacity = 0;

    public float Opacity
    {
        get { return _opacity; }
        set
        {
            _opacity = value;
            foreach (Image image in bgImages)
            {
                Color color = image.color;
                color.a = _opacity;
                image.color = color;
            }
        }
    }

    private string _current;

    void SetBackground(string id)
    {
        _current = id;
        SetBackground(manager.GetTexture(id));
    }

    void SetBackground(Sprite texture = null)
    {
        foreach (Image image in bgImages)
        {
            image.enabled = texture != null;
            image.sprite = texture;
        }
    }

    public void FadeInOut(float time, string id, Action onComplete = null)
    {
        if (_current == id)
        {
            onComplete?.Invoke();
        }
        else
        {
            if (_opacity <= 0)
            {
                FadeOut(time, id, onComplete);
            }
            else
            {
                FadeIn(time, () => { FadeOut(time, id, onComplete); });
            }
        }
    }

    public void FadeIn(float time, Action action = null)
    {
        Tween.Value(time).From(_opacity).To(0).OnUpdate(f => { Opacity = f; })
            .OnComplete(action)
            .Start();
    }

    void FadeOut(float time, string id, Action onComplete = null)
    {
        SetBackground(id);
        Tween.Value(time).From(_opacity).To(1).OnUpdate(f => { Opacity = f; }).OnComplete(onComplete).Start();
    }
}