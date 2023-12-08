using UnityEngine;
using UnityEngine.UI;
namespace SuperFight
{
    using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(UIManager))]
    public class UIManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Get References"))
            {
                UIManager u = target as UIManager;
                u.screens = FindObjectsOfType<ScreenUI>(true);
                u.topUI = u.GetComponentInChildren<TopUI>(true);
                u.popups = FindObjectsOfType<PopupUI>(true);
                EditorUtility.SetDirty(u);
            }
        }
    }
#endif
    public class UIManager : Singleton<UIManager>
    {
        public ScreenUI[] screens;
        public PopupUI[] popups;
        public TopUI topUI;
        public TransitionUI transition;
        public RectTransform popupHolder;
        private GameManager manager;
        public CanvasScaler[] canvasScaler;
        public Camera UICamera;
        public void Initialize(GameManager manager)
        {
            this.manager = manager;
            for (int i = 0; i < canvasScaler.Length; i++)
            {
                canvasScaler[i].matchWidthOrHeight = 1f;
                if (mygame.sdk.SdkUtil.isiPad())
                {
                    canvasScaler[i].matchWidthOrHeight = 0.5f;
                }
            }
            for (int i = 0; i < screens.Length; i++)
            {
                screens[i].Initialize(this);
            }
            for (int i = 0; i < popups.Length; i++)
            {
                popups[i].Initialize(this);
            }
            topUI.Initialize();
        }
        public void DeactiveAllScreen(bool showTopUI)
        {
            topUI.SetActive(showTopUI);
            for (int i = 0; i < screens.Length; i++)
            {
                screens[i].Deactive();
            }
        }
        public T ActiveScreen<T>(bool showTopUI = true)
        {
            topUI.SetActive(showTopUI);
            T screen = default;
            for (int i = 0; i < screens.Length; i++)
            {
                if (screens[i] is T)
                {
                    screens[i].Active();
                    screen = screens[i].GetComponent<T>();
                }
                else
                {
                    screens[i].Deactive();
                }
            }
            return screen;
        }

        public T GetScreen<T>()
        {
            for (int i = 0; i < screens.Length; i++)
            {
                if (screens[i] is T)
                {
                    return screens[i].GetComponent<T>();
                }
            }
            return default;
        }

        public T ShowPopup<T>(System.Action onClose)
        {
            T popup = default;
            for (int i = 0; i < popups.Length; i++)
            {
                if (popups[i] is T)
                {
                    popups[i].Show(onClose);
                    popup = popups[i].GetComponent<T>();
                }
            }
            return popup;
        }

        public T GetPopup<T>()
        {
            T popup = default;
            for (int i = 0; i < popups.Length; i++)
            {
                if (popups[i] is T)
                {
                    popup = popups[i].GetComponent<T>();
                }
            }
            return popup;
        }
    }
}
public static class CanvasPositioningExtensions
{
    public static Vector3 WorldToCanvasPosition(this Canvas canvas, Vector3 worldPosition, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        var viewportPosition = camera.WorldToViewportPoint(worldPosition);
        return canvas.ViewportToCanvasPosition(viewportPosition);
    }

    public static Vector3 ScreenToCanvasPosition(this Canvas canvas, Vector3 screenPosition)
    {
        var viewportPosition = new Vector3(screenPosition.x / Screen.width, screenPosition.y / Screen.height, 0);
        return canvas.ViewportToCanvasPosition(viewportPosition);
    }

    public static Vector3 ViewportToCanvasPosition(this Canvas canvas, Vector3 viewportPosition)
    {
        var centerBasedViewPortPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0);
        var canvasRect = canvas.GetComponent<RectTransform>();
        var scale = canvasRect.sizeDelta;
        return Vector3.Scale(centerBasedViewPortPosition, scale);
    }
}
