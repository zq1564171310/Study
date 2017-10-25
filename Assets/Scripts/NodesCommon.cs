using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 零件公共类，处理零件数据
/// </summary>
public class NodesCommon : Singleton<NodesCommon>
{
    /// <summary>
    /// 声明一个集合变量，作为储存零件的集合
    /// </summary>
    private List<Node> NodeList = new List<Node>();

    /// <summary>
    /// 往零件集合中添加零件
    /// </summary>
    /// <param name="node"></param>
    public void AddNodeToList(Node node)
    {
        NodeList.Add(node);
    }

    /// <summary>
    /// 获取零件模型
    /// </summary>
    /// <returns></returns>
    public List<Node> GetNodeList()
    {
        return NodeList;
    }

    // Use this for initialization
    void Start()
    {

    }
}
