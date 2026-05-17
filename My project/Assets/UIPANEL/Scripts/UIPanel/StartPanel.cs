using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartPanel : BasePanel
{
    private static string Name="StartPanel";
    private static string Path="Panel/StartPanel";
    public static readonly UITepy UIPanelType = new UITepy(Path,Name);
    
    // 菜单相关变量
    public List<Text> menuItems;
    public List<TMP_Text> tmpMenuItems;
    public List<Image> menuIcons;
    private int selectedIndex = 0;
    private Color normalColor = Color.white; // 白色
    private Color selectedColor = new Color(0.196f, 0.780f, 0.969f); // 蓝青色 (RGB: 50, 199, 247)
    
    // 按钮引用
    private Button backButton;
    private Button loadButton;
    private Button settingButton;
    
    // 是否使用键盘导航
    private bool useKeyboardNavigation = true;
    // 是否是鼠标选中状态
    private bool isMouseSelected = false;
public StartPanel():base(UIPanelType)
{
    // uiType is already set in the base constructor
}
    public override void ONStart()
    {
        base.ONStart();
        
        // 获取按钮引用
        backButton = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj,"Back");
        loadButton = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj,"Load");
        settingButton = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj,"Setting");
        
        // 添加按钮点击事件
        backButton.onClick.AddListener(BackButtonClick);
        loadButton.onClick.AddListener(LoadButtonClick);
        settingButton.onClick.AddListener(SettingButtonClick);
        
        // 初始化菜单选项列表（这会根据实际获取顺序设置索引）
        InitializeMenuItems();
        
        // 添加鼠标悬停事件（在初始化之后根据实际按钮设置索引）
        AddMouseHoverEvent(backButton, GetButtonMenuIndex(backButton));
        AddMouseHoverEvent(loadButton, GetButtonMenuIndex(loadButton));
        AddMouseHoverEvent(settingButton, GetButtonMenuIndex(settingButton));
        
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
        
        // 确保按钮有Image组件作为点击区域
        Image image = button.GetComponent<Image>();
        if (image == null)
        {
            image = button.gameObject.AddComponent<Image>();
            // 设置一个透明的颜色作为点击区域
            image.color = new Color(1, 1, 1, 0);
            Debug.LogWarning($"按钮 {button.name} 没有Image组件，已自动添加");
        }
        
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
        // 恢复所有菜单选项到未选中状态
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
            // 获取Text或TMP_Text组件
            Text textItem = menuItems != null && i < menuItems.Count ? menuItems[i] : null;
            TMP_Text tmpTextItem = tmpMenuItems != null && i < tmpMenuItems.Count ? tmpMenuItems[i] : null;
            
            if (textItem == null && tmpTextItem == null) continue;
            
            // 设置为未选中状态
            if (textItem != null)
            {
                textItem.color = normalColor;
                textItem.fontStyle = FontStyle.Normal;
                
                // 移除发光效果
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
                
                // 移除发光效果
                tmpTextItem.outlineWidth = 0.05f;
            }
            
            // 更新图标
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
        
        Debug.Log("开始初始化菜单选项...");
        
        // 获取所有按钮组件
        Button[] buttons = ActiveObj.GetComponentsInChildren<Button>();
        Debug.Log($"找到 {buttons.Length} 个按钮");
        
        foreach (Button button in buttons)
        {
            Debug.Log($"处理按钮: {button.name}");
            
            // 首先尝试获取TMP_Text组件
            TMP_Text tmpText = button.GetComponentInChildren<TMP_Text>();
            
            // 如果没有找到TMP_Text，尝试获取普通Text组件
            if (tmpText == null)
            {
                Text text = button.GetComponentInChildren<Text>();
                if (text != null)
                {
                    menuItems.Add(text);
                    tmpMenuItems.Add(null);
                    Debug.Log($"添加Text菜单选项: {text.text}");
                }
                else
                {
                    Debug.LogWarning($"按钮 {button.name} 没有Text或TMP_Text组件");
                    continue;
                }
            }
            else
            {
                tmpMenuItems.Add(tmpText);
                menuItems.Add(null);
                Debug.Log($"添加TMP_Text菜单选项: {tmpText.text}");
            }
            
            // 尝试获取图标（假设图标是按钮的子对象，名为Icon）
            Image icon = button.transform.Find("Icon")?.GetComponent<Image>();
            if (icon != null)
            {
                menuIcons.Add(icon);
                Debug.Log($"找到图标: {icon.name}");
            }
            else
            {
                menuIcons.Add(null);
                Debug.Log($"按钮 {button.name} 没有图标");
            }
        }
        
        Debug.Log($"初始化完成，共 {menuItems.Count + tmpMenuItems.Count} 个菜单选项");
        
        // 如果没有找到任何菜单选项，尝试直接查找名为MenuItems的父对象
        if (menuItems.Count == 0 && tmpMenuItems.Count == 0)
        {
            Debug.LogWarning("没有找到任何菜单选项，尝试查找MenuItems父对象");
            Transform menuParent = ActiveObj.transform.Find("MenuItems");
            if (menuParent != null)
            {
                // 获取所有Text和TMP_Text组件
                Text[] texts = menuParent.GetComponentsInChildren<Text>();
                TMP_Text[] tmpTexts = menuParent.GetComponentsInChildren<TMP_Text>();
                
                foreach (Text text in texts)
                {
                    menuItems.Add(text);
                    tmpMenuItems.Add(null);
                    menuIcons.Add(null);
                    Debug.Log($"从MenuItems父对象添加Text: {text.text}");
                }
                
                foreach (TMP_Text tmpText in tmpTexts)
                {
                    tmpMenuItems.Add(tmpText);
                    menuItems.Add(null);
                    menuIcons.Add(null);
                    Debug.Log($"从MenuItems父对象添加TMP_Text: {tmpText.text}");
                }
            }
        }
    }
    
    private void UpdateMenuSelection()
    {
        // 检查菜单选项列表
        bool hasMenuItems = (menuItems != null && menuItems.Count > 0) || (tmpMenuItems != null && tmpMenuItems.Count > 0);
        if (!hasMenuItems)
        {
            Debug.LogWarning("没有可用的菜单选项");
            return;
        }
        
        int totalItems = menuItems != null ? menuItems.Count : tmpMenuItems.Count;
        
        for (int i = 0; i < totalItems; i++)
        {
            // 获取Text或TMP_Text组件
            Text textItem = menuItems != null && i < menuItems.Count ? menuItems[i] : null;
            TMP_Text tmpTextItem = tmpMenuItems != null && i < tmpMenuItems.Count ? tmpMenuItems[i] : null;
            
            if (textItem == null && tmpTextItem == null) continue;
            
            if (i == selectedIndex)
            {
                // 选中状态
                if (textItem != null)
                {
                    textItem.color = selectedColor;
                    textItem.fontStyle = FontStyle.Bold;
                    
                    // 添加发光效果
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
                    
                    // 添加发光效果
                    tmpTextItem.outlineColor = selectedColor;
                    tmpTextItem.outlineWidth = 0.2f;
                }
                
                // 更新图标
                if (menuIcons != null && i < menuIcons.Count && menuIcons[i] != null)
                {
                    menuIcons[i].color = selectedColor;
                }
            }
            else
            {
                // 未选中状态
                if (textItem != null)
                {
                    textItem.color = normalColor;
                    textItem.fontStyle = FontStyle.Normal;
                    
                    // 移除发光效果
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
                    
                    // 移除发光效果
                    tmpTextItem.outlineWidth = 0.05f;
                }
                
                // 更新图标
                if (menuIcons != null && i < menuIcons.Count && menuIcons[i] != null)
                {
                    menuIcons[i].color = normalColor;
                }
            }
        }
    }
    
    private void HandleInput()
    {
        // 如果正在使用鼠标交互，则忽略键盘输入
        if (!useKeyboardNavigation || isMouseSelected) return;
        
        // 检查是否有可用的菜单选项
        bool hasMenuItems = (menuItems != null && menuItems.Count > 0) || (tmpMenuItems != null && tmpMenuItems.Count > 0);
        if (!hasMenuItems) return;
        
        // 获取总菜单数量
        int totalItems = menuItems != null && menuItems.Count > 0 ? menuItems.Count : tmpMenuItems.Count;
        
        // 键盘导航
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
        // 获取总菜单数量
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
                if (backButton != null)
                {
                    BackButtonClick();
                }
                break;
            case 1:
                if (loadButton != null)
                {
                    LoadButtonClick();
                }
                break;
            case 2:
                if (settingButton != null)
                {
                    SettingButtonClick();
                }
                break;
        }
    }
    private void SettingButtonClick()
    {
        // 播放点击音效
        AudioManager.PlayClick();
        GameRoot.GetInstance().UIManager_Root.Push(new SettingPanel());
    }
    private void BackButtonClick()
    {
        // 播放点击音效
        AudioManager.PlayClick();
        GameRoot.GetInstance().UIManager_Root.Push(new QuitPanel());
    }
    private void LoadButtonClick()
    {
        AudioManager.PlayClick();
        Scene2 scene2 = new Scene2();
        GameRoot.GetInstance().ScenesControl_Root.LoadSceneAsync(scene2.SceneName, scene2);
    }
    public override void OnEnable()
    {
        base.OnEnable();
    }
    public override void OnDisable()
    {
        Debug.Log("StartPanel IS back");
        base.OnDisable();
    }
    public override void OnDestroy()
    {
        // 取消注册Update方法
        GameRoot.GetInstance().UnregisterUpdateMethod(HandleInput);
        base.OnDestroy();
    }
}
