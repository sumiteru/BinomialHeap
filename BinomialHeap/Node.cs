using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinomialHeap
{
    class Node
    {
        public int Key { get; set; }
        public Node Parent { get; set; }
        public Node Child { get; set; }
        public Node Sibling { get; set; }

        public int Degree { get; set; }

        public Node(int value)
        {
            this.Key = value;
        }

        public Node(string value)
        {
            int key;
            if (Int32.TryParse(value, out key))
                this.Key = key;
        }

        public override string ToString()
        {
            return string.Format("-->Klucz: {0}   Stopień: {1}\n", this.Key, this.Degree);
        }

        public string displayNodeTree()
        {
            string result = this.ToString();

            result += displayChild(1);

            return result;
        }

        private string displayChild(int space)
        {
            string result = "";
            Node child = this.Child;

            if(child != null)
            {
                string tab = "";
                for (int i = 0; i < space; i++)
                    tab += "---";

                result += string.Format("{0}{1}", tab, child);
                result += child.displayChild(space + 1);
                result += child.displaySibling(space);
            }

            return result;
        }

        private string displaySibling(int space)
        {
            string result = "";
            Node sibling = this.Sibling;

            if(sibling != null)
            {
                string tab = "";
                for (int i = 0; i < space; i++)
                    tab += "---";

                result += string.Format("{0}{1}", tab, sibling);
                result += sibling.displayChild(space + 1);
                result += sibling.displaySibling(space);
            }

            return result;
        }

        public Node findNodeByValue(int key)
        {
            Node sibling = this;
            while(sibling != null)
            {
                if (sibling.Key == key)
                    return sibling;

                Node child = sibling.Child;
                while(child != null)
                {
                    Node result = child.findNodeByValue(key);
                    if (result != null)
                        return result;

                    child = child.Child;
                }

                sibling = sibling.Sibling;
            }

            return null;
        }

        public void reduceKey()
        {
            this.Key = int.MinValue;
            this.heapify();
        }

        private void heapify()
        {
            Node current = this;
            while(current.Parent != null)
            {
                if(current.Key < current.Parent.Key)
                {
                    int tmp = current.Key;
                    current.Key = current.Parent.Key;
                    current.Parent.Key = tmp;
                }

                current = current.Parent;
            }
        }
    }
}
