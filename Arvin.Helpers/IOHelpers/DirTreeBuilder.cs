using AL.DataStruct;
using Arvin.Extensions;
using Arvin.LogHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime;
using System.Text;
using System.Xml.Linq;

namespace Arvin.Helpers.IOHelpers
{
    public class DirTreeBuilder : TreeBuider<DirectoryInfo, FileInfo>
    {
        public DirTreeBuilder(string rootPath, bool isPrint = true) : base(new DirectoryInfo(rootPath), isPrint)
        {

        }
        public DirTreeBuilder(DirectoryInfo root, bool isPrint = false) : base(root, isPrint)
        {
        }

        protected override ChildNodeList<DirectoryInfo, FileInfo> GetChildNodeList(DirectoryInfo node)
        {
            // 获取当前目录下的所有文件和子目录
            DirectoryInfo[] subDirs = node.GetDirectories();
            FileInfo[] files = node.GetFiles();
            if (subDirs.Length == 0 && files.Length == 0)
                return null;
            ChildNodeList<DirectoryInfo, FileInfo> childNodeList = new ChildNodeList<DirectoryInfo, FileInfo>();
            childNodeList.NodeList = new List<DirectoryInfo>(subDirs);
            childNodeList.LeafList = new List<FileInfo>(files);
            return childNodeList;
        }
        StringBuilder sb = new StringBuilder();
        protected override void HandleNode(TreeNode<DirectoryInfo, FileInfo> node)
        {
            //base.Print(node);
            //根据node实现树节点的打印
            if (node == null)
                return;
            if (node.Level == 0)
            {
                sb.AppendLine(node.Node.Name);
                return;
            }
            sb.AppendLine(GetTreeNodeString(node));
        }

        string GetName(TreeNode<DirectoryInfo, FileInfo> node)
        {
            string name = node.Node?.Name;
            if (name.IsNullOrWhiteSpace())
                name = node.Leaf.Name;
            return name;
        }
        string GetTreeNodeString(TreeNode<DirectoryInfo, FileInfo> node, string connectStr = "├─", string name = "")
        {
            if (name.IsNullOrWhiteSpace())
                name = GetName(node);
            if (name == "00.jpg")
            {

            }
            string res = $"{connectStr} {name}";
            if (node.Level - 1 == 0)
                return res;
            int spaceFreeTimes = node.Level - 1;
            while (spaceFreeTimes-- > 0)
            {
                res = $"│{new string(' ', 2)}{res}";
            }
            return res;
        }
        public string GetTreeString()
        {
            base.BuildTree();
            string res = sb.ToString();
            //LastNodeList统一替换为
            if (this.NodeLasts.Count > 0)
            {
                foreach (var node in this.NodeLasts)
                {
                    var name = GetName(node);
                    string old = GetTreeNodeString(node, name: name);
                    string newStr = GetTreeNodeString(node, "└─", name: name);
                    res = res.Replace(old, newStr);
                }
            }
            ALog.WriteLine(res);
            return res;
        }
    }
}
