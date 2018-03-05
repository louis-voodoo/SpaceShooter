﻿using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class UIElement : MonoBehaviour
{

    protected Coroutine _showHideCoroutine = null;
    protected bool _visible = false;
    private Animator __anim;
    protected Animator _anim
    {
        get
        {
            if (__anim == null)
                __anim = GetComponent<Animator>();
            return (__anim);
        }
        set
        {
            __anim = value;
        }
    }
    protected CanvasGroup _canvasGroup;

    public bool Showing { get; private set; }
    public bool Hiding { get; private set; }

    public virtual void ShowEarly()
    {

    }

    public virtual void Show()
    {

        if (_showHideCoroutine != null)
            StopCoroutine(_showHideCoroutine);
        _visible = true;
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
        Showing = false;
    }

    public virtual void Hide()
    {

        if (_showHideCoroutine != null)
            StopCoroutine(_showHideCoroutine);
        _visible = false;
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        Hiding = false;
    }

    public virtual void ToggleVisible()
    {
        if (_visible == true)
            Hide();
        else
            Show();
    }

    IEnumerator SlowHide(float time, Delegate del = null)
    {
        float _animTime = 0f;
        Hiding = true;

        while (_animTime < time)
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 1f - (_animTime / time);
            yield return null;
            _animTime += Time.deltaTime;
        }
        _canvasGroup.alpha = 0f;
        Hide();
        if (del != null)
            del.DynamicInvoke();
    }
    public virtual void Hide(float time, Delegate del = null)
    {
        if (_showHideCoroutine != null)
            StopCoroutine(_showHideCoroutine);
        _showHideCoroutine = StartCoroutine(SlowHide(time, del));
    }

    IEnumerator SlowShow(float time, Delegate del = null)
    {
        Showing = true;
        float _animTime = 0f;
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
        while (_animTime < time)
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = _animTime / time;
            yield return null;
            _animTime += Time.deltaTime;
        }
        Show();
        if (del != null)
            del.DynamicInvoke();

    }

    public void Show(float time, Delegate del = null)
    {
        ShowEarly();
        if (_showHideCoroutine != null)
            StopCoroutine(_showHideCoroutine);
        _showHideCoroutine = StartCoroutine(SlowShow(time, del));
    }

	public bool IsVisible()
	{
		return (_visible);
	}
}