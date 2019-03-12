using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceList {

    private List<Node> resourceList;

    //构造方法
    public ResourceList() {

        resourceList = new List<Node>();

    }

    //添加一个资源
    public bool AddResource(Node node) {

        if(node == null)
        {
            Debug.Log("要添加的节点为空");
            return false;
        }

        resourceList.Add(node);
        return true;
    }

    //得到资源列表
    public List<Node> GetResourceList() {

        return resourceList;
    }

    /*
     * 通过路径查找资源
     * 找到返回资源对象，未找到返回null
     */
    public Node FindResourceByPath(string path) {

        for (int i = 0; i < resourceList.Count; i++)
        {
            if (resourceList[i].GetPath().Equals(path))
            {
                return resourceList[i];
            }
        }

        Debug.Log("资源列表中没有要查找的资源" + path);
        return null;
    }

    //还原所有节点为没遍历过
    public void RecoverIsPass() {

        for (int i = 0; i < resourceList.Count; i++)
        {
            resourceList[i].isPass = false;
        }
    }
}