using System;
using System.Collections.Generic;

class FileRecord
{
    public int Timestamp { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
    public FileRecord Left { get; set; }
    public FileRecord Right { get; set; }

    public FileRecord(int timestamp, string name, string content)
    {
        Timestamp = timestamp;
        Name = name;
        Content = content;
        Left = null;
        Right = null;
    }
}

class FileBST
{
    private FileRecord root;

    public FileBST()
    {
        root = null;
    }

    public void Insert(int timestamp, string name, string content)
    {
        root = Insert(root, timestamp, name, content);
    }

    private FileRecord Insert(FileRecord node, int timestamp, string name, string content)
    {
        if (node == null)
            return new FileRecord(timestamp, name, content);

        if (timestamp < node.Timestamp)
            node.Left = Insert(node.Left, timestamp, name, content);
        else
            node.Right = Insert(node.Right, timestamp, name, content);

        return node;
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
}

class Program
{
    static void Main()
    {
        FileBST fileManager = new FileBST();
        fileManager.Insert(20230101, "Report1", "Annual report 2023");
        fileManager.Insert(20230615, "Invoice2", "June Invoice");
        fileManager.Insert(20230810, "Contract3", "Contract details");

        // Tìm kiếm theo tên
        FileRecord record = fileManager.SearchByName("Invoice2");
        if (record != null)
            Console.WriteLine($"Found: {record.Name}, Content: {record.Content}");
        else
            Console.WriteLine("File not found");

        // Tìm kiếm theo khoảng thời gian
        List<FileRecord> results = fileManager.SearchByTimeRange(20230101, 20230701);
        foreach (var r in results)
            Console.WriteLine($"Timestamp: {r.Timestamp}, Name: {r.Name}, Content: {r.Content}");
    }
}
