/// <copyright>(c) 2017 WyzLink Inc. All rights reserved.</copyright>
/// <author>zq</author>
/// <summary>
/// UI零件分页界面
/// </summary>

using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPartsPage : MonoBehaviour
{
    /// <summary>
    /// 当前页面索引
    /// </summary>
    private int m_PageIndex = 1;

    /// <summary>
    /// 总页数
    /// </summary>
    private int m_PageCount = 0;

    /// <summary>
    /// 元素总个数
    /// </summary>
    private int m_ItemsCount = 0;

    /// <summary>
    /// 元素列表
    /// </summary>
    private List<Node> m_ItemsList;

    /// <summary>
    /// 一页显示的零件个数个
    /// </summary>
    private int Page_Count = 12;

    /// <summary>
    /// 上一页
    /// </summary>
    private Button PreviousPage;

    /// <summary>
    /// 下一页
    /// </summary>
    private Button NextPage;

    /// <summary>
    /// 显示当前页数的标签
    /// </summary>
    private Text m_PanelText;

    /// <summary>
    /// 所有零件的集合
    /// </summary>
    private List<Node> NodesList = new List<Node>();

    /// <summary>
    /// 零件类型
    /// </summary>
    private string m_Type;

    /// <summary>
    /// 刷新界面用的
    /// </summary>
    private int InitFlag = 0;

    private List<Button> BtnList = new List<Button>();          //零件按钮集合

    // Use this for initialization
    void Start()
    {

    }

    void Update()
    {
        if (InitFlag <= 2)   //自动布局的问题，刚开始没分配坐标，第二帧布局的坐标才正常，刚开始程序的时候按钮坐标都是一样的，全在（0，0，0）的位置
        {
            RefreshItems();   //调用刷新方法刷新界面
            InitFlag++;       //控制只刷新2帧，因为第二帧之后，界面坐标都已经分配
        }
    }

    /// <summary>
    /// UI界面初始化，会在程序开始，扫描零件之后，被调用，防止界面无数据
    /// </summary>
    public void Init()
    {
        if (null != NodesCommon.Instance)
        {
            //加载扫描之后的零件集合
            NodesList = NodesCommon.Instance.GetNodeList();
        }
        else
        {
            //如果零件集合，打印错误信息，方便找出问题
            Debug.LogError("NodesCommon没有初始化！");
        }

        //找到下一页的UI按钮
        NextPage = GameObject.Find("Canvas/BG/PartsUI/PartsBtn/NextPage").GetComponent<Button>();
        //找到上一页的UI按钮
        PreviousPage = GameObject.Find("Canvas/BG/PartsUI/PartsBtn/PreviousPage").GetComponent<Button>();
        //找到当前一页的UI文本
        m_PanelText = GameObject.Find("Canvas/BG/PartsUI/PartsBtn/ViewPageText").GetComponent<Text>();

        //为下一页按钮添加事件
        NextPage.onClick.AddListener(() => { Next(); });
        //为上一页按钮添加事件
        PreviousPage.onClick.AddListener(() => { Previous(); });

        foreach (Transform tran in transform)
        {
            BtnList.Add(tran.gameObject.GetComponent<Button>());
            EventTriggerListener.Get(tran.gameObject).onClick = BtnClick;
        }

        //刷新界面
        RefreshItems();
    }

    /// <summary>
    /// 下一页
    /// </summary>
    public void Next()
    {
        //如果零件总页数为0，那么无法下一页
        if (m_PageCount <= 0)
            return;
        //最后一页禁止向后翻页
        if (m_PageIndex > m_PageCount)
            return;
        //当前页数加1
        m_PageIndex += 1;
        //如果页数加1之后，超过总页数，那么停留在最后一页
        if (m_PageIndex >= m_PageCount)
            m_PageIndex = m_PageCount;
        //重新加载，并显示，当前页面的零件数据
        BindPage(m_PageIndex);
        m_PanelText.text = string.Format("第" + "{0}/{1}" + "页", m_PageIndex.ToString(), m_PageCount.ToString());
    }

    /// <summary>
    /// 上一页
    /// </summary>
    public void Previous()
    {
        //如果零件总页数为0，那么无法上一页
        if (m_PageCount <= 0)
            return;

        //第一页时禁止向前翻页
        if (m_PageIndex < 1)
            return;

        //当前页数减1
        m_PageIndex -= 1;

        //如果页数减1之后，，当前页数小于1了，那么停留在第一页
        if (m_PageIndex <= 1)
            m_PageIndex = 1;

        //重新加载，并显示，当前页面的零件数据
        BindPage(m_PageIndex);
        m_PanelText.text = string.Format("第" + "{0}/{1}" + "页", m_PageIndex.ToString(), m_PageCount.ToString());
    }

    /// <summary>
    /// 绑定指定索引处的页面，显示加载该页的零件元素和数据
    /// </summary>
    /// <param name="index">页面索引</param>
    private void BindPage(int index)
    {
        //列表处理，所有零件列表中没东西，那么后续操作无效
        if (m_ItemsList == null || m_ItemsCount <= 0)
            return;

        //索引处理，防止当前页数不合法，小于0或者大于总页数都是不合法的
        if (index < 0 || index > m_ItemsCount)
            return;

        //先将所有零件都隐藏，后续根据页数来决定那些是当前页该显示的零件，才将那一页的零件显示
        for (int i = 0; i < NodesList.Count; i++)
        {
            NodesList[i].gameObject.SetActive(false);
        }

        //分为1页和1页以上两种情况进行页面显示分析
        if (m_PageCount == 1)      //总页数只有一页的情况
        {
            //设置一个变量
            int canDisplay = 0;
            //遍历零件个数，当前项目为12
            for (int i = Page_Count; i > 0; i--)
            {
                //因为只有一页，所以这一页的零件个数等于所有零件个数，当变量小于这一页所有零件个数的时候
                if (canDisplay < m_ItemsCount)
                {
                    //绑定对零件按钮和零件数据进行绑定
                    BindGridItem(transform.GetChild(canDisplay), m_ItemsList[Page_Count - i]);
                    //同时显示被绑定的零件
                    transform.GetChild(canDisplay).gameObject.SetActive(true);
                    //被绑定的零件的按钮也要显示
                    m_ItemsList[Page_Count - i].gameObject.SetActive(true);
                }
                else
                {
                    //对超过canDispaly的零件按钮实施隐藏
                    transform.GetChild(canDisplay).gameObject.SetActive(false);
                }
                canDisplay += 1;
            }
            //以上作用，举例：总共只有5个零件，也就是1页能显示，而且多出7个零件按钮，那么多出来的7个按钮实施隐藏，只显示有零件进行绑定的零件和零件按钮
        }
        else if (m_PageCount > 1)           //总页数大于1的情况
        {
            //1页以上需要特别处理的是最后1页
            //和1页时的情况类似判断最后一页剩下的元素数目
            //第1页时显然剩下的为Page_Count所以不用处理
            if (index == m_PageCount)                 //只需要考虑最后一页，零件不够，那么多出的零件按钮隐藏的情况
            {
                int canDisplay = 0;             //声明一个变量，记录是第几个按钮
                for (int i = Page_Count; i > 0; i--)           //遍历零件个数，当前项目为12
                {
                    //计算最后一页的零件的个数，最后一页剩下的元素数目为 m_ItemsCount - Page_Count * (index-1)，那么当标记的零件按钮小于最后一页的零件，直接绑定
                    if (canDisplay < m_ItemsCount - Page_Count * (index - 1))
                    {
                        //绑定零件和零件按钮
                        BindGridItem(transform.GetChild(canDisplay), m_ItemsList[Page_Count * index - i]);
                        //显示被绑定的零件
                        transform.GetChild(canDisplay).gameObject.SetActive(true);
                        //显示有零件绑定的零件按钮
                        m_ItemsList[Page_Count * index - i].gameObject.SetActive(true);
                    }
                    else
                    {
                        //超过标记零件的，表示零件按钮已经没零件可以绑定了，那么那个零件按钮需要隐藏
                        transform.GetChild(canDisplay).gameObject.SetActive(false);
                    }
                    canDisplay += 1;
                }
            }
            else                    //不是最后一页，那么所有零件只需要与相应的零件按钮进行绑定就行
            {
                for (int i = Page_Count; i > 0; i--)
                {
                    //绑定零件和零件按钮
                    BindGridItem(transform.GetChild(Page_Count - i), m_ItemsList[Page_Count * index - i]);
                    //显示被绑定的零件
                    transform.GetChild(Page_Count - i).gameObject.SetActive(true);
                    //显示有零件绑定的零件按钮
                    m_ItemsList[Page_Count * index - i].gameObject.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// 将零件绑定到指定的零件按钮上
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="gridItem"></param>
    private void BindGridItem(Transform trans, Node gridItem)
    {
        //绑定零件按钮的名字，让按钮显示的名字等于零件名字
        trans.Find("Text").GetComponent<Text>().text = gridItem.PartName;
        //绑定零件的位置，让零件的位置等于零件按钮的位置
        gridItem.gameObject.transform.position = trans.GetChild(1).transform.position;
    }

    /// <summary>
    /// 类型变化，刷新零件界面
    /// </summary>
    /// <param name="type"></param>
    public void RefreshItems()
    {
        if (null != GameObject.Find("Canvas/BG/PartsUI/PartsClassPanel"))
        {
            m_Type = GameObject.Find("Canvas/BG/PartsUI/PartsClassPanel").GetComponent<UIPartsPanelClass>().GetPartsType();
        }

        //判断零件集合是否为空，如果为空就新建一个零件集合
        if (null == m_ItemsList)
        {
            m_ItemsList = new List<Node>();
        }
        else         //如果不为空，那么清空这个零件集合
        {
            m_ItemsList.Clear();           //清空前一类型的数据
        }

        //将扫描到的零件集合的零件添加到UI的零件集合中
        for (int i = 0; i < NodesList.Count; i++)
        {
            if (m_Type == NodesList[i].Type)
            {
                m_ItemsList.Add(NodesList[i]);
            }
        }

        //计算元素总个数
        m_ItemsCount = m_ItemsList.Count;

        //计算总页数
        m_PageCount = (m_ItemsCount % Page_Count) == 0 ? m_ItemsCount / Page_Count : (m_ItemsCount / Page_Count) + 1;

        if (m_PageIndex != 1)
        {
            m_PageIndex = 1;  //每次刷新都必须把页面重置，防止新的页面页数不够
        }

        //显示和加载当前页的零件和零件按钮
        BindPage(m_PageIndex);
        m_PanelText.text = string.Format("第" + "{0}/{1}" + "页", m_PageIndex.ToString(), m_PageCount.ToString());
    }

    /// <summary>
    /// 零件按钮的点击事件
    /// </summary>
    /// <param name="go">被点击的按钮在unity里面的游戏物体</param>
    private void BtnClick(GameObject go)
    {
        Node node;                                  //声明一个变量，用来代表被点击的零件

        GameObject gameobj;                         //克隆一份,作为安装的零件

        GameObject gameOb;                          //克隆一份作为提示

        //循环零件按钮
        for (int i = 0; i < BtnList.Count; i++)
        {
            //零件按钮的名称等于被点击的物体的名字
            if (BtnList[i].name == go.name)
            {
                //获取被点击按钮对应的LCD1零件物体的Node对象
                node = m_ItemsList[(m_PageIndex - 1) * Page_Count + i];

                //将零件架上被点击的零件克隆一份，作为后续被安装的零件
                gameobj = Instantiate(node.gameObject, node.gameObject.transform, true);
                //克隆的零件的名称等于零件架上的物体名称
                gameobj.name = node.name;
                //克隆的零件父物体都是RuntimeObject/Nodes，方便统一管理
                gameobj.transform.parent = GameObject.Find("RuntimeObject/Nodes").transform;
                //克隆的物体大小等于零件架上物体缩放之前的大小，这个值在放入零件架之前被记录过
                gameobj.transform.localScale = node.LocalScale;

                //将零件架上的物体再克隆一份，作为后续提示位置
                gameOb = Instantiate(node.gameObject, node.gameObject.transform, true);
                //提示零件名称需要特殊处理一下
                gameOb.name = node.name + "_Hide";
                //提示零件的父物体也放入RuntimeObject/Nodes，方便统一管理
                gameOb.transform.parent = GameObject.Find("RuntimeObject/Nodes").transform;
                //提示零件的位置，等于零件最终被安装的位置，这样才能起到提示作用
                gameOb.transform.position = node.EndPos;
                //替换提示零件的材质，让提示零件显示透明蓝色
                gameOb.GetComponent<MeshRenderer>().sharedMaterial = GlobalVar.HideLightMate;
                //提示零件的大小，等于零件架上的零件缩放之前的大小
                gameOb.transform.localScale = node.LocalScale;
                //添加拖拽脚本
                gameOb.AddComponent<NodeManager>();

                StartCoroutine(OnMovesIEnumerator(gameobj, gameobj.transform.position));


                break;
            }
        }
    }

    /// <summary>
    /// 零件飞下零件架的协程动画
    /// </summary>
    /// <param name="_GameObject">要飞下零件架的零件物体</param>
    /// <param name="statPos">飞下来的起点位置</param>
    /// <returns></returns>
    IEnumerator OnMovesIEnumerator(GameObject _GameObject, Vector3 statPos)
    {
        float f = 0;   //定义一个参数，记录零件飞的时间
        while (true)
        {
            //如果飞的时间小于1s
            if (f <= 1)
            {
                //零件当前位置，等于起点和终点之间的一个插值，这个插值取决与零件飞的时间和零件飞行的总时间（总时间默认为1s）
                _GameObject.transform.position = Vector3.Lerp(statPos, new Vector3(2.25f, -0.5f, 6), f);
                f += Time.deltaTime;    //零件飞的时间每一帧自加
            }
            else
            {
                //如果飞行时间大于1s，那么结束
                break;
            }
            yield return new WaitForSeconds(0);
        }
    }
}