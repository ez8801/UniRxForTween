using System.Collections;
using UnityEngine;
using UniRx;

public class SimpleTweener
{
    public enum Method
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut,
        BounceIn,
        BounceOut,
    }

    Method _method;
    float _from;
    float _to;
    float _delay;
    float _duration;

    float _amountPerDelta;
    float _x;
    float _y;
    bool _isFinished;

    System.IDisposable _disposable;

    public SimpleTweener(Method method
        , float from, float to, float delay, float duration
        , System.Action<float> onNext, System.Action onCompleted)
    {
        _method = method;
        _from = from;
        _to = to;
        _delay = delay;
        _duration = duration;

        _amountPerDelta  = (duration > 0f) ? Mathf.Abs(1f / duration) : 1000f;
        _x = 0f;
        _y = 0f;
        _isFinished = false;

        _disposable = Observable.FromCoroutine<float>(AsyncTween)
            .Subscribe(onNext, onCompleted);
    }

    public IEnumerator AsyncTween(System.IObserver<float> observer)
    {
        observer.OnNext(0f);

        if (_delay > 0f)
        {
            float dTime = 0f;
            while (dTime < _delay)
            {
                dTime += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        
        while (_isFinished == false)
        {
            switch (_method)
            {
                case Method.Linear:
                    observer.OnNext(Linear());
                    break;
                case Method.EaseIn:
                    observer.OnNext(EaseIn());
                    break;
                case Method.EaseOut:
                    observer.OnNext(EaseOut());
                    break;
                case Method.EaseInOut:
                    observer.OnNext(EaseInOut());
                    break;
                case Method.BounceIn:
                    observer.OnNext(BounceIn());
                    break;
                case Method.BounceOut:
                    observer.OnNext(BounceOut());
                    break;
            }
            yield return null;
        }

        observer.OnCompleted();
    }

    private void OnNext()
    {
        _x += (_duration == 0f) ? 1f : _amountPerDelta * Time.unscaledDeltaTime;
        _x = Mathf.Clamp01(_x);
        _isFinished = (_duration == 0f || _x >= 1f);
    }

    private float Linear()
    {
        OnNext();
        return Mathf.Lerp(_from, _to, _x);
    }

    private float EaseIn()
    {
        OnNext();
        _y = 1f - Mathf.Sin(0.5f * Mathf.PI * (1f - _x));
        return Mathf.Lerp(_from, _to, _y);
    }

    private float EaseOut()
    {
        OnNext();
        _y = Mathf.Sin(0.5f * Mathf.PI * _x);
        return Mathf.Lerp(_from, _to, _y);
    }

    private float EaseInOut()
    {
        OnNext();

        const float pi2 = Mathf.PI * 2f;
        _y = _x - Mathf.Sin(_x * pi2) / pi2;
        return Mathf.Lerp(_from, _to, _y);
    }

    private float BounceIn()
    {
        OnNext();
        _y = BounceLogic(_x);        
        return Mathf.Lerp(_from, _to, _y);
    }

    private float BounceOut()
    {
        OnNext();
        _y = 1f - BounceLogic(1f - _x);
        return Mathf.Lerp(_from, _to, _y);
    }

    private float BounceLogic(float val)
    {
        if (val < 0.363636f) // 0.363636 = (1/ 2.75)
        {
            val = 7.5685f * val * val;
        }
        else if (val < 0.727272f) // 0.727272 = (2 / 2.75)
        {
            val = 7.5625f * (val -= 0.545454f) * val + 0.75f; // 0.545454f = (1.5 / 2.75) 
        }
        else if (val < 0.909090f) // 0.909090 = (2.5 / 2.75) 
        {
            val = 7.5625f * (val -= 0.818181f) * val + 0.9375f; // 0.818181 = (2.25 / 2.75) 
        }
        else
        {
            val = 7.5625f * (val -= 0.9545454f) * val + 0.984375f; // 0.9545454 = (2.625 / 2.75) 
        }
        return val;
    }
    
    #region EaseIn

    public static SimpleTweener EaseIn(
        float duration
        , System.Action<float> onNext)
    {
        return new SimpleTweener(Method.EaseIn, 0f, 1f, 0f, duration, onNext, null);
    }

    public static SimpleTweener EaseIn(
        float duration
        , System.Action onCompleted)
    {
        return new SimpleTweener(Method.EaseIn, 0f, 1f, 0f, duration, null, onCompleted);
    }

    public static SimpleTweener EaseIn(
        float duration
        , System.Action<float> onNext
        , System.Action onCompleted)
    {
        return new SimpleTweener(Method.EaseIn, 0f, 1f, 0f, duration, onNext, onCompleted);
    }

    public static SimpleTweener EaseIn(
        float from
        , float to
        , float duration
        , System.Action<float> onNext
        , System.Action onCompleted)
    {
        return new SimpleTweener(Method.EaseIn, from, to, 0f, duration, onNext, onCompleted);
    }

    #endregion EaseIn

    #region EaseOut

    public static SimpleTweener EaseOut(float duration
        , System.Action<float> onNext)
    {
        return new SimpleTweener(Method.EaseOut, 0f, 1f, 0f, duration, onNext, null);
    }

    public static SimpleTweener EaseOut(float duration
        , System.Action onCompleted)
    {
        return new SimpleTweener(Method.EaseOut, 0f, 1f, 0f, duration, null, onCompleted);
    }

    public static SimpleTweener EaseOut(float duration
        , System.Action<float> onNext
        , System.Action onCompleted)
    {
        return new SimpleTweener(Method.EaseOut, 0f, 1f, 0f, duration, onNext, onCompleted);
    }

    public static SimpleTweener EaseOut(
        float from
        , float to
        , float delay
        , float duration
        , System.Action<float> onNext
        , System.Action onCompleted)
    {
        return new SimpleTweener(Method.EaseOut, from, to, delay, duration, onNext, onCompleted);
    }

    #endregion EaseOut

    #region EaseInOut

    public static SimpleTweener EaseInOut(float duration
        , System.Action<float> onNext)
    {
        return new SimpleTweener(Method.EaseInOut, 0f, 1f, 0f, duration, onNext, null);
    }

    public static SimpleTweener EaseInOut(float duration
        , System.Action onCompleted)
    {
        return new SimpleTweener(Method.EaseInOut, 0f, 1f, 0f, duration, null, onCompleted);
    }

    public static SimpleTweener EaseInOut(float duration
        , System.Action<float> onNext
        , System.Action onCompleted)
    {
        return new SimpleTweener(Method.EaseInOut, 0f, 1f, 0f, duration, onNext, onCompleted);
    }

    public static SimpleTweener EaseInOut(
        float from
        , float to
        , float duration
        , System.Action<float> onNext
        , System.Action onCompleted)
    {
        return new SimpleTweener(Method.EaseInOut, from, to, 0f, duration, onNext, onCompleted);
    }

    #endregion EaseInOut

    #region BounceIn

    public static SimpleTweener BounceIn(float duration
        , System.Action<float> onNext)
    {
        return new SimpleTweener(Method.BounceIn, 0f, 1f, 0f, duration, onNext, null);
    }

    public static SimpleTweener BounceIn(float duration
        , System.Action onCompleted)
    {
        return new SimpleTweener(Method.BounceIn, 0f, 1f, 0f, duration, null, onCompleted);
    }

    public static SimpleTweener BounceIn(float duration
        , System.Action<float> onNext
        , System.Action onCompleted)
    {
        return new SimpleTweener(Method.BounceIn, 0f, 1f, 0f, duration, onNext, onCompleted);
    }

    public static SimpleTweener BounceIn(
        float from
        , float to
        , float duration
        , System.Action<float> onNext
        , System.Action onCompleted)
    {
        return new SimpleTweener(Method.BounceIn, from, to, 0f, duration, onNext, onCompleted);
    }
    
    #endregion BounceIn

    #region BounceOut

    public static SimpleTweener BounceOut(float duration
        , System.Action<float> onNext)
    {
        return new SimpleTweener(Method.BounceOut, 0f, 1f, 0f, duration, onNext, null);
    }

    public static SimpleTweener BounceOut(float duration
        , System.Action onCompleted)
    {
        return new SimpleTweener(Method.BounceOut, 0f, 1f, 0f, duration, null, onCompleted);
    }

    public static SimpleTweener BounceOut(float duration
        , System.Action<float> onNext
        , System.Action onCompleted)
    {
        return new SimpleTweener(Method.BounceOut, 0f, 1f, 0f, duration, onNext, onCompleted);
    }

    public static SimpleTweener BounceOut(
        float from
        , float to
        , float duration
        , System.Action<float> onNext
        , System.Action onCompleted)
    {
        return new SimpleTweener(Method.BounceOut, from, to, 0f, duration, onNext, onCompleted);
    }

    #endregion BounceOut
}