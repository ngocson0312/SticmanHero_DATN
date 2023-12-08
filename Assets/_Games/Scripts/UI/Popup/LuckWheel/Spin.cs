using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace SuperFight
{
    public class Spin : MonoBehaviour
    {
        private LuckWheelPopup spinPopup;
        public float spinDuration;
        public bool _isSpinning;
        public List<GiftBox> box;
        float randomRotateAngle;
        float pieceAngle;
        float pieceHalfAngle;
        public bool cheat;
        public int getSkin1;
        public int getSkin2;
        public Transform RotateAnchor;
        [SerializeField] ItemUtilitiesSO itemUtilities;
        [SerializeField] List<Sprite> iconBg;
        public void Initialize(LuckWheelPopup spinPopup)
        {
            this.spinPopup = spinPopup;
            pieceAngle = 360 / 8;
            pieceHalfAngle = pieceAngle / 2f;
            randomRotateAngle = 600f * spinDuration;
            SetIdGiftBox();
        }

        void SetIdGiftBox()
        {
            for (int i = 0; i < box.Count; i++)
            {
                box[i].DisplayUI();
                box[i].id = i;
            }
        }

        public void OnSpin()
        {
            if (!_isSpinning)
            {
                AudioManager.Instance.PlayOneShot("eff_spinning", 1f);
                _isSpinning = true;
                Vector3 AngleRotate = Vector3.back * Random.Range(randomRotateAngle, randomRotateAngle + 360f);
                if (cheat)//
                {
                    var _angle = AngleRotate.z % 360;

                    //Can get skin
                    if ((_angle > 0 && _angle < 22.5 || _angle < 360 && _angle > 337.5) || (_angle < 0 && _angle > -22.5 || _angle > -360 && _angle < -337.5))
                    {
                        //YellowBox
                        AngleRotate += new Vector3(0, 0, -45);
                    }
                    else if (_angle > 157.5 && _angle < 202.5 || _angle < -157.5 && _angle > -202.5)
                    {
                        //VioletBox
                        AngleRotate += new Vector3(0, 0, -45);
                    }
                }
                for (int i = 0; i < box.Count; i++)
                {
                    box[i].transform.DOLocalRotate(-AngleRotate, spinDuration, RotateMode.Fast).SetEase(Ease.OutFlash);
                }
                RotateAnchor.DORotate(AngleRotate, spinDuration, RotateMode.Fast).SetEase(Ease.OutQuart)
                 .OnComplete(() =>
                 {
                     float angle = RotateAnchor.eulerAngles.z + pieceHalfAngle;
                     int index = ((int)((angle * 8f) / 360f)) % 8;
                     _isSpinning = false;
                     spinPopup.idBox = index;
                     //SoundManager.Instance.stopSoundEff();
                     box[index].ClaimGift();
                     SetItemReward(box[index]);
                     spinPopup.spinCount++;
                 });
            }
        }

        public void SetItemReward(GiftBox giftBox)
        {
            if (giftBox.type == TypeGift.Coin)
            {
                spinPopup.rewardSpin.SetInfo(iconBg[0], giftBox.imgGift.sprite, $"{giftBox.amount}");
            } else if (giftBox.type == TypeGift.Diamond)
            {
                spinPopup.rewardSpin.SetInfo(iconBg[1], giftBox.imgGift.sprite, $"{giftBox.amount}");
            } else
            {
                spinPopup.rewardSpin.SetInfo(itemUtilities.GetGradeBackGround(giftBox.itemData.grade), giftBox.imgGift.sprite, $"{giftBox.txtGift.text}");
            }
            spinPopup.rewardSpin.Show();
        }
    }

}
