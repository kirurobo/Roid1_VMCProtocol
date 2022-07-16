/**
* デスクトップマスコット的サンプルのUI制御
* 
* Author: Kirurobo http://twitter.com/kirurobo
* License: MIT
*/

using System;
using UnityEngine;
using UnityEngine.UI;
using Kirurobo;

/// <summary>
/// WindowControllerの設定をToggleでオン／オフするサンプル
/// </summary>
public class DesktopMascotUI : MonoBehaviour
{
    private UniWindowController uniwinc;
    private UniWindowMoveHandle uniWinMoveHandle;
    private RectTransform canvasRect;

    private float mouseMoveSS = 0f;           // Sum of mouse trajectory squares. [px^2]
    private float mouseMoveSSThreshold = 36f; // Click (not dragging) threshold. [px^2]
    private Vector3 lastMousePosition;        // Right clicked position.
    private float touchDuration = 0f;
    private float touchDurationThreshold = 0.5f;   // Long tap time threshold. [s]

    public Toggle transparentToggle;
    public Toggle topmostToggle;
    public Toggle zoomedToggle;
    private Toggle dragMoveToggle;
    private Toggle showBorderlineToggle;
    public Dropdown transparentTypeDropdown;
    public Text messageText;
    public Button menuCloseButton;
    public RectTransform menuPanel;
    public RectTransform borderlinePanel;

    /// <summary>
    /// 初期化
    /// </summary>
    void Start()
    {
        // UniWindowController を探す
        uniwinc = UniWindowController.current;
        
        // UniWindowDragMove を探す
        uniWinMoveHandle = GameObject.FindObjectOfType<UniWindowMoveHandle>();

        // CanvasのRectTransform取得
        if (menuPanel) canvasRect = menuPanel.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        
        // Toggleのチェック状態を、現在の状態に合わせる
        UpdateUI();

        if (uniwinc)
        {
            // UIを操作された際にはウィンドウに反映されるようにする
            transparentToggle?.onValueChanged.AddListener(val => uniwinc.isTransparent = val);
            topmostToggle?.onValueChanged.AddListener(val => uniwinc.isTopmost = val);
            zoomedToggle?.onValueChanged.AddListener(val => uniwinc.isZoomed = val);
            transparentTypeDropdown?.onValueChanged.AddListener(val => uniwinc.SetTransparentType((UniWindowController.TransparentType)val));
            menuCloseButton?.onClick.AddListener(CloseMenu);

            if (uniWinMoveHandle) dragMoveToggle?.onValueChanged.AddListener(val => uniWinMoveHandle.enabled = val);

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            // Windows でなければ、透過方法の選択は無効とする
            //if (transparentTypeDropdown) transparentTypeDropdown.interactable = false;
            //if (transparentTypeDropdown) transparentTypeDropdown.enabled = false;
            if (transparentTypeDropdown) transparentTypeDropdown.gameObject.SetActive(false);
#endif
            
            // Add events
            uniwinc.OnStateChanged += (type) =>
            {
                UpdateUI();
                ShowEventMessage("State changed: " + type);
            };
            uniwinc.OnMonitorChanged += () => {
                UpdateUI();
                ShowEventMessage("Resolution changed!"); 
            };
            uniwinc.OnDropFiles += files =>
            {
                ShowEventMessage(string.Join(Environment.NewLine, files));
            };
        }

        // UinWinCが準備できてなくても動くListener
        showBorderlineToggle?.onValueChanged.AddListener(val => borderlinePanel.gameObject.SetActive(val));
    }

    /// <summary>
    /// Show the message with timeout
    /// </summary>
    /// <param name="message"></param>
    private void ShowEventMessage(string message)
    {
        //// 特に表示しない
        // if (messageText) messageText.text = message;
        //
        // Debug.Log(message);
    }

    /// <summary>
    /// 毎フレーム行う処理
    /// </summary>
    private void Update()
    {
        // マウス右ボタンクリックでメニューを表示させる。閾値以下の移動ならクリックとみなす。
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
            touchDuration = 0f;
        }
        if (Input.GetMouseButton(1))
        {
            mouseMoveSS += (Input.mousePosition - lastMousePosition).sqrMagnitude;
        }
        if (Input.GetMouseButtonUp(1))
        {
            if (mouseMoveSS < mouseMoveSSThreshold)
            {
                ShowMenu(lastMousePosition);
            }
            mouseMoveSS = 0f;
            touchDuration = 0f;
        }
        
        // ロングタッチでもメニューを表示させる
        if (Input.touchSupported && (Input.touchCount > 0))
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                lastMousePosition = Input.mousePosition;
                touchDuration = 0f;
            }
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                mouseMoveSS += touch.deltaPosition.sqrMagnitude;
                touchDuration += touch.deltaTime;
            }
            if (touch.phase == TouchPhase.Ended)
            {
                if ((mouseMoveSS < mouseMoveSSThreshold) && (touchDuration >= touchDurationThreshold))
                {
                    ShowMenu(lastMousePosition);
                }
                mouseMoveSS = 0f;
                touchDuration = 0f;
            }
        }

        // キーでも設定変更
        if (uniwinc)
        {
            // Toggle transparent
            if (Input.GetKeyUp(KeyCode.T))
            {
                uniwinc.isTransparent = !uniwinc.isTransparent;
            }

            // Toggle always on the front
            if (Input.GetKeyUp(KeyCode.F))
            {
                uniwinc.isTopmost = !uniwinc.isTopmost;
            }

            // Toggle zoom
            if (Input.GetKeyUp(KeyCode.Z))
            {
                uniwinc.isZoomed = !uniwinc.isZoomed;
            }
        }

        // Quit or stop playing when pressed [ESC]
        if (Input.GetKey(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    /// <summary>
    /// Refresh UI on focused
    /// </summary>
    /// <param name="hasFocus"></param>
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            UpdateUI();
        }
    }

    /// <summary>
    /// 指定した座標にコンテキストメニューを表示する
    /// </summary>
    /// <param name="position">中心座標指定</param>
    private void ShowMenu(Vector2 position)
    {
        if (menuPanel)
        {
            Vector2 pos = position * (canvasRect.sizeDelta.x / Screen.width);
            float w = menuPanel.rect.width;
            float h = menuPanel.rect.height;

            // 指定座標に中心が来る前提で位置調整
            pos.y = Mathf.Max(Mathf.Min(pos.y, Screen.height - h / 2f), h / 2f);   // はみ出していれば上に寄せる
            pos.x = Mathf.Max(Mathf.Min(pos.x, Screen.width - w / 2f), w / 2f);    // 右にはみ出していれば左に寄せる

            menuPanel.pivot = Vector2.one * 0.5f;    // Set the center
            menuPanel.anchorMin = Vector2.zero;
            menuPanel.anchorMax = Vector2.zero;
            menuPanel.anchoredPosition = pos;
            menuPanel.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// コンテキストメニューを閉じる
    /// </summary>
    private void CloseMenu()
    {
        if (menuPanel)
        {
            menuPanel.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 実際の状態をUI表示に反映
    /// </summary>
    private void UpdateUI()
    {
        if (uniwinc)
        {
            if (transparentToggle)
            {
                transparentToggle.isOn = uniwinc.isTransparent;
            }

            if (topmostToggle)
            {
                topmostToggle.isOn = uniwinc.isTopmost;
            }
            
            if (zoomedToggle)
            {
                zoomedToggle.isOn = uniwinc.isZoomed;
            }

            if (transparentTypeDropdown)
            {
                transparentTypeDropdown.value = (int)uniwinc.transparentType;
                transparentTypeDropdown.RefreshShownValue();
            }
        }

        // UniWinC 無しでも動作する部分
        if (showBorderlineToggle && borderlinePanel)
        {
            borderlinePanel.gameObject.SetActive(showBorderlineToggle.isOn);
        }
    }

    /// <summary>
    /// テキスト枠がUIにあれば、そこにメッセージを出す。無ければコンソールに出力
    /// </summary>
    /// <param name="text"></param>
    public void OutputMessage(string text)
    {
        if (messageText)
        {
            messageText.text = text;
        }
        else
        {
            Debug.Log(text);
        }
    }
}
