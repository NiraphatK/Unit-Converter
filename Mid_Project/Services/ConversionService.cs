using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

public class ConversionHistory
{
    public double InputValue { get; set; }
    public string FromUnit { get; set; }
    public string ToUnit { get; set; }
    public double ResultValue { get; set; }
    public DateTime Timestamp { get; set; }
}

public class ConversionService
{
    private string GetFilePath()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "conversion_history.json");
    }

    public void SaveHistoryToFile(ConversionHistory history)
    {
        string path = GetFilePath();
        List<ConversionHistory> historyList = new List<ConversionHistory>();

        // ถ้ามีไฟล์แล้วให้โหลดประวัติที่มีอยู่
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            historyList = JsonConvert.DeserializeObject<List<ConversionHistory>>(json) ?? new List<ConversionHistory>();
        }

        // เพิ่มประวัติใหม่
        historyList.Add(history);

        // แปลงเป็น JSON และบันทึกลงไฟล์
        var jsonToWrite = JsonConvert.SerializeObject(historyList, Newtonsoft.Json.Formatting.Indented); // Use Newtonsoft.Json.Formatting.Indented
        File.WriteAllText(path, jsonToWrite);
    }

    public List<ConversionHistory> LoadHistoryFromFile()
    {
        string path = GetFilePath();

        // ถ้ามีไฟล์ประวัติการแปลงอยู่แล้ว
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<ConversionHistory>>(json) ?? new List<ConversionHistory>();
        }

        return new List<ConversionHistory>();
    }
}
