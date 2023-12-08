using UnityEngine;
using DG.Tweening;

namespace SuperFight
{
    public class CameraController : Singleton<CameraController>
    {
        public MobilePostProcessing postProcessing;
        private Vector2 sizeCamera;
        public Vector3 offset;
        public Camera cameraMain;
        private Vector3 velocity;
        private Transform targetFollow;
        public bool stopFollowing;
        public bool unlock;
        public bool hasBound = false;
        public float maxX, maxY, minX, minY;
        public const float CAMERA_ORTHOSIZE = 5.5f;
        private void Start()
        {
            targetFollow = PlayerManager.Instance.transform;
            CameraOnMain(Vector3.zero);
            SizeCam();
            EnablePostProcess(false);
        }
        public void SizeCam()
        {
            sizeCamera = new Vector2();
            sizeCamera.y = CAMERA_ORTHOSIZE * 2;
            float w = Screen.width;
            float h = Screen.height;
            float screenRatio = w / h;
            sizeCamera.x = sizeCamera.y * w / h;
        }
        public void EnablePostProcess(bool status)
        {
            postProcessing.enabled = status;
        }
        public void ReviveSaturation()
        {
            EnablePostProcess(true);
            float value = -0.7f;
            postProcessing.Saturation = -0.7f;
            DOTween.To(() => value, x => value = x, 0, 1).OnUpdate(() =>
            {
                postProcessing.Saturation = value;
            }).OnComplete(() =>
            {
                EnablePostProcess(false);
            });
        }
        public void DeathSaturation()
        {
            EnablePostProcess(true);
            float value = 0;
            postProcessing.Saturation = 0;
            DOTween.To(() => value, x => value = x, -0.7f, 1).OnUpdate(() =>
            {
                postProcessing.Saturation = value;
            });
        }
        void Update()
        {
            HandleFollow();
            if (Input.GetKeyDown(KeyCode.K))
            {
                CameraController.Instance.ShakeCamera(.5f, 1f, 10, 90, true);
            }
        }
        public void SetLimitCamera(float _minX, float _minY, float _maxX, float _maxY)
        {
            maxX = _maxX;
            maxY = _maxY;
            minX = _minX;
            minY = _minY;
        }
        void HandleFollow()
        {
            if (targetFollow == null || stopFollowing) return;
            Vector3 newPosition = targetFollow.position;
            if (newPosition.x - sizeCamera.x / 2f <= minX - 1)
            {
                newPosition.x = minX + sizeCamera.x / 2f;
            }
            if (newPosition.x + sizeCamera.x / 2f >= maxX + 1)
            {
                newPosition.x = maxX - sizeCamera.x / 2f;
            }
            if (newPosition.y - sizeCamera.y / 2f <= minY - 3)
            {
                newPosition.y = (minY - 3) + sizeCamera.y / 2f;
            }
            if (newPosition.y + sizeCamera.y / 2f >= maxY + 1)
            {
                newPosition.y = maxY - sizeCamera.y / 2f;
            }
            transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, 0.1f);
        }
        Tween KillCameraShake;
        public void ShakeCamera(float dur, float stre, int vib, int randness, bool smooth = true)
        {
            if (KillCameraShake != null)
            {
                KillCameraShake.Kill();
                cameraMain.transform.localPosition = offset;
            }
            KillCameraShake = cameraMain.DOShakePosition(dur, stre, vib, randness, smooth).OnComplete(() =>
            {
                cameraMain.transform.localPosition = offset;
            });
        }
        public float GetMaxLimitX()
        {
            return transform.position.x + sizeCamera.x / 2f;
        }
        public float GetMinLimitX()
        {
            return transform.position.x - sizeCamera.x / 2f;
        }
        public void SetTargetFollow(Transform target, bool unlock)
        {
            this.unlock = unlock;
            targetFollow = target;
        }

        public void SetOrthoSize(Vector2 size, float duration)
        {
            float screenRatio = (float)Screen.width / (float)Screen.height;
            float targetRatio = size.x / size.y;
            if (orthoTween != null)
            {
                orthoTween.Kill();
            }
            if (screenRatio >= targetRatio)
            {
                orthoTween = cameraMain.DOOrthoSize(size.y / 2f, duration);
            }
            else
            {
                float differenceSize = targetRatio / screenRatio;
                orthoTween = cameraMain.DOOrthoSize(size.y / 2f * differenceSize, duration);
            }
        }
        public void SetOffset(Vector3 newOffset)
        {
            offset = newOffset;
            offset.z = -20;
            cameraMain.transform.localPosition = offset;
        }
        Tweener orthoTween;
        public void SetOrthoSize(float size, float duration, bool lockSettingSize)
        {
            if (orthoTween != null)
            {
                orthoTween.Kill();
            }
            orthoTween = cameraMain.DOOrthoSize(size, duration);
        }
        public void CamOnWin()
        {
            orthoTween = cameraMain.DOOrthoSize(2.6f, .7f);
        }
        public void CamOnLose()
        {
            orthoTween = cameraMain.DOOrthoSize(2.6f, .7f);
        }
        public void CamOnStart()
        {
            EnablePostProcess(false);
            SetOffset(new Vector3(0, 1.5f));
            transform.position = targetFollow.position;
            cameraMain.orthographicSize = CAMERA_ORTHOSIZE;
        }
        public void CameraOnMain(Vector3 posCamStart)
        {
            SetTargetFollow(PlayerManager.Instance.transform, false);
            EnablePostProcess(false);
            SetOffset(new Vector3(0, 1.5f));
            Vector3 currPos = transform.position;
            currPos.x = posCamStart.x + offset.x;
            currPos.y = posCamStart.y + offset.y;
            currPos.z = offset.z;
            transform.position = currPos;
            cameraMain.orthographicSize = CAMERA_ORTHOSIZE;
        }
        public Bounds GetBounds()
        {
            return new Bounds((Vector2)transform.position, sizeCamera);
        }
    }
}

