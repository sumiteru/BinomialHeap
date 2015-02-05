using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinomialHeap
{
    class BinomialHeap
    {
        public Node Head { get; set; }

        public BinomialHeap()
        {
            Head = null;
        }

        public bool insert(int value)
        {
            if(value > 0)
            {
                Node tmpNode = new Node(value);
                if(this.Head == null)
                {
                    this.Head = tmpNode;
                }
                else
                {
                    BinomialHeap tmpHeap = new BinomialHeap();
                    tmpHeap.Head = tmpNode;
                    this.union(tmpHeap);
                }

                return true;
            }

            return false;
        }

        public bool insert(string value)
        {
            int key;
            if (Int32.TryParse(value, out key))
            {
                insert(key);
                return true;
            }

            return false;
        }

        public void union(BinomialHeap heapToUnite)
        {
            this.merge(heapToUnite);
            if (this.Head == null)
                return;

            Node previousRoot = null;
            Node currentRoot = this.Head;
            Node nextRoot = currentRoot.Sibling;

            while(nextRoot != null)
            {
                /*
                 *  Przechodzimy dalej jeśli:
                 *  - stopień aktualnego korzenia jest mniejszy od kolejnego (są w prawidłowej kolejności) 
                 *  - aktualny korzeń jest pierwszym z trzech kolejnych korzeni o tym samym stopniu
                */
                if((currentRoot.Degree != nextRoot.Degree) || (nextRoot.Sibling != null && currentRoot.Degree == nextRoot.Sibling.Degree))
                {
                    previousRoot = currentRoot;
                    currentRoot = nextRoot;
                }
                else
                {
                    /*
                     * W przeciwnym wypadku:
                     *  - jeśli stopnie aktualnego i następnego korzenia są równe:
                     *      - jeśli klucz aktualnego korzenia(a) jest MNIEJSZY od klucza następnego korzenia(b) -  b staje się dzieckiem a
                     *      - jeśli klucz aktualnego korzenia jest WIĘKSZY od klucza następnego korzenia - a staje się dzieckiem b
                     */
                    if(currentRoot.Key <= nextRoot.Key)
                    {
                        currentRoot.Sibling = nextRoot.Sibling;
                        link(nextRoot, currentRoot);
                    }
                    else
                    {
                        if (previousRoot == null)
                            this.Head = nextRoot;
                        else
                            previousRoot.Sibling = nextRoot;

                        link(currentRoot, nextRoot);
                        currentRoot = nextRoot;
                    }
                }
                nextRoot = currentRoot.Sibling;
            }
        }

        private void merge(BinomialHeap heapToMerge)
        {
            Node head1 = this.Head;
            Node head2 = heapToMerge.Head;
            Node headOfHeap = null;
            Node tailOfHeap = null;

            if (head1.Degree < head2.Degree)
            {
                headOfHeap = head1;
                head1 = head1.Sibling;
            }
            else
            {
                headOfHeap = head2;
                head2 = head2.Sibling;
            }
   
            tailOfHeap = headOfHeap;

            while(head1 != null && head2 != null)
            {
                if(head1.Degree < head2.Degree)
                {
                    tailOfHeap.Sibling = head1;
                    head1 = head1.Sibling;
                }
                else
                {
                    tailOfHeap.Sibling = head2;
                    head2 = head2.Sibling;
                }

                tailOfHeap = tailOfHeap.Sibling;
            }

            if (head1 != null)
                tailOfHeap.Sibling = head1;
            else if (head2 != null)
                tailOfHeap.Sibling = head2;
            else
                tailOfHeap.Sibling = null;

            this.Head = headOfHeap;
        }

        private void link(Node node1, Node node2)
        {
            node1.Parent = node2;
            node1.Sibling = node2.Child;
            node2.Child = node1;
            node2.Degree++;
        }

        public bool deleteNode(int key)
        {
            if (this.Head == null)
                return false;

            Node node = findNodeByValue(key);
            if (node == null)
                return false;

            node.reduceKey();
            deleteMinimum();
            return true;
        }

        public bool deleteNode(string text)
        {
            int key;         
            if(Int32.TryParse(text, out key))
            {
                this.deleteNode(key);
                return true;
            }

            return false;
        }

        public void deleteMinimum()
        {
            BinomialHeap resultHeap = new BinomialHeap();
            Node minRoot = findMinimum();
            removeFromRoots(minRoot);
            
            Node newRoot = minRoot.Child;
            removeFromChildren(minRoot);

            resultHeap.Head = newRoot;
            if (resultHeap.Head == null)
                return;
            
            newRoot = newRoot.Sibling;
            resultHeap.Head.Sibling = null;
            while (newRoot != null)
            {
                BinomialHeap tmp = new BinomialHeap();

                tmp.Head = newRoot;
                newRoot = newRoot.Sibling;
                tmp.Head.Sibling = null;

                resultHeap.union(tmp);
            }

            if (this.Head == null)
                this.Head = resultHeap.Head;
            else
                this.union(resultHeap);
        }

        private void removeFromChildren(Node node)
        {
            Node child = node.Child;

            while(child != null)
            {
                child.Parent = null;
                child = child.Sibling;
            }
                
        }

        private void removeFromRoots(Node node)
        {
            if(node == this.Head)
            {
                this.Head = node.Sibling;
                return;
            }

            Node root = this.Head;
            while(root != null)
            {
                if(root.Sibling == node)
                {
                    root.Sibling = node.Sibling;
                }

                root = root.Sibling;
            }
        }

        private Node findMinimum()
        {
            Node minNode = null;
            int min = int.MaxValue;

            Node iterator = this.Head;
            while (iterator != null)
            {
                if (iterator.Key < min)
                {
                    min = iterator.Key;
                    minNode = iterator;
                }

                iterator = iterator.Sibling;
            }

            return minNode;
        }

        public Node findNodeByValue(int value)
        {
            return this.Head.findNodeByValue(value);
        }

        public Node findNodeByValue(string value)
        {
            int key;
            if(Int32.TryParse(value, out key))
            {
                return this.Head.findNodeByValue(key);
            }
            return null;
        }

        public bool readHeapFromFile(string path)
        {
            BinomialHeap heap = new BinomialHeap();

            string[] lines;
            try
            {
                lines = File.ReadAllLines(path);

                string[] roots = lines[0].Split(';');
                heap.insert(roots[0]);
                Node root = heap.Head;
                for (int i = 1; i < roots.Length; i++)
                {
                    root.Sibling = new Node(roots[i]);
                    root = root.Sibling;
                }

                for (int i = 1; i < lines.Length; i += 2)
                {
                    string key = lines[i].TrimEnd('#');
                    Node parent = heap.findNodeByValue(key);

                    string[] children = lines[i + 1].Split(';');
                    Node firstChild = new Node(children[0]);
                    firstChild.Parent = parent;
                    parent.Child = firstChild;
                    parent.Degree = children.Length;
                    Node sibling = parent.Child;
                    for (int j = 1; j < children.Length; j++)
                    {
                        Node child = new Node(children[j]);
                        child.Parent = parent;
                        sibling.Sibling = child;
                        sibling = sibling.Sibling;
                    }
                }

                this.Head = heap.Head;
                Console.WriteLine("Plik został wczytany.");
            }
            catch(FileNotFoundException)
            {
                Console.WriteLine("\nPlik o podanej nazwie nie istnieje.");
                return false;
            }
            catch(ArgumentException)
            {
                Console.WriteLine("\nŚcieżka nie może być pusta.");
                return false;
            }
            catch(Exception e)
            {
                Console.WriteLine("Wystąpił błąd");
                Console.WriteLine(e);
            }
            finally
            {
                Console.WriteLine("Naciśnij dowolny klawisz aby kontynuować...");
                Console.ReadKey();
            }

            return true;
        }

        public void saveHeapToFile(string path)
        {
            try
            {
                string heapString = this.ToString();
                File.WriteAllText(path, heapString);
                Console.WriteLine("Plik został pomyślnie zapisany.");
            }
            catch(Exception)
            {
                Console.WriteLine("Wystąpił błąd.");
                return;
            }
            finally
            {
                Console.WriteLine("Naciśnij dowolny klawisz aby kontynuować...");
                Console.ReadKey();
            }
        }

        public override string ToString()
        {
            string result = "-->Head\n";
            Node tmp = this.Head;

            while(tmp != null)
            {
                result += tmp.displayNodeTree();
                tmp = tmp.Sibling;
            }

            return result;
        }
    }
}
