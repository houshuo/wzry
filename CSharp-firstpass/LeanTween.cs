using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class LeanTween : MonoBehaviour
{
    private static GameObject _tweenEmpty;
    public static LTDescr descr;
    private static float dt;
    private static float dtActual;
    private static float dtEstimated;
    public static float dtManual;
    private static Action<LTEvent>[] eventListeners;
    public static int EVENTS_MAX;
    private static int eventsMaxSearch;
    private static int finishedCnt;
    private static int frameRendered = -1;
    private static GameObject[] goListeners;
    private static int i;
    private static int INIT_LISTENERS_MAX;
    private static bool isTweenFinished;
    private static int j;
    public static int LISTENERS_MAX;
    private static int maxTweenReached;
    private static int maxTweens = 400;
    private static Vector3 newVect;
    private static float previousRealTime;
    private static AnimationCurve punch;
    private static float ratioPassed;
    private static AnimationCurve shake;
    public static int startSearch;
    public static float tau = 6.283185f;
    public static bool throwErrors = true;
    private static float timeTotal;
    private static Transform trans;
    private static LTDescr tween;
    private static TweenAction tweenAction;
    private static int tweenMaxSearch = -1;
    private static LTDescr[] tweens;
    private static int[] tweensFinished;
    private static float val;

    static LeanTween()
    {
        Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(0.112586f, 0.9976035f), new Keyframe(0.3120486f, -0.1720615f), new Keyframe(0.4316337f, 0.07030682f), new Keyframe(0.5524869f, -0.03141804f), new Keyframe(0.6549395f, 0.003909959f), new Keyframe(0.770987f, -0.009817753f), new Keyframe(0.8838775f, 0.001939224f), new Keyframe(1f, 0f) };
        punch = new AnimationCurve(keys);
        Keyframe[] keyframeArray2 = new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(0.25f, 1f), new Keyframe(0.75f, -1f), new Keyframe(1f, 0f) };
        shake = new AnimationCurve(keyframeArray2);
        startSearch = 0;
        eventsMaxSearch = 0;
        EVENTS_MAX = 10;
        LISTENERS_MAX = 10;
        INIT_LISTENERS_MAX = LISTENERS_MAX;
    }

    public static Vector3[] add(Vector3[] a, Vector3 b)
    {
        Vector3[] vectorArray = new Vector3[a.Length];
        i = 0;
        while (i < a.Length)
        {
            vectorArray[i] = a[i] + b;
            i++;
        }
        return vectorArray;
    }

    public static void addListener(int eventId, Action<LTEvent> callback)
    {
        addListener(tweenEmpty, eventId, callback);
    }

    public static void addListener(GameObject caller, int eventId, Action<LTEvent> callback)
    {
        if (eventListeners == null)
        {
            INIT_LISTENERS_MAX = LISTENERS_MAX;
            eventListeners = new Action<LTEvent>[EVENTS_MAX * LISTENERS_MAX];
            goListeners = new GameObject[EVENTS_MAX * LISTENERS_MAX];
        }
        i = 0;
        while (i < INIT_LISTENERS_MAX)
        {
            int index = (eventId * INIT_LISTENERS_MAX) + i;
            if ((goListeners[index] == null) || (eventListeners[index] == null))
            {
                eventListeners[index] = callback;
                goListeners[index] = caller;
                if (i >= eventsMaxSearch)
                {
                    eventsMaxSearch = i + 1;
                }
                return;
            }
            if ((goListeners[index] == caller) && object.Equals(eventListeners[index], callback))
            {
                return;
            }
            i++;
        }
        Debug.LogError("You ran out of areas to add listeners, consider increasing INIT_LISTENERS_MAX, ex: LeanTween.INIT_LISTENERS_MAX = " + (INIT_LISTENERS_MAX * 2));
    }

    public static LTDescr alpha(LTRect ltRect, float to, float time)
    {
        ltRect.alphaEnabled = true;
        return pushNewTween(tweenEmpty, new Vector3(to, 0f, 0f), time, TweenAction.GUI_ALPHA, options().setRect(ltRect));
    }

    public static LTDescr alpha(GameObject gameObject, float to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ALPHA, options());
    }

    public static LTDescr alpha(RectTransform rectTrans, float to, float time)
    {
        return pushNewTween(rectTrans.gameObject, new Vector3(to, 0f, 0f), time, TweenAction.CANVAS_ALPHA, options().setRect(rectTrans));
    }

    public static LTDescr alphaVertex(GameObject gameObject, float to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ALPHA_VERTEX, options());
    }

    private static void cancel(int uniqueId)
    {
        if (uniqueId >= 0)
        {
            init();
            int index = uniqueId & 0xffff;
            int num2 = uniqueId >> 0x10;
            if (tweens[index].hasInitiliazed && (tweens[index].counter == num2))
            {
                removeTween(index);
            }
        }
    }

    public static void cancel(GameObject gameObject)
    {
        init();
        Transform transform = gameObject.transform;
        for (int i = 0; i <= tweenMaxSearch; i++)
        {
            if (tweens[i].toggle && (tweens[i].trans == transform))
            {
                removeTween(i);
            }
        }
    }

    public static void cancel(LTRect ltRect, int uniqueId)
    {
        if (uniqueId >= 0)
        {
            init();
            int index = uniqueId & 0xffff;
            int num2 = uniqueId >> 0x10;
            if ((tweens[index].ltRect == ltRect) && (tweens[index].counter == num2))
            {
                removeTween(index);
            }
        }
    }

    public static void cancel(GameObject gameObject, int uniqueId)
    {
        if (uniqueId >= 0)
        {
            init();
            int index = uniqueId & 0xffff;
            int num2 = uniqueId >> 0x10;
            if ((tweens[index].trans == null) || ((tweens[index].trans.gameObject == gameObject) && (tweens[index].counter == num2)))
            {
                removeTween(index);
            }
        }
    }

    public static void cancelAll(bool callComplete)
    {
        init();
        for (int i = 0; i <= tweenMaxSearch; i++)
        {
            if (tweens[i].trans != null)
            {
                if (callComplete && (tweens[i].onComplete != null))
                {
                    tweens[i].onComplete();
                }
                removeTween(i);
            }
        }
    }

    private static float clerp(float start, float end, float val)
    {
        float num = 0f;
        float num2 = 360f;
        float num3 = Mathf.Abs((float) ((num2 - num) / 2f));
        float num5 = 0f;
        if ((end - start) < -num3)
        {
            num5 = ((num2 - start) + end) * val;
            return (start + num5);
        }
        if ((end - start) > num3)
        {
            num5 = -((num2 - end) + start) * val;
            return (start + num5);
        }
        return (start + ((end - start) * val));
    }

    public static float closestRot(float from, float to)
    {
        float num = 0f - (360f - to);
        float num2 = 360f + to;
        float num3 = Mathf.Abs((float) (to - from));
        float num4 = Mathf.Abs((float) (num - from));
        float num5 = Mathf.Abs((float) (num2 - from));
        if ((num3 < num4) && (num3 < num5))
        {
            return to;
        }
        if (num4 < num5)
        {
            return num;
        }
        return num2;
    }

    public static LTDescr color(GameObject gameObject, Color to, float time)
    {
        return pushNewTween(gameObject, new Vector3(1f, to.a, 0f), time, TweenAction.COLOR, options().setPoint(new Vector3(to.r, to.g, to.b)));
    }

    public static LTDescr color(RectTransform rectTrans, Color to, float time)
    {
        return pushNewTween(rectTrans.gameObject, new Vector3(1f, to.a, 0f), time, TweenAction.CANVAS_COLOR, options().setRect(rectTrans).setPoint(new Vector3(to.r, to.g, to.b)));
    }

    public static LTDescr delayedCall(float delayTime, Action<object> callback)
    {
        return pushNewTween(tweenEmpty, Vector3.zero, delayTime, TweenAction.CALLBACK, options().setOnComplete(callback));
    }

    public static LTDescr delayedCall(float delayTime, Action callback)
    {
        return pushNewTween(tweenEmpty, Vector3.zero, delayTime, TweenAction.CALLBACK, options().setOnComplete(callback));
    }

    public static LTDescr delayedCall(GameObject gameObject, float delayTime, Action<object> callback)
    {
        return pushNewTween(gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, options().setOnComplete(callback));
    }

    public static LTDescr delayedCall(GameObject gameObject, float delayTime, Action callback)
    {
        return pushNewTween(gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, options().setOnComplete(callback));
    }

    public static LTDescr delayedSound(AudioClip audio, Vector3 pos, float volume)
    {
        return pushNewTween(tweenEmpty, pos, 0f, TweenAction.DELAYED_SOUND, options().setTo(pos).setFrom(new Vector3(volume, 0f, 0f)).setAudio(audio));
    }

    public static LTDescr delayedSound(GameObject gameObject, AudioClip audio, Vector3 pos, float volume)
    {
        return pushNewTween(gameObject, pos, 0f, TweenAction.DELAYED_SOUND, options().setTo(pos).setFrom(new Vector3(volume, 0f, 0f)).setAudio(audio));
    }

    public static LTDescr description(int uniqueId)
    {
        int index = uniqueId & 0xffff;
        int num2 = uniqueId >> 0x10;
        if (((tweens[index] != null) && (tweens[index].uniqueId == uniqueId)) && (tweens[index].counter == num2))
        {
            return tweens[index];
        }
        for (int i = 0; i <= tweenMaxSearch; i++)
        {
            if ((tweens[i].uniqueId == uniqueId) && (tweens[i].counter == num2))
            {
                return tweens[i];
            }
        }
        return null;
    }

    public static LTDescr destroyAfter(LTRect rect, float delayTime)
    {
        return pushNewTween(tweenEmpty, Vector3.zero, delayTime, TweenAction.CALLBACK, options().setRect(rect).setDestroyOnComplete(true));
    }

    public static void dispatchEvent(int eventId)
    {
        dispatchEvent(eventId, null);
    }

    public static void dispatchEvent(int eventId, object data)
    {
        for (int i = 0; i < eventsMaxSearch; i++)
        {
            int index = (eventId * INIT_LISTENERS_MAX) + i;
            if (eventListeners[index] != null)
            {
                if (goListeners[index] != null)
                {
                    eventListeners[index](new LTEvent(eventId, data));
                }
                else
                {
                    eventListeners[index] = null;
                }
            }
        }
    }

    public static void drawBezierPath(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        Vector3 from = a;
        Vector3 vector3 = (-a + ((Vector3) (3f * (b - c)))) + d;
        Vector3 vector4 = (Vector3) ((3f * (a + c)) - (6f * b));
        Vector3 vector5 = (Vector3) (3f * (b - a));
        for (float i = 1f; i <= 30f; i++)
        {
            float num = i / 30f;
            Vector3 to = ((Vector3) (((((vector3 * num) + vector4) * num) + vector5) * num)) + a;
            Gizmos.DrawLine(from, to);
            from = to;
        }
    }

    private static float easeInBack(float start, float end, float val)
    {
        end -= start;
        val /= 1f;
        float num = 1.70158f;
        return ((((end * val) * val) * (((num + 1f) * val) - num)) + start);
    }

    private static float easeInBounce(float start, float end, float val)
    {
        end -= start;
        float num = 1f;
        return ((end - easeOutBounce(0f, end, num - val)) + start);
    }

    private static float easeInCirc(float start, float end, float val)
    {
        end -= start;
        return ((-end * (Mathf.Sqrt(1f - (val * val)) - 1f)) + start);
    }

    private static float easeInCubic(float start, float end, float val)
    {
        end -= start;
        return ((((end * val) * val) * val) + start);
    }

    private static float easeInElastic(float start, float end, float val)
    {
        end -= start;
        float num = 1f;
        float num2 = num * 0.3f;
        float num3 = 0f;
        float num4 = 0f;
        if (val == 0f)
        {
            return start;
        }
        val /= num;
        if (val == 1f)
        {
            return (start + end);
        }
        if ((num4 == 0f) || (num4 < Mathf.Abs(end)))
        {
            num4 = end;
            num3 = num2 / 4f;
        }
        else
        {
            num3 = (num2 / 6.283185f) * Mathf.Asin(end / num4);
        }
        val--;
        return (-((num4 * Mathf.Pow(2f, 10f * val)) * Mathf.Sin((((val * num) - num3) * 6.283185f) / num2)) + start);
    }

    private static float easeInExpo(float start, float end, float val)
    {
        end -= start;
        return ((end * Mathf.Pow(2f, 10f * ((val / 1f) - 1f))) + start);
    }

    private static float easeInOutBack(float start, float end, float val)
    {
        float num = 1.70158f;
        end -= start;
        val /= 0.5f;
        if (val < 1f)
        {
            num *= 1.525f;
            return (((end / 2f) * ((val * val) * (((num + 1f) * val) - num))) + start);
        }
        val -= 2f;
        num *= 1.525f;
        return (((end / 2f) * (((val * val) * (((num + 1f) * val) + num)) + 2f)) + start);
    }

    private static float easeInOutBounce(float start, float end, float val)
    {
        end -= start;
        float num = 1f;
        if (val < (num / 2f))
        {
            return ((easeInBounce(0f, end, val * 2f) * 0.5f) + start);
        }
        return (((easeOutBounce(0f, end, (val * 2f) - num) * 0.5f) + (end * 0.5f)) + start);
    }

    private static float easeInOutCirc(float start, float end, float val)
    {
        val /= 0.5f;
        end -= start;
        if (val < 1f)
        {
            return (((-end / 2f) * (Mathf.Sqrt(1f - (val * val)) - 1f)) + start);
        }
        val -= 2f;
        return (((end / 2f) * (Mathf.Sqrt(1f - (val * val)) + 1f)) + start);
    }

    private static float easeInOutCubic(float start, float end, float val)
    {
        val /= 0.5f;
        end -= start;
        if (val < 1f)
        {
            return (((((end / 2f) * val) * val) * val) + start);
        }
        val -= 2f;
        return (((end / 2f) * (((val * val) * val) + 2f)) + start);
    }

    private static float easeInOutElastic(float start, float end, float val)
    {
        end -= start;
        float num = 1f;
        float num2 = num * 0.3f;
        float num3 = 0f;
        float num4 = 0f;
        if (val == 0f)
        {
            return start;
        }
        val /= num / 2f;
        if (val == 2f)
        {
            return (start + end);
        }
        if ((num4 == 0f) || (num4 < Mathf.Abs(end)))
        {
            num4 = end;
            num3 = num2 / 4f;
        }
        else
        {
            num3 = (num2 / 6.283185f) * Mathf.Asin(end / num4);
        }
        if (val < 1f)
        {
            val--;
            return ((-0.5f * ((num4 * Mathf.Pow(2f, 10f * val)) * Mathf.Sin((((val * num) - num3) * 6.283185f) / num2))) + start);
        }
        val--;
        return (((((num4 * Mathf.Pow(2f, -10f * val)) * Mathf.Sin((((val * num) - num3) * 6.283185f) / num2)) * 0.5f) + end) + start);
    }

    private static float easeInOutExpo(float start, float end, float val)
    {
        val /= 0.5f;
        end -= start;
        if (val < 1f)
        {
            return (((end / 2f) * Mathf.Pow(2f, 10f * (val - 1f))) + start);
        }
        val--;
        return (((end / 2f) * (-Mathf.Pow(2f, -10f * val) + 2f)) + start);
    }

    private static float easeInOutQuad(float start, float end, float val)
    {
        val /= 0.5f;
        end -= start;
        if (val < 1f)
        {
            return ((((end / 2f) * val) * val) + start);
        }
        val--;
        return (((-end / 2f) * ((val * (val - 2f)) - 1f)) + start);
    }

    private static float easeInOutQuadOpt(float start, float diff, float ratioPassed)
    {
        ratioPassed /= 0.5f;
        if (ratioPassed < 1f)
        {
            return ((((diff / 2f) * ratioPassed) * ratioPassed) + start);
        }
        ratioPassed--;
        return (((-diff / 2f) * ((ratioPassed * (ratioPassed - 2f)) - 1f)) + start);
    }

    private static float easeInOutQuart(float start, float end, float val)
    {
        val /= 0.5f;
        end -= start;
        if (val < 1f)
        {
            return ((((((end / 2f) * val) * val) * val) * val) + start);
        }
        val -= 2f;
        return (((-end / 2f) * ((((val * val) * val) * val) - 2f)) + start);
    }

    private static float easeInOutQuint(float start, float end, float val)
    {
        val /= 0.5f;
        end -= start;
        if (val < 1f)
        {
            return (((((((end / 2f) * val) * val) * val) * val) * val) + start);
        }
        val -= 2f;
        return (((end / 2f) * (((((val * val) * val) * val) * val) + 2f)) + start);
    }

    private static float easeInOutSine(float start, float end, float val)
    {
        end -= start;
        return (((-end / 2f) * (Mathf.Cos((3.141593f * val) / 1f) - 1f)) + start);
    }

    private static float easeInQuad(float start, float end, float val)
    {
        end -= start;
        return (((end * val) * val) + start);
    }

    private static float easeInQuadOpt(float start, float diff, float ratioPassed)
    {
        return (((diff * ratioPassed) * ratioPassed) + start);
    }

    private static float easeInQuart(float start, float end, float val)
    {
        end -= start;
        return (((((end * val) * val) * val) * val) + start);
    }

    private static float easeInQuint(float start, float end, float val)
    {
        end -= start;
        return ((((((end * val) * val) * val) * val) * val) + start);
    }

    private static float easeInSine(float start, float end, float val)
    {
        end -= start;
        return (((-end * Mathf.Cos((val / 1f) * 1.570796f)) + end) + start);
    }

    private static float easeOutBack(float start, float end, float val)
    {
        float num = 1.70158f;
        end -= start;
        val = (val / 1f) - 1f;
        return ((end * (((val * val) * (((num + 1f) * val) + num)) + 1f)) + start);
    }

    private static float easeOutBounce(float start, float end, float val)
    {
        val /= 1f;
        end -= start;
        if (val < 0.3636364f)
        {
            return ((end * ((7.5625f * val) * val)) + start);
        }
        if (val < 0.7272727f)
        {
            val -= 0.5454546f;
            return ((end * (((7.5625f * val) * val) + 0.75f)) + start);
        }
        if (val < 0.90909090909090906)
        {
            val -= 0.8181818f;
            return ((end * (((7.5625f * val) * val) + 0.9375f)) + start);
        }
        val -= 0.9545454f;
        return ((end * (((7.5625f * val) * val) + 0.984375f)) + start);
    }

    private static float easeOutCirc(float start, float end, float val)
    {
        val--;
        end -= start;
        return ((end * Mathf.Sqrt(1f - (val * val))) + start);
    }

    private static float easeOutCubic(float start, float end, float val)
    {
        val--;
        end -= start;
        return ((end * (((val * val) * val) + 1f)) + start);
    }

    private static float easeOutElastic(float start, float end, float val)
    {
        end -= start;
        float num = 1f;
        float num2 = num * 0.3f;
        float num3 = 0f;
        float num4 = 0f;
        if (val == 0f)
        {
            return start;
        }
        val /= num;
        if (val == 1f)
        {
            return (start + end);
        }
        if ((num4 == 0f) || (num4 < Mathf.Abs(end)))
        {
            num4 = end;
            num3 = num2 / 4f;
        }
        else
        {
            num3 = (num2 / 6.283185f) * Mathf.Asin(end / num4);
        }
        return ((((num4 * Mathf.Pow(2f, -10f * val)) * Mathf.Sin((((val * num) - num3) * 6.283185f) / num2)) + end) + start);
    }

    private static float easeOutExpo(float start, float end, float val)
    {
        end -= start;
        return ((end * (-Mathf.Pow(2f, (-10f * val) / 1f) + 1f)) + start);
    }

    private static float easeOutQuad(float start, float end, float val)
    {
        end -= start;
        return (((-end * val) * (val - 2f)) + start);
    }

    private static float easeOutQuadOpt(float start, float diff, float ratioPassed)
    {
        return (((-diff * ratioPassed) * (ratioPassed - 2f)) + start);
    }

    private static float easeOutQuart(float start, float end, float val)
    {
        val--;
        end -= start;
        return ((-end * ((((val * val) * val) * val) - 1f)) + start);
    }

    private static float easeOutQuint(float start, float end, float val)
    {
        val--;
        end -= start;
        return ((end * (((((val * val) * val) * val) * val) + 1f)) + start);
    }

    private static float easeOutSine(float start, float end, float val)
    {
        end -= start;
        return ((end * Mathf.Sin((val / 1f) * 1.570796f)) + start);
    }

    public static void init()
    {
        init(maxTweens);
    }

    public static void init(int maxSimultaneousTweens)
    {
        if (tweens == null)
        {
            maxTweens = maxSimultaneousTweens;
            tweens = new LTDescr[maxTweens];
            tweensFinished = new int[maxTweens];
            _tweenEmpty = new GameObject();
            _tweenEmpty.name = "~LeanTween";
            _tweenEmpty.AddComponent(typeof(LeanTween));
            _tweenEmpty.isStatic = true;
            _tweenEmpty.hideFlags = HideFlags.HideAndDontSave;
            UnityEngine.Object.DontDestroyOnLoad(_tweenEmpty);
            for (int i = 0; i < maxTweens; i++)
            {
                tweens[i] = new LTDescr();
            }
        }
    }

    public static bool IsInitialised()
    {
        return (tweens != null);
    }

    public static bool isTweening(LTRect ltRect)
    {
        for (int i = 0; i <= tweenMaxSearch; i++)
        {
            if (tweens[i].toggle && (tweens[i].ltRect == ltRect))
            {
                return true;
            }
        }
        return false;
    }

    public static bool isTweening(int uniqueId)
    {
        int index = uniqueId & 0xffff;
        int num2 = uniqueId >> 0x10;
        if ((index < 0) || (index >= maxTweens))
        {
            return false;
        }
        return ((tweens[index].counter == num2) && tweens[index].toggle);
    }

    public static bool isTweening(GameObject gameObject = null)
    {
        if (gameObject == null)
        {
            for (int j = 0; j <= tweenMaxSearch; j++)
            {
                if (tweens[j].toggle)
                {
                    return true;
                }
            }
            return false;
        }
        Transform transform = gameObject.transform;
        for (int i = 0; i <= tweenMaxSearch; i++)
        {
            if (tweens[i].toggle && (tweens[i].trans == transform))
            {
                return true;
            }
        }
        return false;
    }

    private static float linear(float start, float end, float val)
    {
        return Mathf.Lerp(start, end, val);
    }

    public static object logError(string error)
    {
        if (throwErrors)
        {
            Debug.LogError(error);
        }
        else
        {
            Debug.Log(error);
        }
        return null;
    }

    public static LTDescr move(LTRect ltRect, Vector2 to, float time)
    {
        return pushNewTween(tweenEmpty, (Vector3) to, time, TweenAction.GUI_MOVE, options().setRect(ltRect));
    }

    public static LTDescr move(GameObject gameObject, Vector2 to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to.x, to.y, gameObject.transform.position.z), time, TweenAction.MOVE, options());
    }

    public static LTDescr move(GameObject gameObject, Vector3 to, float time)
    {
        return pushNewTween(gameObject, to, time, TweenAction.MOVE, options());
    }

    public static LTDescr move(GameObject gameObject, Vector3[] to, float time)
    {
        descr = options();
        if (descr.path == null)
        {
            descr.path = new LTBezierPath(to);
        }
        else
        {
            descr.path.setPoints(to);
        }
        return pushNewTween(gameObject, new Vector3(1f, 0f, 0f), time, TweenAction.MOVE_CURVED, descr);
    }

    public static LTDescr move(RectTransform rectTrans, Vector3 to, float time)
    {
        return pushNewTween(rectTrans.gameObject, to, time, TweenAction.CANVAS_MOVE, options().setRect(rectTrans));
    }

    public static LTDescr moveLocal(GameObject gameObject, Vector3 to, float time)
    {
        return pushNewTween(gameObject, to, time, TweenAction.MOVE_LOCAL, options());
    }

    public static LTDescr moveLocal(GameObject gameObject, Vector3[] to, float time)
    {
        descr = options();
        if (descr.path == null)
        {
            descr.path = new LTBezierPath(to);
        }
        else
        {
            descr.path.setPoints(to);
        }
        return pushNewTween(gameObject, new Vector3(1f, 0f, 0f), time, TweenAction.MOVE_CURVED_LOCAL, descr);
    }

    public static LTDescr moveLocalX(GameObject gameObject, float to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_LOCAL_X, options());
    }

    public static LTDescr moveLocalY(GameObject gameObject, float to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_LOCAL_Y, options());
    }

    public static LTDescr moveLocalZ(GameObject gameObject, float to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_LOCAL_Z, options());
    }

    public static LTDescr moveMargin(LTRect ltRect, Vector2 to, float time)
    {
        return pushNewTween(tweenEmpty, (Vector3) to, time, TweenAction.GUI_MOVE_MARGIN, options().setRect(ltRect));
    }

    public static LTDescr moveSpline(GameObject gameObject, Vector3[] to, float time)
    {
        descr = options();
        descr.spline = new LTSpline(to);
        return pushNewTween(gameObject, new Vector3(1f, 0f, 0f), time, TweenAction.MOVE_SPLINE, descr);
    }

    public static LTDescr moveSplineLocal(GameObject gameObject, Vector3[] to, float time)
    {
        descr = options();
        descr.spline = new LTSpline(to);
        return pushNewTween(gameObject, new Vector3(1f, 0f, 0f), time, TweenAction.MOVE_SPLINE_LOCAL, descr);
    }

    public static LTDescr moveX(GameObject gameObject, float to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_X, options());
    }

    public static LTDescr moveY(GameObject gameObject, float to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_Y, options());
    }

    public static LTDescr moveZ(GameObject gameObject, float to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_Z, options());
    }

    public void OnLevelWasLoaded(int lvl)
    {
        LTGUI.reset();
    }

    public static LTDescr options()
    {
        init();
        j = 0;
        i = startSearch;
        while (j < maxTweens)
        {
            if (i >= (maxTweens - 1))
            {
                i = 0;
            }
            if (!tweens[i].toggle)
            {
                if ((i + 1) > tweenMaxSearch)
                {
                    tweenMaxSearch = i + 1;
                }
                startSearch = i + 1;
                break;
            }
            j++;
            if (j >= maxTweens)
            {
                return (logError("LeanTween - You have run out of available spaces for tweening. To avoid this error increase the number of spaces to available for tweening when you initialize the LeanTween class ex: LeanTween.init( " + (maxTweens * 2) + " );") as LTDescr);
            }
            i++;
        }
        tweens[i].reset();
        tweens[i].setId((uint) i);
        return tweens[i];
    }

    public static LTDescr options(LTDescr seed)
    {
        Debug.LogError("error this function is no longer used");
        return null;
    }

    public static void pause(int uniqueId)
    {
        int index = uniqueId & 0xffff;
        int num2 = uniqueId >> 0x10;
        if (tweens[index].counter == num2)
        {
            tweens[index].pause();
        }
    }

    public static void pause(GameObject gameObject)
    {
        Transform transform = gameObject.transform;
        for (int i = 0; i <= tweenMaxSearch; i++)
        {
            if (tweens[i].trans == transform)
            {
                tweens[i].pause();
            }
        }
    }

    [Obsolete("Use 'pause( id )' instead")]
    public static void pause(GameObject gameObject, int uniqueId)
    {
        pause(uniqueId);
    }

    public static void pauseAll()
    {
        init();
        for (int i = 0; i <= tweenMaxSearch; i++)
        {
            tweens[i].pause();
        }
    }

    public static LTDescr play(RectTransform rectTransform, Sprite[] sprites)
    {
        float num = 0.25f;
        float time = num * sprites.Length;
        return pushNewTween(rectTransform.gameObject, new Vector3(sprites.Length - 1f, 0f, 0f), time, TweenAction.CANVAS_PLAYSPRITE, options().setSprites(sprites).setRepeat(-1));
    }

    private static LTDescr pushNewTween(GameObject gameObject, Vector3 to, float time, TweenAction tweenAction, LTDescr tween)
    {
        init(maxTweens);
        if ((gameObject == null) || (tween == null))
        {
            return null;
        }
        tween.trans = gameObject.transform;
        tween.to = to;
        tween.time = time;
        tween.type = tweenAction;
        return tween;
    }

    public static bool removeListener(int eventId, Action<LTEvent> callback)
    {
        return removeListener(tweenEmpty, eventId, callback);
    }

    public static bool removeListener(GameObject caller, int eventId, Action<LTEvent> callback)
    {
        i = 0;
        while (i < eventsMaxSearch)
        {
            int index = (eventId * INIT_LISTENERS_MAX) + i;
            if ((goListeners[index] == caller) && object.Equals(eventListeners[index], callback))
            {
                eventListeners[index] = null;
                goListeners[index] = null;
                return true;
            }
            i++;
        }
        return false;
    }

    public static void removeTween(int i)
    {
        if (tweens[i].toggle)
        {
            tweens[i].toggle = false;
            if (tweens[i].destroyOnComplete)
            {
                if (tweens[i].ltRect != null)
                {
                    LTGUI.destroy(tweens[i].ltRect.id);
                }
                else if (tweens[i].trans.gameObject != _tweenEmpty)
                {
                    UnityEngine.Object.Destroy(tweens[i].trans.gameObject);
                }
            }
            startSearch = i;
            if ((i + 1) >= tweenMaxSearch)
            {
                startSearch = 0;
            }
        }
    }

    public static void reset()
    {
        tweens = null;
    }

    public static void resume(int uniqueId)
    {
        int index = uniqueId & 0xffff;
        int num2 = uniqueId >> 0x10;
        if (tweens[index].counter == num2)
        {
            tweens[index].resume();
        }
    }

    public static void resume(GameObject gameObject)
    {
        Transform transform = gameObject.transform;
        for (int i = 0; i <= tweenMaxSearch; i++)
        {
            if (tweens[i].trans == transform)
            {
                tweens[i].resume();
            }
        }
    }

    [Obsolete("Use 'resume( id )' instead")]
    public static void resume(GameObject gameObject, int uniqueId)
    {
        resume(uniqueId);
    }

    public static void resumeAll()
    {
        init();
        for (int i = 0; i <= tweenMaxSearch; i++)
        {
            tweens[i].resume();
        }
    }

    public static LTDescr rotate(LTRect ltRect, float to, float time)
    {
        return pushNewTween(tweenEmpty, new Vector3(to, 0f, 0f), time, TweenAction.GUI_ROTATE, options().setRect(ltRect));
    }

    public static LTDescr rotate(GameObject gameObject, Vector3 to, float time)
    {
        return pushNewTween(gameObject, to, time, TweenAction.ROTATE, options());
    }

    public static LTDescr rotate(RectTransform rectTrans, float to, float time)
    {
        return pushNewTween(rectTrans.gameObject, new Vector3(to, 0f, 0f), time, TweenAction.CANVAS_ROTATEAROUND, options().setRect(rectTrans).setAxis(Vector3.forward));
    }

    public static LTDescr rotateAround(GameObject gameObject, Vector3 axis, float add, float time)
    {
        return pushNewTween(gameObject, new Vector3(add, 0f, 0f), time, TweenAction.ROTATE_AROUND, options().setAxis(axis));
    }

    public static LTDescr rotateAround(RectTransform rectTrans, Vector3 axis, float to, float time)
    {
        return pushNewTween(rectTrans.gameObject, new Vector3(to, 0f, 0f), time, TweenAction.CANVAS_ROTATEAROUND, options().setRect(rectTrans).setAxis(axis));
    }

    public static LTDescr rotateAroundLocal(GameObject gameObject, Vector3 axis, float add, float time)
    {
        return pushNewTween(gameObject, new Vector3(add, 0f, 0f), time, TweenAction.ROTATE_AROUND_LOCAL, options().setAxis(axis));
    }

    public static LTDescr rotateAroundLocal(RectTransform rectTrans, Vector3 axis, float to, float time)
    {
        return pushNewTween(rectTrans.gameObject, new Vector3(to, 0f, 0f), time, TweenAction.CANVAS_ROTATEAROUND_LOCAL, options().setRect(rectTrans).setAxis(axis));
    }

    public static LTDescr rotateLocal(GameObject gameObject, Vector3 to, float time)
    {
        return pushNewTween(gameObject, to, time, TweenAction.ROTATE_LOCAL, options());
    }

    public static LTDescr rotateX(GameObject gameObject, float to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ROTATE_X, options());
    }

    public static LTDescr rotateY(GameObject gameObject, float to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ROTATE_Y, options());
    }

    public static LTDescr rotateZ(GameObject gameObject, float to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ROTATE_Z, options());
    }

    public static LTDescr scale(LTRect ltRect, Vector2 to, float time)
    {
        return pushNewTween(tweenEmpty, (Vector3) to, time, TweenAction.GUI_SCALE, options().setRect(ltRect));
    }

    public static LTDescr scale(GameObject gameObject, Vector3 to, float time)
    {
        return pushNewTween(gameObject, to, time, TweenAction.SCALE, options());
    }

    public static LTDescr scale(RectTransform rectTrans, Vector3 to, float time)
    {
        return pushNewTween(rectTrans.gameObject, to, time, TweenAction.CANVAS_SCALE, options().setRect(rectTrans));
    }

    public static LTDescr scaleX(GameObject gameObject, float to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.SCALE_X, options());
    }

    public static LTDescr scaleY(GameObject gameObject, float to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.SCALE_Y, options());
    }

    public static LTDescr scaleZ(GameObject gameObject, float to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.SCALE_Z, options());
    }

    private static float spring(float start, float end, float val)
    {
        val = Mathf.Clamp01(val);
        val = ((Mathf.Sin((val * 3.141593f) * (0.2f + (((2.5f * val) * val) * val))) * Mathf.Pow(1f - val, 2.2f)) + val) * (1f + (1.2f * (1f - val)));
        return (start + ((end - start) * val));
    }

    public static LTDescr textAlpha(RectTransform rectTransform, float to, float time)
    {
        return pushNewTween(rectTransform.gameObject, new Vector3(to, 0f, 0f), time, TweenAction.TEXT_ALPHA, options());
    }

    private static void textAlphaRecursive(Transform trans, float val)
    {
        Text component = trans.gameObject.GetComponent<Text>();
        if (component != null)
        {
            Color color = component.color;
            color.a = val;
            component.color = color;
        }
        if (trans.childCount > 0)
        {
            IEnumerator enumerator = trans.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    textAlphaRecursive(current, val);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }
    }

    public static LTDescr textColor(RectTransform rectTransform, Color to, float time)
    {
        return pushNewTween(rectTransform.gameObject, new Vector3(1f, to.a, 0f), time, TweenAction.TEXT_COLOR, options().setPoint(new Vector3(to.r, to.g, to.b)));
    }

    private static Color tweenColor(LTDescr tween, float val)
    {
        Vector3 vector = tween.point - tween.axis;
        float num = tween.to.y - tween.from.y;
        return new Color(tween.axis.x + (vector.x * val), tween.axis.y + (vector.y * val), tween.axis.z + (vector.z * val), tween.from.y + (num * val));
    }

    private static float tweenOnCurve(LTDescr tweenDescr, float ratioPassed)
    {
        return (tweenDescr.from.x + (tweenDescr.diff.x * tweenDescr.animationCurve.Evaluate(ratioPassed)));
    }

    private static Vector3 tweenOnCurveVector(LTDescr tweenDescr, float ratioPassed)
    {
        return new Vector3(tweenDescr.from.x + (tweenDescr.diff.x * tweenDescr.animationCurve.Evaluate(ratioPassed)), tweenDescr.from.y + (tweenDescr.diff.y * tweenDescr.animationCurve.Evaluate(ratioPassed)), tweenDescr.from.z + (tweenDescr.diff.z * tweenDescr.animationCurve.Evaluate(ratioPassed)));
    }

    public static void update()
    {
        if (frameRendered != Time.frameCount)
        {
            init();
            dtEstimated = Time.realtimeSinceStartup - previousRealTime;
            if (dtEstimated > 0.2f)
            {
                dtEstimated = 0.2f;
            }
            previousRealTime = Time.realtimeSinceStartup;
            dtActual = Time.deltaTime;
            maxTweenReached = 0;
            finishedCnt = 0;
            for (int i = 0; (i <= tweenMaxSearch) && (i < maxTweens); i++)
            {
                if (!tweens[i].toggle)
                {
                    continue;
                }
                maxTweenReached = i;
                tween = tweens[i];
                trans = tween.trans;
                timeTotal = tween.time;
                tweenAction = tween.type;
                dt = dtActual;
                if (tween.useEstimatedTime)
                {
                    dt = dtEstimated;
                    timeTotal = tween.time;
                }
                else if (tween.useFrames)
                {
                    dt = 1f;
                }
                else if (tween.useManualTime)
                {
                    dt = dtManual;
                }
                else if (tween.direction == 0f)
                {
                    dt = 0f;
                }
                if (trans == null)
                {
                    removeTween(i);
                    continue;
                }
                isTweenFinished = false;
                if (tween.delay <= 0f)
                {
                    if (((tween.passed + dt) > tween.time) && (tween.direction > 0f))
                    {
                        isTweenFinished = true;
                        tween.passed = tween.time;
                    }
                    else if ((tween.direction < 0f) && ((tween.passed - dt) < 0f))
                    {
                        isTweenFinished = true;
                        tween.passed = float.Epsilon;
                    }
                }
                if (!tween.hasInitiliazed && (((tween.passed == 0.0) && (tween.delay == 0.0)) || (tween.passed > 0.0)))
                {
                    tween.init();
                }
                if (tween.delay > 0f)
                {
                    goto Label_303D;
                }
                if (timeTotal <= 0f)
                {
                    ratioPassed = 0f;
                }
                else
                {
                    ratioPassed = tween.passed / timeTotal;
                }
                if (ratioPassed > 1f)
                {
                    ratioPassed = 1f;
                }
                else if (ratioPassed < 0f)
                {
                    ratioPassed = 0f;
                }
                if ((tweenAction < TweenAction.MOVE_X) || (tweenAction >= TweenAction.MOVE))
                {
                    goto Label_1984;
                }
                if (tween.animationCurve != null)
                {
                    val = tweenOnCurve(tween, ratioPassed);
                    goto Label_0AF8;
                }
                switch (tween.tweenType)
                {
                    case LeanTweenType.linear:
                        val = tween.from.x + (tween.diff.x * ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeOutQuad:
                        val = easeOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInQuad:
                        val = easeInQuadOpt(tween.from.x, tween.diff.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInOutQuad:
                        val = easeInOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInCubic:
                        val = easeInCubic(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeOutCubic:
                        val = easeOutCubic(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInOutCubic:
                        val = easeInOutCubic(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInQuart:
                        val = easeInQuart(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeOutQuart:
                        val = easeOutQuart(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInOutQuart:
                        val = easeInOutQuart(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInQuint:
                        val = easeInQuint(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeOutQuint:
                        val = easeOutQuint(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInOutQuint:
                        val = easeInOutQuint(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInSine:
                        val = easeInSine(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeOutSine:
                        val = easeOutSine(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInOutSine:
                        val = easeInOutSine(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInExpo:
                        val = easeInExpo(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeOutExpo:
                        val = easeOutExpo(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInOutExpo:
                        val = easeInOutExpo(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInCirc:
                        val = easeInCirc(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeOutCirc:
                        val = easeOutCirc(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInOutCirc:
                        val = easeInOutCirc(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInBounce:
                        val = easeInBounce(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeOutBounce:
                        val = easeOutBounce(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInOutBounce:
                        val = easeInOutBounce(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInBack:
                        val = easeInBack(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeOutBack:
                        val = easeOutBack(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInOutBack:
                        val = easeInOutElastic(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInElastic:
                        val = easeInElastic(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeOutElastic:
                        val = easeOutElastic(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeInOutElastic:
                        val = easeInOutElastic(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeSpring:
                        val = spring(tween.from.x, tween.to.x, ratioPassed);
                        goto Label_0AF8;

                    case LeanTweenType.easeShake:
                    case LeanTweenType.punch:
                        if (tween.tweenType != LeanTweenType.punch)
                        {
                            break;
                        }
                        tween.animationCurve = punch;
                        goto Label_0A22;

                    default:
                        val = tween.from.x + (tween.diff.x * ratioPassed);
                        goto Label_0AF8;
                }
                if (tween.tweenType == LeanTweenType.easeShake)
                {
                    tween.animationCurve = shake;
                }
            Label_0A22:
                tween.to.x = tween.from.x + tween.to.x;
                tween.diff.x = tween.to.x - tween.from.x;
                val = tweenOnCurve(tween, ratioPassed);
            Label_0AF8:
                if (tweenAction == TweenAction.MOVE_X)
                {
                    trans.position = new Vector3(val, trans.position.y, trans.position.z);
                }
                else if (tweenAction == TweenAction.MOVE_Y)
                {
                    trans.position = new Vector3(trans.position.x, val, trans.position.z);
                }
                else if (tweenAction == TweenAction.MOVE_Z)
                {
                    trans.position = new Vector3(trans.position.x, trans.position.y, val);
                }
                if (tweenAction == TweenAction.MOVE_LOCAL_X)
                {
                    trans.localPosition = new Vector3(val, trans.localPosition.y, trans.localPosition.z);
                }
                else if (tweenAction == TweenAction.MOVE_LOCAL_Y)
                {
                    trans.localPosition = new Vector3(trans.localPosition.x, val, trans.localPosition.z);
                }
                else if (tweenAction == TweenAction.MOVE_LOCAL_Z)
                {
                    trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, val);
                }
                else if (tweenAction == TweenAction.MOVE_CURVED)
                {
                    if (tween.path.orientToPath)
                    {
                        if (tween.path.orientToPath2d)
                        {
                            tween.path.place2d(trans, val);
                        }
                        else
                        {
                            tween.path.place(trans, val);
                        }
                    }
                    else
                    {
                        trans.position = tween.path.point(val);
                    }
                }
                else if (tweenAction == TweenAction.MOVE_CURVED_LOCAL)
                {
                    if (tween.path.orientToPath)
                    {
                        if (tween.path.orientToPath2d)
                        {
                            tween.path.placeLocal2d(trans, val);
                        }
                        else
                        {
                            tween.path.placeLocal(trans, val);
                        }
                    }
                    else
                    {
                        trans.localPosition = tween.path.point(val);
                    }
                }
                else if (tweenAction == TweenAction.MOVE_SPLINE)
                {
                    if (tween.spline.orientToPath)
                    {
                        if (tween.spline.orientToPath2d)
                        {
                            tween.spline.place2d(trans, val);
                        }
                        else
                        {
                            tween.spline.place(trans, val);
                        }
                    }
                    else
                    {
                        trans.position = tween.spline.point(val);
                    }
                }
                else if (tweenAction == TweenAction.MOVE_SPLINE_LOCAL)
                {
                    if (tween.spline.orientToPath)
                    {
                        if (tween.spline.orientToPath2d)
                        {
                            tween.spline.placeLocal2d(trans, val);
                        }
                        else
                        {
                            tween.spline.placeLocal(trans, val);
                        }
                    }
                    else
                    {
                        trans.localPosition = tween.spline.point(val);
                    }
                }
                else if (tweenAction == TweenAction.SCALE_X)
                {
                    trans.localScale = new Vector3(val, trans.localScale.y, trans.localScale.z);
                }
                else if (tweenAction == TweenAction.SCALE_Y)
                {
                    trans.localScale = new Vector3(trans.localScale.x, val, trans.localScale.z);
                }
                else if (tweenAction == TweenAction.SCALE_Z)
                {
                    trans.localScale = new Vector3(trans.localScale.x, trans.localScale.y, val);
                }
                else if (tweenAction == TweenAction.ROTATE_X)
                {
                    trans.eulerAngles = new Vector3(val, trans.eulerAngles.y, trans.eulerAngles.z);
                }
                else if (tweenAction == TweenAction.ROTATE_Y)
                {
                    trans.eulerAngles = new Vector3(trans.eulerAngles.x, val, trans.eulerAngles.z);
                }
                else if (tweenAction == TweenAction.ROTATE_Z)
                {
                    trans.eulerAngles = new Vector3(trans.eulerAngles.x, trans.eulerAngles.y, val);
                }
                else if (tweenAction == TweenAction.ROTATE_AROUND)
                {
                    Vector3 localPosition = trans.localPosition;
                    trans.RotateAround(trans.TransformPoint(tween.point), tween.axis, -val);
                    Vector3 vector2 = localPosition - trans.localPosition;
                    trans.localPosition = localPosition - vector2;
                    trans.rotation = tween.origRotation;
                    trans.RotateAround(trans.TransformPoint(tween.point), tween.axis, val);
                }
                else if (tweenAction == TweenAction.ROTATE_AROUND_LOCAL)
                {
                    Vector3 vector3 = trans.localPosition;
                    trans.RotateAround(trans.TransformPoint(tween.point), trans.TransformDirection(tween.axis), -val);
                    Vector3 vector4 = vector3 - trans.localPosition;
                    trans.localPosition = vector3 - vector4;
                    trans.localRotation = tween.origRotation;
                    trans.RotateAround(trans.TransformPoint(tween.point), trans.TransformDirection(tween.axis), val);
                }
                else if (tweenAction == TweenAction.ALPHA)
                {
                    SpriteRenderer component = trans.gameObject.GetComponent<SpriteRenderer>();
                    if (component != null)
                    {
                        component.color = new Color(component.color.r, component.color.g, component.color.b, val);
                    }
                    else
                    {
                        if (trans.gameObject.GetComponent<Renderer>() != null)
                        {
                            foreach (Material material in trans.gameObject.GetComponent<Renderer>().materials)
                            {
                                if (material.HasProperty("_Color"))
                                {
                                    material.color = new Color(material.color.r, material.color.g, material.color.b, val);
                                }
                                else if (material.HasProperty("_TintColor"))
                                {
                                    Color color = material.GetColor("_TintColor");
                                    material.SetColor("_TintColor", new Color(color.r, color.g, color.b, val));
                                }
                            }
                        }
                        if (trans.childCount > 0)
                        {
                            IEnumerator enumerator = trans.GetEnumerator();
                            try
                            {
                                while (enumerator.MoveNext())
                                {
                                    Transform current = (Transform) enumerator.Current;
                                    if (current.gameObject.GetComponent<Renderer>() != null)
                                    {
                                        foreach (Material material2 in current.gameObject.GetComponent<Renderer>().materials)
                                        {
                                            material2.color = new Color(material2.color.r, material2.color.g, material2.color.b, val);
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                IDisposable disposable = enumerator as IDisposable;
                                if (disposable == null)
                                {
                                }
                                disposable.Dispose();
                            }
                        }
                    }
                }
                else if (tweenAction == TweenAction.ALPHA_VERTEX)
                {
                    Mesh mesh = trans.GetComponent<MeshFilter>().mesh;
                    Vector3[] vertices = mesh.vertices;
                    Color32[] colorArray = new Color32[vertices.Length];
                    Color32 color2 = mesh.colors32[0];
                    color2 = new Color((float) color2.r, (float) color2.g, (float) color2.b, val);
                    for (int k = 0; k < vertices.Length; k++)
                    {
                        colorArray[k] = color2;
                    }
                    mesh.colors32 = colorArray;
                }
                else if ((tweenAction == TweenAction.COLOR) || (tweenAction == TweenAction.CALLBACK_COLOR))
                {
                    Color color3 = tweenColor(tween, val);
                    if (tweenAction == TweenAction.COLOR)
                    {
                        if (trans.gameObject.GetComponent<Renderer>() != null)
                        {
                            foreach (Material material3 in trans.gameObject.GetComponent<Renderer>().materials)
                            {
                                material3.color = color3;
                            }
                        }
                        if (trans.childCount > 0)
                        {
                            IEnumerator enumerator2 = trans.GetEnumerator();
                            try
                            {
                                while (enumerator2.MoveNext())
                                {
                                    Transform transform2 = (Transform) enumerator2.Current;
                                    if (transform2.gameObject.GetComponent<Renderer>() != null)
                                    {
                                        foreach (Material material4 in transform2.gameObject.GetComponent<Renderer>().materials)
                                        {
                                            material4.color = color3;
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                IDisposable disposable2 = enumerator2 as IDisposable;
                                if (disposable2 == null)
                                {
                                }
                                disposable2.Dispose();
                            }
                        }
                    }
                    if (tween.onUpdateColor != null)
                    {
                        tween.onUpdateColor(color3);
                    }
                }
                else if (tweenAction == TweenAction.CANVAS_ALPHA)
                {
                    Color color4 = tween.uiImage.color;
                    color4.a = val;
                    tween.uiImage.color = color4;
                }
                else if (tweenAction == TweenAction.CANVAS_COLOR)
                {
                    Color color5 = tweenColor(tween, val);
                    tween.uiImage.color = color5;
                    if (tween.onUpdateColor != null)
                    {
                        tween.onUpdateColor(color5);
                    }
                }
                else if (tweenAction == TweenAction.TEXT_ALPHA)
                {
                    textAlphaRecursive(trans, val);
                }
                else if (tweenAction == TweenAction.TEXT_COLOR)
                {
                    Color color6 = tweenColor(tween, val);
                    tween.uiText.color = color6;
                    if (tween.onUpdateColor != null)
                    {
                        tween.onUpdateColor(color6);
                    }
                    if (trans.childCount > 0)
                    {
                        IEnumerator enumerator3 = trans.GetEnumerator();
                        try
                        {
                            while (enumerator3.MoveNext())
                            {
                                Transform transform3 = (Transform) enumerator3.Current;
                                Text text = transform3.gameObject.GetComponent<Text>();
                                if (text != null)
                                {
                                    text.color = color6;
                                }
                            }
                        }
                        finally
                        {
                            IDisposable disposable3 = enumerator3 as IDisposable;
                            if (disposable3 == null)
                            {
                            }
                            disposable3.Dispose();
                        }
                    }
                }
                else if (tweenAction == TweenAction.CANVAS_ROTATEAROUND)
                {
                    RectTransform rectTransform = tween.rectTransform;
                    Vector3 vector5 = rectTransform.localPosition;
                    rectTransform.RotateAround(rectTransform.TransformPoint(tween.point), tween.axis, -val);
                    Vector3 vector6 = vector5 - rectTransform.localPosition;
                    rectTransform.localPosition = vector5 - vector6;
                    rectTransform.rotation = tween.origRotation;
                    rectTransform.RotateAround(rectTransform.TransformPoint(tween.point), tween.axis, val);
                }
                else if (tweenAction == TweenAction.CANVAS_ROTATEAROUND_LOCAL)
                {
                    RectTransform transform5 = tween.rectTransform;
                    Vector3 vector7 = transform5.localPosition;
                    transform5.RotateAround(transform5.TransformPoint(tween.point), transform5.TransformDirection(tween.axis), -val);
                    Vector3 vector8 = vector7 - transform5.localPosition;
                    transform5.localPosition = vector7 - vector8;
                    transform5.rotation = tween.origRotation;
                    transform5.RotateAround(transform5.TransformPoint(tween.point), transform5.TransformDirection(tween.axis), val);
                }
                else if (tweenAction == TweenAction.CANVAS_PLAYSPRITE)
                {
                    int index = (int) Mathf.Round(val);
                    tween.uiImage.sprite = tween.sprites[index];
                }
                goto Label_2F43;
            Label_1984:
                if (tweenAction < TweenAction.MOVE)
                {
                    goto Label_2F43;
                }
                if (tween.animationCurve != null)
                {
                    newVect = tweenOnCurveVector(tween, ratioPassed);
                }
                else if (tween.tweenType == LeanTweenType.linear)
                {
                    newVect = new Vector3(tween.from.x + (tween.diff.x * ratioPassed), tween.from.y + (tween.diff.y * ratioPassed), tween.from.z + (tween.diff.z * ratioPassed));
                }
                else
                {
                    if (tween.tweenType < LeanTweenType.linear)
                    {
                        goto Label_2C86;
                    }
                    switch (tween.tweenType)
                    {
                        case LeanTweenType.easeOutQuad:
                        {
                            float x = easeOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed);
                            float y = easeOutQuadOpt(tween.from.y, tween.diff.y, ratioPassed);
                            newVect = new Vector3(x, y, easeOutQuadOpt(tween.from.z, tween.diff.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInQuad:
                        {
                            float introduced86 = easeInQuadOpt(tween.from.x, tween.diff.x, ratioPassed);
                            float introduced87 = easeInQuadOpt(tween.from.y, tween.diff.y, ratioPassed);
                            newVect = new Vector3(introduced86, introduced87, easeInQuadOpt(tween.from.z, tween.diff.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInOutQuad:
                        {
                            float introduced88 = easeInOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed);
                            float introduced89 = easeInOutQuadOpt(tween.from.y, tween.diff.y, ratioPassed);
                            newVect = new Vector3(introduced88, introduced89, easeInOutQuadOpt(tween.from.z, tween.diff.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInCubic:
                        {
                            float introduced90 = easeInCubic(tween.from.x, tween.to.x, ratioPassed);
                            float introduced91 = easeInCubic(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced90, introduced91, easeInCubic(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeOutCubic:
                        {
                            float introduced92 = easeOutCubic(tween.from.x, tween.to.x, ratioPassed);
                            float introduced93 = easeOutCubic(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced92, introduced93, easeOutCubic(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInOutCubic:
                        {
                            float introduced94 = easeInOutCubic(tween.from.x, tween.to.x, ratioPassed);
                            float introduced95 = easeInOutCubic(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced94, introduced95, easeInOutCubic(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInQuart:
                        {
                            float introduced96 = easeInQuart(tween.from.x, tween.to.x, ratioPassed);
                            float introduced97 = easeInQuart(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced96, introduced97, easeInQuart(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeOutQuart:
                        {
                            float introduced98 = easeOutQuart(tween.from.x, tween.to.x, ratioPassed);
                            float introduced99 = easeOutQuart(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced98, introduced99, easeOutQuart(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInOutQuart:
                        {
                            float introduced100 = easeInOutQuart(tween.from.x, tween.to.x, ratioPassed);
                            float introduced101 = easeInOutQuart(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced100, introduced101, easeInOutQuart(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInQuint:
                        {
                            float introduced102 = easeInQuint(tween.from.x, tween.to.x, ratioPassed);
                            float introduced103 = easeInQuint(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced102, introduced103, easeInQuint(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeOutQuint:
                        {
                            float introduced104 = easeOutQuint(tween.from.x, tween.to.x, ratioPassed);
                            float introduced105 = easeOutQuint(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced104, introduced105, easeOutQuint(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInOutQuint:
                        {
                            float introduced106 = easeInOutQuint(tween.from.x, tween.to.x, ratioPassed);
                            float introduced107 = easeInOutQuint(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced106, introduced107, easeInOutQuint(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInSine:
                        {
                            float introduced108 = easeInSine(tween.from.x, tween.to.x, ratioPassed);
                            float introduced109 = easeInSine(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced108, introduced109, easeInSine(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeOutSine:
                        {
                            float introduced110 = easeOutSine(tween.from.x, tween.to.x, ratioPassed);
                            float introduced111 = easeOutSine(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced110, introduced111, easeOutSine(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInOutSine:
                        {
                            float introduced112 = easeInOutSine(tween.from.x, tween.to.x, ratioPassed);
                            float introduced113 = easeInOutSine(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced112, introduced113, easeInOutSine(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInExpo:
                        {
                            float introduced114 = easeInExpo(tween.from.x, tween.to.x, ratioPassed);
                            float introduced115 = easeInExpo(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced114, introduced115, easeInExpo(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeOutExpo:
                        {
                            float introduced116 = easeOutExpo(tween.from.x, tween.to.x, ratioPassed);
                            float introduced117 = easeOutExpo(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced116, introduced117, easeOutExpo(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInOutExpo:
                        {
                            float introduced118 = easeInOutExpo(tween.from.x, tween.to.x, ratioPassed);
                            float introduced119 = easeInOutExpo(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced118, introduced119, easeInOutExpo(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInCirc:
                        {
                            float introduced120 = easeInCirc(tween.from.x, tween.to.x, ratioPassed);
                            float introduced121 = easeInCirc(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced120, introduced121, easeInCirc(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeOutCirc:
                        {
                            float introduced122 = easeOutCirc(tween.from.x, tween.to.x, ratioPassed);
                            float introduced123 = easeOutCirc(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced122, introduced123, easeOutCirc(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInOutCirc:
                        {
                            float introduced124 = easeInOutCirc(tween.from.x, tween.to.x, ratioPassed);
                            float introduced125 = easeInOutCirc(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced124, introduced125, easeInOutCirc(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInBounce:
                        {
                            float introduced126 = easeInBounce(tween.from.x, tween.to.x, ratioPassed);
                            float introduced127 = easeInBounce(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced126, introduced127, easeInBounce(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeOutBounce:
                        {
                            float introduced128 = easeOutBounce(tween.from.x, tween.to.x, ratioPassed);
                            float introduced129 = easeOutBounce(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced128, introduced129, easeOutBounce(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInOutBounce:
                        {
                            float introduced130 = easeInOutBounce(tween.from.x, tween.to.x, ratioPassed);
                            float introduced131 = easeInOutBounce(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced130, introduced131, easeInOutBounce(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInBack:
                        {
                            float introduced132 = easeInBack(tween.from.x, tween.to.x, ratioPassed);
                            float introduced133 = easeInBack(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced132, introduced133, easeInBack(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeOutBack:
                        {
                            float introduced134 = easeOutBack(tween.from.x, tween.to.x, ratioPassed);
                            float introduced135 = easeOutBack(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced134, introduced135, easeOutBack(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInOutBack:
                        {
                            float introduced136 = easeInOutBack(tween.from.x, tween.to.x, ratioPassed);
                            float introduced137 = easeInOutBack(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced136, introduced137, easeInOutBack(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInElastic:
                        {
                            float introduced138 = easeInElastic(tween.from.x, tween.to.x, ratioPassed);
                            float introduced139 = easeInElastic(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced138, introduced139, easeInElastic(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeOutElastic:
                        {
                            float introduced140 = easeOutElastic(tween.from.x, tween.to.x, ratioPassed);
                            float introduced141 = easeOutElastic(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced140, introduced141, easeOutElastic(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeInOutElastic:
                        {
                            float introduced142 = easeInOutElastic(tween.from.x, tween.to.x, ratioPassed);
                            float introduced143 = easeInOutElastic(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced142, introduced143, easeInOutElastic(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeSpring:
                        {
                            float introduced146 = spring(tween.from.x, tween.to.x, ratioPassed);
                            float introduced147 = spring(tween.from.y, tween.to.y, ratioPassed);
                            newVect = new Vector3(introduced146, introduced147, spring(tween.from.z, tween.to.z, ratioPassed));
                            break;
                        }
                        case LeanTweenType.easeShake:
                        case LeanTweenType.punch:
                            if (tween.tweenType != LeanTweenType.punch)
                            {
                                goto Label_2AEB;
                            }
                            tween.animationCurve = punch;
                            goto Label_2B0B;
                    }
                }
                goto Label_2CFF;
            Label_2AEB:
                if (tween.tweenType == LeanTweenType.easeShake)
                {
                    tween.animationCurve = shake;
                }
            Label_2B0B:
                tween.to = tween.from + tween.to;
                tween.diff = tween.to - tween.from;
                if ((tweenAction == TweenAction.ROTATE) || (tweenAction == TweenAction.ROTATE_LOCAL))
                {
                    float introduced144 = closestRot(tween.from.x, tween.to.x);
                    float introduced145 = closestRot(tween.from.y, tween.to.y);
                    tween.to = new Vector3(introduced144, introduced145, closestRot(tween.from.z, tween.to.z));
                }
                newVect = tweenOnCurveVector(tween, ratioPassed);
                goto Label_2CFF;
            Label_2C86:
                newVect = new Vector3(tween.from.x + (tween.diff.x * ratioPassed), tween.from.y + (tween.diff.y * ratioPassed), tween.from.z + (tween.diff.z * ratioPassed));
            Label_2CFF:
                if (tweenAction == TweenAction.MOVE)
                {
                    trans.position = newVect;
                }
                else if (tweenAction == TweenAction.MOVE_LOCAL)
                {
                    trans.localPosition = newVect;
                }
                else if (tweenAction == TweenAction.ROTATE)
                {
                    trans.eulerAngles = newVect;
                }
                else if (tweenAction == TweenAction.ROTATE_LOCAL)
                {
                    trans.localEulerAngles = newVect;
                }
                else if (tweenAction == TweenAction.SCALE)
                {
                    trans.localScale = newVect;
                }
                else if (tweenAction == TweenAction.GUI_MOVE)
                {
                    tween.ltRect.rect = new Rect(newVect.x, newVect.y, tween.ltRect.rect.width, tween.ltRect.rect.height);
                }
                else if (tweenAction == TweenAction.GUI_MOVE_MARGIN)
                {
                    tween.ltRect.margin = new Vector2(newVect.x, newVect.y);
                }
                else if (tweenAction == TweenAction.GUI_SCALE)
                {
                    tween.ltRect.rect = new Rect(tween.ltRect.rect.x, tween.ltRect.rect.y, newVect.x, newVect.y);
                }
                else if (tweenAction == TweenAction.GUI_ALPHA)
                {
                    tween.ltRect.alpha = newVect.x;
                }
                else if (tweenAction == TweenAction.GUI_ROTATE)
                {
                    tween.ltRect.rotation = newVect.x;
                }
                else if (tweenAction == TweenAction.CANVAS_MOVE)
                {
                    tween.rectTransform.anchoredPosition3D = newVect;
                }
                else if (tweenAction == TweenAction.CANVAS_SCALE)
                {
                    tween.rectTransform.localScale = newVect;
                }
            Label_2F43:
                if (tween.hasUpdateCallback)
                {
                    if (tween.onUpdateFloat != null)
                    {
                        tween.onUpdateFloat(val);
                    }
                    else if (tween.onUpdateFloatObject != null)
                    {
                        tween.onUpdateFloatObject(val, tween.onUpdateParam);
                    }
                    else if (tween.onUpdateVector3Object != null)
                    {
                        tween.onUpdateVector3Object(newVect, tween.onUpdateParam);
                    }
                    else if (tween.onUpdateVector3 != null)
                    {
                        tween.onUpdateVector3(newVect);
                    }
                    else if (tween.onUpdateVector2 != null)
                    {
                        tween.onUpdateVector2(new Vector2(newVect.x, newVect.y));
                    }
                }
            Label_303D:
                if (isTweenFinished)
                {
                    if ((tween.loopType == LeanTweenType.once) || (tween.loopCount == 1))
                    {
                        tweensFinished[finishedCnt] = i;
                        finishedCnt++;
                        if (tweenAction == TweenAction.GUI_ROTATE)
                        {
                            tween.ltRect.rotateFinished = true;
                        }
                        if (tweenAction == TweenAction.DELAYED_SOUND)
                        {
                            AudioSource.PlayClipAtPoint((AudioClip) tween.onCompleteParam, tween.to, tween.from.x);
                        }
                    }
                    else
                    {
                        if (((tween.loopCount < 0) && (tween.type == TweenAction.CALLBACK)) || tween.onCompleteOnRepeat)
                        {
                            if (tweenAction == TweenAction.DELAYED_SOUND)
                            {
                                AudioSource.PlayClipAtPoint((AudioClip) tween.onCompleteParam, tween.to, tween.from.x);
                            }
                            if (tween.onComplete != null)
                            {
                                tween.onComplete();
                            }
                            else if (tween.onCompleteObject != null)
                            {
                                tween.onCompleteObject(tween.onCompleteParam);
                            }
                        }
                        if (tween.loopCount >= 1)
                        {
                            tween.loopCount--;
                        }
                        if (tween.loopType == LeanTweenType.pingPong)
                        {
                            tween.direction = 0f - tween.direction;
                        }
                        else
                        {
                            tween.passed = float.Epsilon;
                        }
                    }
                }
                else if (tween.delay <= 0f)
                {
                    tween.passed += dt * tween.direction;
                }
                else
                {
                    tween.delay -= dt;
                    if (tween.delay < 0f)
                    {
                        tween.passed = 0f;
                        tween.delay = 0f;
                    }
                }
            }
            tweenMaxSearch = maxTweenReached;
            frameRendered = Time.frameCount;
            for (int j = 0; j < finishedCnt; j++)
            {
                LeanTween.j = tweensFinished[j];
                tween = tweens[LeanTween.j];
                if (tween.onComplete != null)
                {
                    removeTween(LeanTween.j);
                    tween.onComplete();
                }
                else if (tween.onCompleteObject != null)
                {
                    removeTween(LeanTween.j);
                    tween.onCompleteObject(tween.onCompleteParam);
                }
                else
                {
                    removeTween(LeanTween.j);
                }
            }
        }
    }

    public void Update()
    {
        update();
    }

    public static LTDescr value(GameObject gameObject, float from, float to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.CALLBACK, options().setFrom(new Vector3(from, 0f, 0f)));
    }

    public static LTDescr value(GameObject gameObject, Color from, Color to, float time)
    {
        return pushNewTween(gameObject, new Vector3(1f, to.a, 0f), time, TweenAction.CALLBACK_COLOR, options().setPoint(new Vector3(to.r, to.g, to.b)).setFromColor(from).setHasInitialized(false));
    }

    public static LTDescr value(GameObject gameObject, Vector2 from, Vector2 to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to.x, to.y, 0f), time, TweenAction.VALUE3, options().setTo(new Vector3(to.x, to.y, 0f)).setFrom(new Vector3(from.x, from.y, 0f)));
    }

    public static LTDescr value(GameObject gameObject, Vector3 from, Vector3 to, float time)
    {
        return pushNewTween(gameObject, to, time, TweenAction.VALUE3, options().setFrom(from));
    }

    public static LTDescr value(GameObject gameObject, Action<float> callOnUpdate, float from, float to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.CALLBACK, options().setTo(new Vector3(to, 0f, 0f)).setFrom(new Vector3(from, 0f, 0f)).setOnUpdate(callOnUpdate));
    }

    public static LTDescr value(GameObject gameObject, Action<Color> callOnUpdate, Color from, Color to, float time)
    {
        return pushNewTween(gameObject, new Vector3(1f, to.a, 0f), time, TweenAction.CALLBACK_COLOR, options().setPoint(new Vector3(to.r, to.g, to.b)).setAxis(new Vector3(from.r, from.g, from.b)).setFrom(new Vector3(0f, from.a, 0f)).setHasInitialized(false).setOnUpdateColor(callOnUpdate));
    }

    public static LTDescr value(GameObject gameObject, Action<Vector2> callOnUpdate, Vector2 from, Vector2 to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to.x, to.y, 0f), time, TweenAction.VALUE3, options().setTo(new Vector3(to.x, to.y, 0f)).setFrom(new Vector3(from.x, from.y, 0f)).setOnUpdateVector2(callOnUpdate));
    }

    public static LTDescr value(GameObject gameObject, Action<Vector3> callOnUpdate, Vector3 from, Vector3 to, float time)
    {
        return pushNewTween(gameObject, to, time, TweenAction.VALUE3, options().setTo(to).setFrom(from).setOnUpdateVector3(callOnUpdate));
    }

    public static LTDescr value(GameObject gameObject, Action<float, object> callOnUpdate, float from, float to, float time)
    {
        return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.CALLBACK, options().setTo(new Vector3(to, 0f, 0f)).setFrom(new Vector3(from, 0f, 0f)).setOnUpdateObject(callOnUpdate));
    }

    public static int maxSearch
    {
        get
        {
            return tweenMaxSearch;
        }
    }

    public static GameObject tweenEmpty
    {
        get
        {
            init(maxTweens);
            return _tweenEmpty;
        }
    }
}

