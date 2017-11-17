using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 存放项目资源对象，包含脚本，物体，材质，后续UI优化以后，将只保留材质贴图等固定资源，实现模块化，将不在获取物体，只获取绝对位置的脚本和物体
/// </summary>
public class GlobalVar : MonoBehaviour
{
    //材质，提示零件材质
    public static Material HideLightMate = Resources.Load<Material>("Test2");
}
