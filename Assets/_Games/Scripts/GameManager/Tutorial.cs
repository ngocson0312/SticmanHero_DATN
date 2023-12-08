using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace SuperFight
{
    public class Tutorial : Singleton<Tutorial>
    {
        public static int TutorialStep
        {
            get { return PlayerPrefs.GetInt("tutorial_jxh", 0); }
            set { PlayerPrefs.SetInt("tutorial_jxh", value); }
        }
        [SerializeField] RectTransform hand;
        [SerializeField] Image imgFocus;
        [SerializeField] GameObject holder;
        public void TutorialClick(Button button, float delay, UnityAction callback)
        {
            StartCoroutine(IETutorial());
            IEnumerator IETutorial()
            {
                button.interactable = false;
                Canvas overrider = button.gameObject.AddComponent<Canvas>();
                button.gameObject.AddComponent<GraphicRaycaster>();
                RectTransform rect = button.GetComponent<RectTransform>();
                imgFocus.pixelsPerUnitMultiplier = 1;
                imgFocus.enabled = false;
                hand.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.1f);
                holder.SetActive(true);
                yield return new WaitForSeconds(delay);
                hand.anchorMax = rect.anchorMax;
                hand.anchorMin = rect.anchorMin;
                hand.position = rect.position;
                imgFocus.enabled = true;
                hand.gameObject.SetActive(true);
                imgFocus.rectTransform.anchorMax = rect.anchorMax;
                imgFocus.rectTransform.anchorMin = rect.anchorMin;
                imgFocus.rectTransform.position = rect.position;
                overrider.overrideSorting = true;
                overrider.sortingOrder = 1;
                float timer = 0.5f;
                while (timer > 0)
                {
                    timer -= Time.deltaTime;
                    imgFocus.pixelsPerUnitMultiplier = timer / 0.5f;
                    imgFocus.SetAllDirty();
                    yield return null;
                }
                button.interactable = true;
                button.onClick.AddListener(OnClickTutorial);
            }
            void OnClickTutorial()
            {
                Canvas cv = button.GetComponent<Canvas>();
                GraphicRaycaster gr = button.GetComponent<GraphicRaycaster>();
                Destroy(gr);
                Destroy(cv);
                holder.SetActive(false);
                callback?.Invoke();
                button.onClick.RemoveListener(OnClickTutorial);
            }
        }
    }
}
