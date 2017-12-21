using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour, IManipulationHandler
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    /// 拖拽过程触发回调方法
    /// </summary>
    /// <param name="eventData"></param>
    void IManipulationHandler.OnManipulationUpdated(ManipulationEventData eventData)
    {
        //如果被拖拽的物体存在拖拽脚本（防止该脚本已经被删除，那么不满足安装要求）
        if (null != gameObject.GetComponent<HandDraggable>())
        {
            //如果被拖拽的物体绑定有Node脚本，防止该物体不是零件
            if (null != gameObject.GetComponent<Node>())
            {
                //被拖拽物体距离安装位置小于0.08m，表示可以吸附了，也就是说进入吸附区域了
                if (0.08f >= Vector3.Distance(gameObject.transform.position, gameObject.GetComponent<Node>().EndPos))
                {
                    //删除物体上拖拽的脚本，即不允许在被拖拽
                    Destroy(gameObject.GetComponent<HandDraggable>());
                    //吸附到安装位置
                    gameObject.transform.position = gameObject.GetComponent<Node>().EndPos;
                    //删除被安装物体上面的碰撞体，防止后续物体发生碰撞
                    Destroy(gameObject.GetComponent<BoxCollider>());
                }
            }
        }
    }


    void IManipulationHandler.OnManipulationCanceled(ManipulationEventData eventData)
    {

    }

    void IManipulationHandler.OnManipulationCompleted(ManipulationEventData eventData)
    {

    }

    void IManipulationHandler.OnManipulationStarted(ManipulationEventData eventData)
    {

    }
}
