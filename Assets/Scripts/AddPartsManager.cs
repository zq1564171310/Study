using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 扫描零件，往零件集合中添加模型
/// </summary>
public class AddPartsManager : MonoBehaviour
{
    //声明一个集合，存储所有零件模型
    private List<Node> nodeList = new List<Node>();

    //声明父物体，（也就是LCD1）
    private AssembleController RootPartGameObject;

    //父物体的transform组件
    private Transform RootTrans;

    //声明一个框，用来显示测试数据
    private Text Test;

    // Use this for initialization
    void Start()
    {
        //给父物体赋值
        RootPartGameObject = FindObjectOfType<AssembleController>();
        //给父物体的transform组件赋值
        RootTrans = RootPartGameObject.transform;
        //给这个测试框初始化
        Test = GameObject.Find("Canvas/Test").GetComponent<Text>();

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
                    //给物体添加碰撞器，Hololens选中的物体必须带有Collider
                    node.gameObject.AddComponent<BoxCollider>();
                    //将这个物体的Node添加到集合
                    nodeList.Add(node);
                }
            }
        }
        //让测试框打印程序一共扫描到LDC1有多少模型
        Test.text = "LCD1机器一共有" + nodeList.Count + "个模型";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
