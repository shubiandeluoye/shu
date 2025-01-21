using UnityEngine;
using UnityEngine.UI;
using UIModule.Core;
using System.Collections;

namespace UIModule.Components
{
    /// <summary>
    /// Toast提示UI组件
    /// 负责：
    /// 1. 显示临时消息
    /// 2. 自动淡出
    /// 3. 消息队列管理
    /// </summary>
    public class ToastUI : UIBase
    {
        [Header("UI References")]
        [SerializeField] private Text messageText;
        
        [Header("Toast Settings")]
        [SerializeField] private float defaultDuration = 2f;
        [SerializeField] private float fadeInTime = 0.2f;
        [SerializeField] private float fadeOutTime = 0.3f;

        private Coroutine currentToastCoroutine;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Hide(); // 初始化时隐藏
        }

        public void ShowMessage(string message, float duration = 0)
        {
            if (currentToastCoroutine != null)
            {
                StopCoroutine(currentToastCoroutine);
            }

            if (duration <= 0) duration = defaultDuration;
            messageText.text = message;
            currentToastCoroutine = StartCoroutine(ShowToastCoroutine(duration));
        }

        private IEnumerator ShowToastCoroutine(float duration)
        {
            Show();

            // 淡入
            float elapsed = 0;
            while (elapsed < fadeInTime)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = elapsed / fadeInTime;
                yield return null;
            }
            canvasGroup.alpha = 1;

            // 显示持续时间
            yield return new WaitForSeconds(duration);

            // 淡出
            elapsed = fadeOutTime;
            while (elapsed > 0)
            {
                elapsed -= Time.deltaTime;
                canvasGroup.alpha = elapsed / fadeOutTime;
                yield return null;
            }
            canvasGroup.alpha = 0;

            Hide();
            currentToastCoroutine = null;
        }

        public override void UpdateUI()
        {
            // Toast 不需要定期更新
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (currentToastCoroutine != null)
            {
                StopCoroutine(currentToastCoroutine);
            }
        }
    }
} 