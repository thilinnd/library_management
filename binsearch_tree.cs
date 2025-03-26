using System;
using System.Collections.Generic;
using System.IO;


class FileRecord
{
    public string BookID { get; set; }
    public int Timestamp { get; set; }
    public string Name { get; set; }
    public string Author { get; set; }
    public string Category { get; set; } 
    public FileRecord Left { get; set; }
    public FileRecord Right { get; set; }

    public FileRecord(string bookID, int timestamp, string name, string author, string category)
    {
        BookID = bookID; 
        Timestamp = timestamp;
        Name = name;
        Author = author;
        Category = category;
        Left = null;
        Right = null;
    }
}

class FileBST
{
    private FileRecord root;

    public void UpdateOrInsert(string bookID, int timestamp, string name, string author, string category)
    {
        root = UpdateOrInsert(root, bookID, timestamp, name, author, category);
    }

    private FileRecord UpdateOrInsert(FileRecord node, string bookID, int timestamp, string name, string author, string category)
    {
        if (node == null)
            return new FileRecord(bookID, timestamp, name, author, category);

        if (bookID == node.BookID)
        {
            // Ghi đè dữ liệu sách
            node.Timestamp = timestamp;
            node.Name = name;
            node.Author = author;
            node.Category = category;
        }
        else if (timestamp < node.Timestamp)
            node.Left = UpdateOrInsert(node.Left, bookID, timestamp, name, author, category);
        else
            node.Right = UpdateOrInsert(node.Right, bookID, timestamp, name, author, category);

        return node;
    }
    public void LoadFromCSV(string filePath)
    {
        try
        {
            using (StreamReader sr = new StreamReader(filePath, System.Text.Encoding.UTF8))
            {
                string line;
                bool firstLine = true; // Skip the header line
                while ((line = sr.ReadLine()) != null)
                {
                    if (firstLine)
                    {
                        firstLine = false;
                        continue;
                    }

                    string[] parts = line.Split(',');
                    if (parts.Length != 5) 
                    {
                        Console.WriteLine($"Dòng không hợp lệ: {line}");
                        continue;
                    }

                    string bookID = parts[0];

                    if (!int.TryParse(parts[1], out int timestamp))
                    {
                        Console.WriteLine($"❌ Không thể chuyển đổi BookID hoặc Timestamp: {parts[0]}, {parts[1]}");
                        continue;
                    }

                    string name = parts[2];
                    string author = parts[3];
                    string category = parts[4];
                    
                    UpdateOrInsert(bookID, timestamp, name, author, category);
                }
            }
            Console.WriteLine("📥 Dữ liệu đã được tải thành công từ CSV.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi khi đọc file CSV: {ex.Message}\n{ex.StackTrace}");
        }
    }
    public void PrintInOrder()
    {
        PrintInOrder(root);
    }

    private void PrintInOrder(FileRecord node)
    {
        if (node == null) return;

        PrintInOrder(node.Left);
        Console.WriteLine($"BookID: {node.BookID}, Timestamp: {node.Timestamp}, Name: {node.Name}, Author: {node.Author}, Category: {node.Category}");
        PrintInOrder(node.Right);
    }
    public FileRecord SearchByName(string name)
    {
        return SearchByName(root, name);
    }

    private FileRecord SearchByName(FileRecord node, string name)
    {
        if (node == null)
            return null;
        if (node.Name == name)
            return node;

        FileRecord leftResult = SearchByName(node.Left, name);
        if (leftResult != null)
            return leftResult;
        
        return SearchByName(node.Right, name);
    }

    public List<FileRecord> SearchByTimeRange(int start, int end)
    {
        List<FileRecord> results = new List<FileRecord>();
        SearchByTimeRange(root, start, end, results);
        return results;
    }

    private void SearchByTimeRange(FileRecord node, int start, int end, List<FileRecord> results)
    {
        if (node == null)
            return;

        if (start <= node.Timestamp && node.Timestamp <= end)
            results.Add(node);
        
        if (start < node.Timestamp)
            SearchByTimeRange(node.Left, start, end, results);
        
        if (node.Timestamp < end)
            SearchByTimeRange(node.Right, start, end, results);
    }

    public void PrintBooksByAuthor(string author)
    {
        List<FileRecord> results = new List<FileRecord>();
        SearchByAuthor(root, author, results);

        if (results.Count == 0)
        {
            Console.WriteLine($"Không tìm thấy sách của tác giả: {author}");
            return;
        }

        Console.WriteLine($"📚 Danh sách sách của tác giả {author}:");
        foreach (var book in results)
        {
            Console.WriteLine($"- {book.Name} (BookID: {book.BookID}, Timestamp: {book.Timestamp}, Category: {book.Category})");
        }
    }

    private void SearchByAuthor(FileRecord node, string author, List<FileRecord> results)
    {
        if (node == null)
            return;

        if (node.Author.Equals(author, StringComparison.OrdinalIgnoreCase))
            results.Add(node);

        SearchByAuthor(node.Left, author, results);
        SearchByAuthor(node.Right, author, results);
    }

    public void PrintBooksByCategory(string category)
    {
        List<FileRecord> results = new List<FileRecord>();
        SearchByCategory(root, category, results);

        if (results.Count == 0)
        {
            Console.WriteLine($"Không tìm thấy sách thuộc thể loại: {category}");
            return;
        }

        Console.WriteLine($"📚 Danh sách sách thuộc thể loại {category}:");
        foreach (var book in results)
        {
            Console.WriteLine($"- {book.Name} (BookID: {book.BookID}, Timestamp: {book.Timestamp}, Author: {book.Author})");
        }
    }

    private void SearchByCategory(FileRecord node, string category, List<FileRecord> results)
    {
        if (node == null)
            return;

        if (node.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            results.Add(node);

        SearchByCategory(node.Left, category, results);
        SearchByCategory(node.Right, category, results);
    }
    
    public FileRecord FindMin()
    {
        return FindMin(root);
    }

    private FileRecord FindMin(FileRecord node)
    {
        while (node?.Left != null)
            node = node.Left;
        return node;
    }

    public FileRecord FindMax()
    {
        return FindMax(root);
    }

    private FileRecord FindMax(FileRecord node)
    {
        while (node?.Right != null)
            node = node.Right;
        return node;
    }

    public void Delete(int timestamp)
    {
        root = Delete(root, timestamp);
    }

    private FileRecord Delete(FileRecord node, int timestamp)
    {
        if (node == null)
            return null;

        if (timestamp < node.Timestamp)
            node.Left = Delete(node.Left, timestamp);
        else if (timestamp > node.Timestamp)
            node.Right = Delete(node.Right, timestamp);
        else
        {
            if (node.Left == null)
                return node.Right;
            else if (node.Right == null)
                return node.Left;

            FileRecord minLargerNode = FindMin(node.Right);
            node.Timestamp = minLargerNode.Timestamp;
            node.Name = minLargerNode.Name;
            node.Author = minLargerNode.Author; 
            node.Right = Delete(node.Right, minLargerNode.Timestamp);
        }
        return node;
    }

    public void DeleteByID(string bookID)
    {
        root = DeleteByID(root, bookID);
    }

    private FileRecord DeleteByID(FileRecord node, string bookID)
    {
        if (node == null)
            return null;

        if (node.BookID == bookID)
        {
            if (node.Left == null)
                return node.Right;
            else if (node.Right == null)
                return node.Left;

            FileRecord minLargerNode = FindMin(node.Right);
            node.BookID = minLargerNode.BookID;
            node.Timestamp = minLargerNode.Timestamp;
            node.Name = minLargerNode.Name;
            node.Author = minLargerNode.Author;
            node.Category = minLargerNode.Category;
            node.Right = DeleteByID(node.Right, minLargerNode.BookID);
        }
        else
        {
            node.Left = DeleteByID(node.Left, bookID);
            node.Right = DeleteByID(node.Right, bookID);
        }

        return node;
    }

    
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        string filePath = "D:\\Tài liệu học tập\\Năm 3\\ctdlgt\\FINAL\\listofbook.csv";
        FileBST fileManager = new FileBST();
        fileManager.LoadFromCSV(filePath);

        // // Tìm kiếm theo tên
        // FileRecord record = fileManager.SearchByName("Invoice2");
        // if (record != null)
        //     Console.WriteLine($"Found: {record.Name}, Author: {record.Author}");
        // else
        //     Console.WriteLine("File not found");

        // // Tìm kiếm theo khoảng thời gian
        // List<FileRecord> results = fileManager.SearchByTimeRange(20230101, 20230701);
        // foreach (var r in results)
        //     Console.WriteLine($"Timestamp: {r.Timestamp}, Name: {r.Name}, Author: {r.Author}");

        // Tìm kiếm theo tác giả
        // // fileManager.PrintBooksByAuthor("James Clear");
        // Tìm kiếm theo thể loại
        // fileManager.PrintBooksByCategory("Programming");
        
        // // print in order
        // fileManager.PrintInOrder();

        // string bookIDToDelete = "N003";
        // fileManager.DeleteByID(bookIDToDelete);
        // Console.WriteLine($"Sách có ID {bookIDToDelete} đã được xóa (nếu tồn tại).");

        // // Kiểm tra lại sau khi xóa
        // fileManager.PrintInOrder();

        // Insert sách 
        fileManager.UpdateOrInsert("N006", 20230615, "Invoice4", "James Clear", "Programming");
        fileManager.UpdateOrInsert("N007", 20230616, "Invoice5", "Robert C. Martin", "Novel");
        // Ghi đè
        fileManager.UpdateOrInsert("N006", 19550617, "Hello World", "James Clear", "Self-help");
        fileManager.PrintInOrder();
       
        // Tìm min/max
        Console.WriteLine("Min timestamp file: " + fileManager.FindMin()?.Name);
        Console.WriteLine("Max timestamp file: " + fileManager.FindMax()?.Name);

        // Xóa hồ sơ
        fileManager.Delete(20230615);
        Console.WriteLine("After deletion, searching for Invoice2: " + (fileManager.SearchByName("Invoice2") == null ? "Not found" : "Found"));
    }
}
