using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuitPanel : BasePanel
{
    private static string Name="QuitPanel";
    private static string Path="Panel/QuitPanel";
    public static readonly UITepy UIPanelType = new UITepy(Path,Name);

    // 菜单相关变量
    public List<Text> menuItems;
    public List<TMP_Text> tmpMenuItems;
    public List<Image> menuIcons;
    private int selectedIndex = 0;
    private Color normalColor = Color.white; // 白色
    private Color selectedColor = new Color(0.196f, 0.780f, 0.969f); // 蓝青色 (RGB: 50, 199, 247)

    // 按钮引用
    private Button quitButton;
    private Button backButton;

    // 是否使用键盘导航
    private bool useKeyboardNavigation = true;
    // 是否是鼠标选中状态
    private bool isMouseSelected = false;

public QuitPanel():base(UIPanelType)
{
    // uiType is already set in the base constructor
}
    public override void ONStart()
    {
        base.ONStart();

        // 获取按钮引用
        Button[] buttons = ActiveObj.GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            btn.interactable = true;
            
            if (btn.name == "Quit")
            {
                quitButton = btn;
                quitButton.onClick.AddListener(QuitButtonClick);
            }
            else if (btn.name == "Back")
            {
                backButton = btn;
                backButton.onClick.AddListener(BackButtonClick);
            }
        }

        // 初始化菜单选项列表（这会根据实际获取顺序设置索引）
        InitializeMenuItems();

        // 添加鼠标悬停事件（在初始化之后根据实际按钮设置索引）
        AddMouseHoverEvent(quitButton, GetButtonMenuIndex(quitButton));
        AddMouseHoverEvent(backButton, GetButtonMenuIndex(backButton));

        // 更新菜单选中状态
        UpdateMenuSelection();

        // 注册Update方法用于键盘导航
        GameRoot.GetInstance().RegisterUpdateMethod(HandleInput);
    }

    private int GetButtonMenuIndex(Button button)
    {
        // 查找按钮在菜单列表中的实际索引
        Button[] allButtons = ActiveObj.GetComponentsInChildren<Button>();
        for (int i = 0; i < allButtons.Length; i++)
        {
            if (allButtons[i] == button)
            {
                return i;
            }
        }
        return 0;
    }

    private void AddMouseHoverEvent(Button button, int index)
    {
        if (button == null) return;

        // 使用EventTrigger组件处理鼠标事件
        UnityEngine.EventSystems.EventTrigger trigger = button.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }

        // 清除现有的事件，避免重复添加
        trigger.triggers.Clear();

        // 添加鼠标进入事件
        UnityEngine.EventSystems.EventTrigger.Entry enterEvent = new UnityEngine.EventSystems.EventTrigger.Entry();
        enterEvent.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        enterEvent.callback.AddListener((data) => { OnMouseEnter(index); });
        trigger.triggers.Add(enterEvent);

        // 添加鼠标离开事件
        UnityEngine.EventSystems.EventTrigger.Entry exitEvent = new UnityEngine.EventSystems.EventTrigger.Entry();
        exitEvent.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
        exitEvent.callback.AddListener((data) => { OnMouseExit(); });
        trigger.triggers.Add(exitEvent);
    }

    private void OnMouseEnter(int index)
    {
        // 鼠标悬停时更新选中状态
        isMouseSelected = true;
        selectedIndex = index;
        UpdateMenuSelection();
    }

    private void OnMouseExit()
    {
        // 鼠标离开时恢复键盘导航状态
        isMouseSelected = false;
        ResetMenuSelection();
    }

    private void ResetMenuSelection()
    {
        // 检查菜单选项列表
        bool hasMenuItems = (menuItems != null && menuItems.Count > 0) || (tmpMenuItems != null && tmpMenuItems.Count > 0);
        if (!hasMenuItems) return;

        int totalItems = menuItems != null ? menuItems.Count : tmpMenuItems.Count;

        for (int i = 0; i < totalItems; i++)
        {
            Text textItem = menuItems != null && i < menuItems.Count ? menuItems[i] : null;
            TMP_Text tmpTextItem = tmpMenuItems != null && i < tmpMenuItems.Count ? tmpMenuItems[i] : null;

            if (textItem == null && tmpTextItem == null) continue;

            if (textItem != null)
            {
                textItem.color = normalColor;
                textItem.fontStyle = FontStyle.Normal;

                Outline outline = textItem.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = false;
                }
            }
            else if (tmpTextItem != null)
            {
                tmpTextItem.color = normalColor;
                tmpTextItem.fontStyle = FontStyles.Normal;
                tmpTextItem.outlineWidth = 0.05f;
            }

            if (menuIcons != null && i < menuIcons.Count && menuIcons[i] != null)
            {
                menuIcons[i].color = normalColor;
            }
        }
    }

    private void InitializeMenuItems()
    {
        menuItems = new List<Text>();
        tmpMenuItems = new List<TMP_Text>();
        menuIcons = new List<Image>();

        Button[] buttons = ActiveObj.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            TMP_Text tmpText = button.GetComponentInChildren<TMP_Text>();

            if (tmpText == null)
            {
                Text text = button.GetComponentInChildren<Text>();
                if (text != null)
                {
                    menuItems.Add(text);
                    tmpMenuItems.Add(null);
                }
                else
                {
                    continue;
                }
            }
            else
            {
                tmpMenuItems.Add(tmpText);
                menuItems.Add(null);
            }

            Image icon = button.transform.Find("Icon")?.GetComponent<Image>();
            menuIcons.Add(icon);
        }
    }

    private void UpdateMenuSelection()
    {
        bool hasMenuItems = (menuItems != null && menuItems.Count > 0) || (tmpMenuItems != null && tmpMenuItems.Count > 0);
        if (!hasMenuItems)
        {
            Debug.LogWarning("没有可用的菜单选项");
            return;
        }

        int totalItems = menuItems != null ? menuItems.Count : tmpMenuItems.Count;

        for (int i = 0; i < totalItems; i++)
        {
            Text textItem = menuItems != null && i < menuItems.Count ? menuItems[i] : null;
            TMP_Text tmpTextItem = tmpMenuItems != null && i < tmpMenuItems.Count ? tmpMenuItems[i] : null;

            if (textItem == null && tmpTextItem == null) continue;

            if (i == selectedIndex)
            {
                if (textItem != null)
                {
                    textItem.color = selectedColor;
                    textItem.fontStyle = FontStyle.Bold;

                    Outline outline = textItem.GetComponent<Outline>();
                    if (outline == null)
                    {
                        outline = textItem.gameObject.AddComponent<Outline>();
                    }
                    outline.effectColor = selectedColor;
                    outline.effectDistance = new Vector2(2, 2);
                }
                else if (tmpTextItem != null)
                {
                    tmpTextItem.color = selectedColor;
                    tmpTextItem.fontStyle = FontStyles.Bold;
                    tmpTextItem.outlineColor = selectedColor;
                    tmpTextItem.outlineWidth = 0.2f;
                }

                if (menuIcons != null && i < menuIcons.Count && menuIcons[i] != null)
                {
                    menuIcons[i].color = selectedColor;
                }
            }
            else
            {
                if (textItem != null)
                {
                    textItem.color = normalColor;
                    textItem.fontStyle = FontStyle.Normal;

                    Outline outline = textItem.GetComponent<Outline>();
                    if (outline != null)
                    {
                        outline.enabled = false;
                    }
                }
                else if (tmpTextItem != null)
                {
                    tmpTextItem.color = normalColor;
                    tmpTextItem.fontStyle = FontStyles.Normal;
                    tmpTextItem.outlineWidth = 0.05f;
                }

                if (menuIcons != null && i < menuIcons.Count && menuIcons[i] != null)
                {
                    menuIcons[i].color = normalColor;
                }
            }
        }
    }

    private void HandleInput()
    {
        if (!useKeyboardNavigation || isMouseSelected) return;

        bool hasMenuItems = (menuItems != null && menuItems.Count > 0) || (tmpMenuItems != null && tmpMenuItems.Count > 0);
        if (!hasMenuItems) return;

        int totalItems = menuItems != null && menuItems.Count > 0 ? menuItems.Count : tmpMenuItems.Count;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeSelection(-1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeSelection(1, totalItems);
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            SelectMenuItem();
        }
    }

    private void ChangeSelection(int direction, int totalItems = -1)
    {
        if (totalItems < 0)
        {
            totalItems = menuItems != null && menuItems.Count > 0 ? menuItems.Count : tmpMenuItems.Count;
        }

        int oldIndex = selectedIndex;
        selectedIndex = Mathf.Clamp(selectedIndex + direction, 0, totalItems - 1);

        if (oldIndex != selectedIndex)
        {
            UpdateMenuSelection();
            AudioManager.PlayClick();
        }
    }

    private void SelectMenuItem()
    {
        switch (selectedIndex)
        {
            case 0:
                if (quitButton != null)
                {
                    QuitButtonClick();
                }
                break;
            case 1:
                if (backButton != null)
                {
                    BackButtonClick();
                }
                break;
        }
    }

    private void QuitButtonClick()
    {
        AudioManager.PlayClick();
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void BackButtonClick()
    {
        AudioManager.PlayClick();
        
        if (GameRoot.GetInstance()?.UIManager_Root != null)
        {
            GameRoot.GetInstance().UIManager_Root.Pop(false);
        }
        else if (ActiveObj != null)
        {
            ActiveObj.SetActive(false);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();

        // 确保面板可以交互
        CanvasGroup canvasGroup = ActiveObj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = ActiveObj.AddComponent<CanvasGroup>();
        }
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public override void OnDisable()
    {
        Debug.Log("QuitPanel IS back");
        base.OnDisable();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
