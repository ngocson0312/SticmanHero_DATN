using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using mygame.sdk;

namespace SuperFight
{
    public class InappButton : MonoBehaviour
    {
        [SerializeField] private List<Sprite> listIcon;
        [SerializeField] private Image iconReceive;
        [SerializeField] private Text txtReceive;
        [SerializeField] private Image iconInapp;
        [SerializeField] protected Text price;
        [SerializeField] protected List<GameObject> listIconButton;
        [SerializeField] private Button button;
        [SerializeField] protected int coinReceive;
        [SerializeField] private bool isFree;
        public TypeInapp typeInapp;
        public void Initialize(int idx)
        {
            button.onClick.AddListener(() => ClickBuy(idx));
            SetActiveIcon();
            SetInfo(idx);
        }

        public virtual void ClickBuy(int idx)
        {
            if (!isFree)
            {
                if (typeInapp == TypeInapp.DIAMOND)
                {
                    InappHelper.Instance.BuyPackage($"diamond{(idx + 1)}", "", (State) =>
                    {
                        if (State.status == 1)
                        {
                            int rcv = InappHelper.Instance.getMoneyRcv($"diamond{(idx + 1)}", "diamond");
                            DataManager.Instance.AddDiamond(rcv, 0, "BuyDiamond");
                        }
                    });
                }
                else
                {
                    if (DataManager.Diamond >= coinReceive / 100)
                    {
                        DataManager.Instance.AddDiamond(-coinReceive / 100, 0, "BuyGold");
                        DataManager.Instance.AddCoin(coinReceive, 0, "BuyGold");
                    }
                }
            }
        }

        public virtual void SetInfo(int idx)
        {
            if (typeInapp == TypeInapp.DIAMOND)
            {
                button.image.sprite = listIcon[4];
                iconReceive.sprite = listIcon[0];
                iconInapp.sprite = listIcon[2];
                listIconButton[1].SetActive(true);
                if (!isFree)
                {
                   // price.text = InappHelper.Instance.getPrice($"diamond{(idx + 1)}");
                  //  txtReceive.text = $"{InappHelper.Instance.getMoneyRcv($"diamond{(idx + 1)}", "diamond")}";
                }
                else
                {
                    SetActiveIcon();
                    price.gameObject.SetActive(true);
                    price.text = "Free";
                  //  txtReceive.text = $"{coinReceive}";
                }

            }
            else if (typeInapp == TypeInapp.GOLD)
            {
                button.image.sprite = listIcon[5];
                iconReceive.sprite = listIcon[1];
                iconInapp.sprite = listIcon[3];
                listIconButton[0].SetActive(true);
                if (!isFree)
                {
                   // price.text = $"{coinReceive / 100}";
                  //  txtReceive.text = $"{coinReceive}";
                }
                else
                {
                    SetActiveIcon();
                    price.gameObject.SetActive(true);
                    price.text = "Free";
                  //  txtReceive.text = $"{coinReceive}";
                }

            }
        }

        public void SetActiveIcon()
        {
            for (int i = 0; i < listIconButton.Count; i++)
            {
                listIconButton[i].SetActive(false);
            }
        }
    }

    public enum TypeInapp
    {
        GOLD, DIAMOND
    }
}
