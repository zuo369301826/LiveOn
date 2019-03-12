using System.Collections.Generic;
using UnityEngine;

public class Node {
 
    public bool isPass;                 //判断是否遍历过          

    private int id;                     //编号
    private string name;                //资源名
    private string assetPath;           //路径    
    private string md5;                 //文件的md5码
    private string metaPath;            //meta文件名
    private string metaMd5;             //meta文件md5码
    private string assetBundleName;     //所属assetbundle包名

    protected int inDegree;             //入度
    protected int outDegree;            //出度
    protected List<Node> parent;        //父节点列表
    protected List<Node> children;      //子节点列表

    /*
     * 构造方法
     */
    public Node(int id = -2, string assetPath = "null", string md5 = "null", string metaPath = "null", string metaMd5 = "null", string assetBundleName = "#") {

        this.id = id;
        string[] paths = assetPath.Split('/');
        this.name = paths[paths.Length - 1];
        this.assetPath = assetPath;
        this.md5 = md5;
        this.metaPath = metaPath;
        this.metaMd5 = metaMd5;
        this.assetBundleName = assetBundleName;
        inDegree = 0;
        outDegree = 0;
        parent = new List<Node>();
        children = new List<Node>();
        isPass = false;

    }

    public void SetAssetBundleName(string name) {

        assetBundleName = name;

    }

    //得到资源编号
    public int GetId() {

        return id;

    }

    //得到路径
    public string GetPath() {

        return assetPath;

    }

    //得到资源名
    public string GetName() {

        return name;

    }

    //得到md5码
    public string GetMd5() {

        return md5;

    }

    //得到meta文件的名字
    public string GetMetaName() {

        return metaPath;

    }

    //得到meta文件的md5码
    public string GetMetaMd5() {

        return metaMd5;

    }

    //得到asset bundle包名
    public string GetAssetBundleName() {

        return assetBundleName;

    }

    //得到入度
    public int GetInDegree() {

        return inDegree;

    }

    //得到出度
    public int GetOutDegree() { 

        return outDegree;

    }

    //得到子节点列表
    public List<Node> GetChildren() {

        return children;

    }

    //得到父节点列表
    public List<Node> GetParent() {

        return parent;

    }

    /*
     * 添加子节点
     * 成功返回true，失败返回false
     */
    public bool AddChildNode(Node node) {

        if (node == null)
        {
            Debug.Log("要添加的子节点为空！");
            return false;
        }

        int i;

        for (i = 0; i < outDegree; i++)
        {
            if (children[i].GetId() == node.GetId())
            {
                Debug.Log("要添加的子节点已存在！");
                return false;
            }
        }

        //找到插入位置
        for (i = 0; i < outDegree; i++)
        {
            if (node.GetId() < children[i].GetId())
                break;
        }

        if (node.AddParentNode(this))           //子节点添加自己作为父节点
        {
            children.Insert(i, node);
            outDegree++;
            return true;
        }else
        {
            Debug.Log("添加父节点失败！");
            return false;
        }

    }

    /*
     * 删除子节点
     * 成功返回true，失败返回false
     */
    public bool RemoveChildNode(Node node) {

        if (node == null)
        {
            Debug.Log("要删除的子节点为空！");
            return false;
        }

        if (children.Remove(node))
        {
            if (node.RemoveParentNode(this))             //同时在子节点的父节点列表中删除自己
            {
                outDegree--;
                return true;
            }
            else
            {
                Debug.Log("删除子节点失败！，因为删除节点操作是双向的，所以子节点中的操作出现了失败，导致了这个方法的失败。");
                return false;
            }
        }
        else
        {
            Debug.Log("删除子节点失败！");
            return false;
        }

    }

    /*
     * 添加父节点
     * 成功返回true，失败返回false
     */

    private bool AddParentNode(Node node)
    {

        if (node == null)
        {
            Debug.Log("要添加的父节点为空");
            return false;
        }

        int i;

        for (i = 0; i < inDegree; i++)
        {
            if (parent[i].GetId() == node.GetId())
            {
                Debug.Log("要添加的父节点已存在");
                return false;
            }
        }

        for (i = 0; i < inDegree; i++)
        {
            if (node.GetId() < parent[i].GetId())
                break;
        }

        parent.Insert(i, node);
        inDegree++;
        return true;

    }

    /*
     * 删除父节点
     * 成功返回true，失败返回false
     */
    private bool RemoveParentNode(Node node) {

        if (node == null)
        {
            Debug.Log("要删除的父节点为空");
            return false;
        }
        if (parent.Remove(node))
        {
            inDegree--;
            return true;
        }
        else
        {
            Debug.Log("删除父节点失败！");
            return false;
        }

    }
}