using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Speech.Synthesis;

namespace WpfTextReader;

/// <summary>
/// MainWindow 用 ViewModel。
/// </summary>
internal sealed partial class MainWindowViewModel : ObservableObject
{
    /// <summary>
    /// ウィンドウタイトル。
    /// </summary>
    [ObservableProperty]
    private string _title = "WpfTextReader";

    /// <summary>
    /// 音声一覧。
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<InstalledVoice> _voices = new ObservableCollection<InstalledVoice>();

    /// <summary>
    /// 選択した音声。
    /// </summary>
    [ObservableProperty]
    private InstalledVoice _selectedVoice;

    /// <summary>
    /// 読み上げたいテキスト。
    /// </summary>
    [ObservableProperty]
    private string _targetText = "";

    /// <summary>
    /// コンストラクター。
    /// </summary>
    public MainWindowViewModel()
    {
        using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
        {
            foreach (InstalledVoice voice in synthesizer.GetInstalledVoices())
            {
                this.Voices.Add(voice);
            }
        }

        this.SelectedVoice = this.Voices.FirstOrDefault();
    }

    /// <summary>
    /// テキストを読み上げる。
    /// </summary>
    [RelayCommand]
    private void ReadText()
    {
        if ((this.SelectedVoice == null) || string.IsNullOrWhiteSpace(this.TargetText))
        {
            return;
        }

        using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
        {
            synthesizer.SelectVoice(this.SelectedVoice.VoiceInfo.Name);
            synthesizer.Speak(this.TargetText);
        }
    }
}
