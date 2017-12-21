using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 扫描零件，往零件集合中添加模型
/// </summary>
public class AddPartsManager : MonoBehaviour
{
    //声明父物体，（也就是LCD1）
    private AssembleController RootPartGameObject;

    //父物体的transform组件
    private Transform RootTrans;

    /// <summary>
    /// UI零件分页界面
    /// </summary>
    private UIPartsPage _UIPartsPage;

    /// <summary>
    /// UI零件分类界面
    /// </summary>
    private UIPartsPanelClass _UIPartsPanelClass;

    // Use this for initialization
    void Start()
    {
        //给父物体赋值
        RootPartGameObject = FindObjectOfType<AssembleController>();
        //给父物体的transform组件赋值
        RootTrans = RootPartGameObject.transform;

        //初始化零件集合
        if (null == NodesCommon.Instance)
        {
            //实例化零件公共类
            var _NodesCommon = new GameObject("NodesCommon", typeof(NodesCommon));
            //实例化的时候，会生成一个空物体，为该空物体指定一个父物体，方便查找和统一管理，不要随意摆放，影响unity的控制面板的管理
            _NodesCommon.transform.parent = gameObject.transform;
        }

        if (null != RootTrans)
        {
            //定义一个node变量，临时存储node
            Node node;
            foreach (Transform child in RootTrans)           //对LCD1做循环，循环获取LCD1机器模型下面零件
            {
                //判断这些零件模型是否挂载了Node脚本，有Node脚本的才认为是真实零件能匹配，不是空物体，并且没有被隐藏的模型
                if (null != child.GetComponent<Node>() && true == child.gameObject.active)
                {
                    //获取零件的node脚本
                    node = child.gameObject.GetComponent<Node>();
                    //将零件大小存入Node中，也可以理解为替Node中的LocalScale赋值
                    node.LocalScale = child.localScale;
                    //记下零件的安装位置
                    node.EndPos = child.transform.position;
                    //记录零件名称
                    node.PartName = child.transform.name;

                    #region  Test      
                    if (node.PartName.Contains("Cube"))
                    {
                        node.Type = "立方体";
                    }
                    else if (node.PartName.Contains("Sphere"))
                    {
                        node.Type = "球体";
                    }
                    else if (node.PartName.Contains("Sphere"))
                    {
                        node.Type = "胶囊体";
                    }
                    else if (node.PartName.Contains("Cylinder"))
                    {
                        node.Type = "圆柱体";
                    }
                    else
                    {
                        node.Type = "其他";
                    }
                    #endregion

                    //将这个物体的Node添加到集合
                    NodesCommon.Instance.AddNodeToList(node);
                }
            }
        }

        //定义一个放缩值，默认为1，不放缩
        float ScalingNum = 1;
        //将零件集合循环
        for (int i = 0; i < NodesCommon.Instance.GetNodeList().Count; i++)
        {
            //如果零件有MeshFilter组件，那么就进行放缩
            if (null != NodesCommon.Instance.GetNodeList()[i].GetComponent<MeshFilter>())
            {
                //获取放缩比例
                ScalingNum = NodesCommon.Instance.GetNodeList()[i].Scaling();
                //进行零件放缩
                NodesCommon.Instance.GetNodeList()[i].gameObject.transform.localScale /= ScalingNum;
            }
        }

        //获取UI零件分页界面的脚本
        _UIPartsPage = GameObject.Find("Canvas/BG/PartsUI/PartsPanel").GetComponent<UIPartsPage>();
        //获取UI零件分类脚本
        _UIPartsPanelClass = GameObject.Find("Canvas/BG/PartsUI/PartsClassPanel").GetComponent<UIPartsPanelClass>();

        //对UI分页界面脚本进行初始化，实际上就是控制UI在扫描零件之后初始化，防止UI界面没有数据，空实现，回报错或者界面没东西
        _UIPartsPage.Init();
        //对UI分类界面脚本进行初始化，实际上就是控制UI在扫描零件之后初始化，防止UI界面没有数据，空实现，回报错或者界面没东西
        _UIPartsPanelClass.Init();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
