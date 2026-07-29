
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using JsonDictionaryEditor.Controls;
using Newtonsoft.Json;

namespace JsonDictionaryEditor.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void ExitMenuItem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Environment.Exit(0);
    }

    private void AddNewPairButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        KVPairEditor item = new();
        item.SetParent(this);
        EditorPanel.Children.Add(item);
    }
    string? current_file_path = null;
    void Save()
    {
        if (current_file_path is null) return;
        Dictionary<string, string> data = new Dictionary<string, string>();
        foreach (var item in EditorPanel.Children)
        {
            if (item is KVPairEditor editor)
            {
                var pair = editor.GetKVPair();
                if (pair.Item1 is not null && pair.Item2 is not null)
                    data[pair.Item1] = pair.Item2;
            }
        }
        File.WriteAllText(current_file_path, JsonConvert.SerializeObject(data));
    }
    public void RemoveItem(KVPairEditor editor)
    {
        if (this.EditorPanel.Children.Contains(editor))
        {
            EditorPanel.Children.Remove(editor);
        }
    }
    async Task SaveAs()
    {
        var toplevel = TopLevel.GetTopLevel(this);
        if (toplevel is null) return;
        var file = await toplevel.StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions());
        if (file is null) return;
        var path = file.TryGetLocalPath();
        current_file_path = path;
        Save();
    }
    private async void SaveAsBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await SaveAs();
    }

    private async void SaveBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (current_file_path is null) await SaveAs();
        else Save();
    }

    private async void OpenMenuItem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var toplevel = TopLevel.GetTopLevel(this);
        if (toplevel is null) return;
        var file = await toplevel.StorageProvider.OpenFilePickerAsync(new()
        {
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType>() {
                new(".*"),
                new(".txt"),
                new(".json"),
                new(".lang"),
                new(".manifest"),
                }
        });
        if (file is not null)
        {
            var f = file.FirstOrDefault();
            if (f is null) return;
            var p = f.TryGetLocalPath();
            this.current_file_path = p;
            EditorPanel.Children.Clear();
            if (current_file_path is not null)
            {
                Dictionary<string, string>? data = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(current_file_path));
                if (data is not null)
                {
                    foreach (var item in data)
                    {

                        KVPairEditor editor = new();
                        editor.SetParent(this);
                        editor.SetKV(item.Key, item.Value);
                        EditorPanel.Children.Add(editor);
                    }
                }

            }
        }
    }

    private void NewMenuItem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        current_file_path = null;
        this.EditorPanel.Children.Clear();
    }
}