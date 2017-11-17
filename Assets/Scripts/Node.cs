using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 零件类，后续会添加零件属性和参数以及零件相关的基础方法等
/// </summary>
public class Node : MonoBehaviour
{
    public string PartName;                    //零件的名称

    public Vector3 LocalScale;                //记录零件原本大小，因为后续可能会将零件缩放等操作

    public Vector3 EndPos;                   //零件的安装位置

    // Use this for initialization
    void Start()
    {
        PartName = gameObject.name;                   //给零件的名称初始化，让零件的名称等于该脚本所指向的零件模型的名称
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 获取零件的大小尺寸
    /// </summary>
    /// <param name="partModel">要获取大小的零件物体</param>
    /// <returns></returns>
    public Vector3 GetPartModelRealSize()
    {
        //判断零件是否存在MeshFilter，如果不存在，说明物体是父物体，下面还有子物体，那么获取大小换另外方式获取
        if (null != GetComponent<MeshFilter>())
        {
            //获取物体MeshFilter的大小然后乘以缩放比例就是物体的真实大小
            //获取真实的x轴大小
            float xSize = GetComponent<MeshFilter>().mesh.bounds.size.x * transform.localScale.x;
            //获取真实的y轴大小
            float ySize = (float)Math.Round(GetComponent<MeshFilter>().mesh.bounds.size.y * transform.localScale.y, 3);
            //获取真实的z轴大小
            float zSize = GetComponent<MeshFilter>().mesh.bounds.size.z * transform.localScale.z;
            //将获取的xyz放入vector3中
            Vector3 Ver = new Vector3(xSize, ySize, zSize);
            return Ver;
        }
        else   //如果不存在Meshfilter
        {
            //声明一个边界框类
            Bounds totalBounds = new Bounds();
            //获取物体的Renderer类对象
            var renderer = GetComponent<Renderer>();
            //判断物体是否存在Renderer对象
            if (renderer != null)
            {
                //如果物体存在Renderer对象，那么直接获取其边界框
                totalBounds = renderer.bounds;
            }
            //对物体做遍历，遍历父物体下的子物体
            foreach (Transform t in transform)
            {
                //获取子物体的Renderer的对象
                var childRenderer = t.GetComponent<Renderer>();
                //如果Renderer对象不为空
                if (childRenderer != null)
                {
                    //那么获取子物体的边界框对象
                    totalBounds.Encapsulate(childRenderer.bounds);
                }
            }
            //返回边界框对象的大小
            return totalBounds.extents;
        }
    }

    /// <summary>
    /// 获取统一规格需要缩放的大小比例，不同的零件大小不一，但缩放到指定规格，需要获取一个值，来确定放多大，或者缩多小
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public float Scaling()
    {
        //声明一个值，默认为1，也就是不放缩
        float scalingNum = 1;
        //声明一个值，最终放大缩小之后的比例
        Vector3 targeSize = new Vector3(0.15f, 0.15f, 0.15f);
        //获取零件的真实大小
        Vector3 localSize = GetPartModelRealSize();
        //声明一个集合，用来存放零件的大小的x,y,z
        List<float> list = new List<float>();
        //将x,y,z添加到集合中
        list.Add(localSize.x);
        list.Add(localSize.y);
        list.Add(localSize.z);
        //对集合进行排序，由小到大
        list.Sort();
        //如果xyz最大的一个大于目标值，那么缩小
        if (list[2] >= targeSize.x)
        {
            //将计算将这个最大值缩小到目标大小的一个比例
            scalingNum = list[2] / targeSize.x;                    //目前定义的x，y，z是一样大的，先把逻辑简单化，将来在做复杂逻辑
        }
        else     //如果零件的最大的一个轴的大小都小于目标值，那么放大
        {
            //计算放大倍数
            scalingNum = Mathf.Max(0.5f, list[2] / targeSize.x);        // Do not make the parts too big
        }
        //返回放大缩小的比例
        return scalingNum;
    }
}
