using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DependencyGraph {

    protected Node root;        //指向所有关系路径的根结点

    //构造方法，初始化根节点
    public DependencyGraph() {

        root = new Node(-1, "root", "root", "root", "root");

    }

    //得到根节点
    public Node GetRootNode() {

        return root;

    }

    //添加节点到root节点下
    public void AddResource(Node node) {

        root.AddChildNode(node);

    }

    /*
     * 添加一组依赖关系
     * 改变节点之间的指向关系
     */
    public bool AddDependent(Node parent, Node child) {

        if (parent == null) 
        {
            Debug.Log("父节点为空！");
            return false;
        }
        if (child == null) 
        {
            Debug.Log("子节点为空！");
            return false;
        }       

        parent.AddChildNode(child);

        if ((child.GetParent())[0].GetId() == -1) 
        {
            root.RemoveChildNode(child);
        }

        return true;
    }

    //创建asset bundle build map
    public AssetBundleBuildMap CreateAssetBundleBuildMap() {

        AssetBundleBuildMap buildMap = new AssetBundleBuildMap();

        Partition(root, buildMap);
        
        return buildMap;

    }

    //深度优先遍历依赖关系图，找到链状依赖资源，和依赖关系相同的资源，进行 asset bundle 打包划分
    private void Partition(Node node, AssetBundleBuildMap buildMap) {

        if (node == null)
        {
            Debug.Log("依赖关系图为空！");
            return;
        }

        if (node.isPass == false)
        {
            if (node.GetAssetBundleName().Equals("#"))
            {
                if (node.GetId() != -1)
                {
                    node.SetAssetBundleName(node.GetMd5());
                    buildMap.AddAsset(node, node.GetAssetBundleName());
                }
            }           

            if (node.GetOutDegree() == 1)
            {
                if (IsLink(node.GetChildren()[0]))
                {
                    node.GetChildren()[0].SetAssetBundleName(node.GetAssetBundleName());                       
                    buildMap.AddAsset(node.GetChildren()[0], node.GetChildren()[0].GetAssetBundleName());
                }
            }

            else if (node.GetOutDegree() > 1)
            {
                if (node.GetId() == -1)
                {
                    for (int i = 0; i < node.GetOutDegree(); i++)
                    {
                        if (node.GetChildren()[i].GetOutDegree() == 0)
                        {
                            node.GetChildren()[i].SetAssetBundleName(node.GetChildren()[i].GetMd5());
                            buildMap.AddAsset(node.GetChildren()[i], node.GetChildren()[i].GetAssetBundleName());
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < node.GetOutDegree(); i++)
                    {
                        for (int j = i + 1; j < node.GetOutDegree(); j++)
                        {
                            if (IsSameDependency(node.GetChildren()[i], node.GetChildren()[j]))
                            {
                                if (node.GetChildren()[i].GetAssetBundleName().Equals("#"))
                                {
                                    node.GetChildren()[i].SetAssetBundleName(node.GetChildren()[i].GetMd5());
                                    buildMap.AddAsset(node.GetChildren()[i], node.GetChildren()[i].GetAssetBundleName());
                                }
                                node.GetChildren()[j].SetAssetBundleName(node.GetChildren()[i].GetAssetBundleName());
                                buildMap.AddAsset(node.GetChildren()[j], node.GetChildren()[j].GetAssetBundleName());
                            }
                        }
                    }
                }
            }

            node.isPass = true;
        }

        for (int i = 0; i < node.GetOutDegree(); i++)
        {
            Partition(node.GetChildren()[i], buildMap);
        }

    }

    public void GetDependencyAssetsId(Node node, List<int> idList) {

        idList.Add(node.GetId());

        if (node.isPass == false)
            node.isPass = true;
        else
            return;

        for (int i = 0; i < node.GetOutDegree(); i++) 
        {          
            GetDependencyAssetsId(node.GetChildren()[i], idList);
        }
    }

    //判断是否是链状
    private bool IsLink(Node node) {

        if (node.GetInDegree() == 1)
            return true;
        return false;

    }

    /*
     * 判断两个资源节点依赖关系是否相同
     * 父列表和子列表全都一样的两个节点算作相同依赖关系的节点
     */
    private bool IsSameDependency(Node node1, Node node2){

        if (node1.GetInDegree() != node2.GetInDegree() || node1.GetOutDegree() != node2.GetOutDegree())
            return false;

        for (int i = 0; i < node1.GetInDegree(); i++)
        {
            if (node1.GetParent()[i].GetId() != node2.GetParent()[i].GetId())
                return false;
        }

        for (int i = 0; i < node1.GetOutDegree(); i++)
        {
            if (node1.GetChildren()[i].GetId() != node2.GetChildren()[i].GetId())
                return false;
        }

        return true;
    }
}
