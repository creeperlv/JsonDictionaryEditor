using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JsonDictionaryEditor.Views;

namespace JsonDictionaryEditor.Controls;

public partial class KVPairEditor : UserControl
{
    MainView? mainView = null;
    public KVPairEditor()
    {
        InitializeComponent();
    }
    public void SetParent(MainView mv)
    {
        this.mainView = mv;
    }
    public void SetKV(string Key, string Value)
    {
        KeyBox.Text = Key;
        ValueBox.Text = Value;
    }
    public (string?, string?) GetKVPair() => (KeyBox.Text, ValueBox.Text);

    private void RemoveBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        mainView?.RemoveItem(this);
    }
}