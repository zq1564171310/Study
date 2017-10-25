using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 零件类，后续会添加零件属性和参数以及零件相关的基础方法等
/// </summary>
public class Node : MonoBehaviour
{
    public string PartName;                    //零件的名称

    // Use this for initialization
    void Start()
    {
        PartName = gameObject.name;                   //给零件的名称初始化，让零件的名称等于该脚本所指向的零件模型的名称
    }

    // Update is called once per frame
    void Update()
    {

    }

}
