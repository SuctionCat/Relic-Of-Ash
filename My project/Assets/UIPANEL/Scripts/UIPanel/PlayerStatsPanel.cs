using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsPanel : BasePanel
{
    private static string Name="PlayerStatsPanel";
    private static string Path="Panel/PlayerStatsPanel";
    public static readonly UITepy UIPanelType = new UITepy(Path,Name);

    // 技能按钮引用
    private Button buttonE;
    private Button buttonQ;
    
    // 技能冷却时间（秒）
    private float cooldownE = 5f;
    private float cooldownQ = 3f;
    
    // 当前冷却时间
    private float currentCooldownE = 0f;
    private float currentCooldownQ = 0f;
    
    // 冷却时间文本（可选）
    private Text cooldownTextE;
    private Text cooldownTextQ;

public PlayerStatsPanel():base(UIPanelType)
{
    // uiType is already set in the base constructor
}
    // Start is called before the first frame update
    public override void ONStart()
    {
        base.ONStart();
        UImched.GetInstance().GetOrAddComponent<Button>(ActiveObj,"Back").onClick.AddListener(BackButtonClick);
        
        // 获取技能按钮引用
        buttonE = UImched.GetInstance().GetOrAddComponent<Button>(ActiveObj,"ButtonE");
        buttonQ = UImched.GetInstance().GetOrAddComponent<Button>(ActiveObj,"ButtonQ");
        
        // 为技能按钮添加点击事件
        if(buttonE != null)
        {
            buttonE.onClick.AddListener(OnButtonEClick);
        }
        
        if(buttonQ != null)
        {
            buttonQ.onClick.AddListener(OnButtonQClick);
        }
        
        // 尝试获取冷却时间文本（如果存在）
        cooldownTextE = UImched.GetInstance().GetOrAddComponent<Text>(ActiveObj,"CooldownTextE");
        cooldownTextQ = UImched.GetInstance().GetOrAddComponent<Text>(ActiveObj,"CooldownTextQ");
        
        // 初始隐藏冷却文本
        if(cooldownTextE != null)
        {
            cooldownTextE.gameObject.SetActive(false);
        }
        if(cooldownTextQ != null)
        {
            cooldownTextQ.gameObject.SetActive(false);
        }
        
        // 注册Update方法到游戏循环
        GameRoot.GetInstance().RegisterUpdateMethod(Update);
    }
    
    private void OnButtonEClick()
    {
        // 检查是否在冷却中
        if(currentCooldownE <= 0)
        {
            // 播放点击音效
            AudioManager.PlayClick();
            
            // 开始技能冷却
            currentCooldownE = cooldownE;
            buttonE.interactable = false;
            
            // 这里可以添加技能E的具体逻辑
            Debug.Log("技能E被使用");
        }
    }
    
    private void OnButtonQClick()
    {
        // 检查是否在冷却中
        if(currentCooldownQ <= 0)
        {
            // 播放点击音效
            AudioManager.PlayClick();
            
            // 开始技能冷却
            currentCooldownQ = cooldownQ;
            buttonQ.interactable = false;
            
            // 这里可以添加技能Q的具体逻辑
            Debug.Log("技能Q被使用");
        }
    }
    
    private void Update()
    {
        // 检测键盘输入
        if(Input.GetKeyDown(KeyCode.E))
        {
            OnButtonEClick();
        }
        
        if(Input.GetKeyDown(KeyCode.Q))
        {
            OnButtonQClick();
        }
        
        // 更新技能E的冷却
        if(currentCooldownE > 0)
        {
            currentCooldownE -= Time.deltaTime;
            
            // 更新冷却文本
            if(cooldownTextE != null)
            {
                cooldownTextE.text = Mathf.Ceil(currentCooldownE).ToString();
                cooldownTextE.gameObject.SetActive(true);
            }
            
            // 冷却结束
            if(currentCooldownE <= 0)
            {
                buttonE.interactable = true;
                if(cooldownTextE != null)
                {
                    cooldownTextE.gameObject.SetActive(false);
                }
            }
        }
        
        // 更新技能Q的冷却
        if(currentCooldownQ > 0)
        {
            currentCooldownQ -= Time.deltaTime;
            
            // 更新冷却文本
            if(cooldownTextQ != null)
            {
                cooldownTextQ.text = Mathf.Ceil(currentCooldownQ).ToString();
                cooldownTextQ.gameObject.SetActive(true);
            }
            
            // 冷却结束
            if(currentCooldownQ <= 0)
            {
                buttonQ.interactable = true;
                if(cooldownTextQ != null)
                {
                    cooldownTextQ.gameObject.SetActive(false);
                }
            }
        }
    }
    
    private void BackButtonClick()
    {
        // 播放点击音效
        AudioManager.PlayClick();
        GameRoot.GetInstance().UIManager_Root.Pop(false);
    }
    
    public override void OnEnable()
    {
        base.OnEnable();
        UIManager.GetInstance().DisableAllPanelsExcept(this);
    }
    
    public override void OnDisable()
    {
        Debug.Log("PlayerStatsPanel IS back");
        base.OnDisable();
        UIManager.GetInstance().RestorePreviousPanelInteractivity();
    }
    
    public override void OnDestroy()
    {
        // 取消注册Update方法
        GameRoot.GetInstance().UnregisterUpdateMethod(Update);
        base.OnDestroy();
    }
   
}