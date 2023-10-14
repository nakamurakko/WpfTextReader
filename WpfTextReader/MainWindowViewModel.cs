using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;

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

        // 1. SpeechSynthesizer.Speak() は、読み上げ終わるまで画面がフリーズする。
        //using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
        //{
        //    synthesizer.SelectVoice(this.SelectedVoice.VoiceInfo.Name);
        //    synthesizer.Speak(this.TargetText);
        //}

        // 2. SpeechSynthesizer.SpeakAsync() を使えばフリーズしない。
        // ただし、
        // 「SpeechSynthesizer への最後の参照を解放する前に、必ず Dispose を呼び出してください。 そうしないと、ガベージ コレクターが SpeechSynthesizer オブジェクトの Finalize メソッドを呼び出すまで、使用されているリソースは解放されません。」
        // とあるので、メソッド内で SpeechSynthesizer をインスタンス化する場合は
        // SpeechSynthesizer.SpeakAsync() は使用しない方が良い。
        // <https://learn.microsoft.com/ja-jp/dotnet/api/system.speech.synthesis.speechsynthesizer?view=netframework-4.8.1&viewFallbackFrom=net-6.0#:~:text=SpeechSynthesizer%E3%81%B8%E3%81%AE%E6%9C%80%E5%BE%8C%E3%81%AE%E5%8F%82%E7%85%A7%E3%82%92%E8%A7%A3%E6%94%BE%E3%81%99%E3%82%8B%E5%89%8D%E3%81%AB%E3%80%81%E5%BF%85%E3%81%9ADispose%E3%82%92%E5%91%BC%E3%81%B3%E5%87%BA%E3%81%97%E3%81%A6%E3%81%8F%E3%81%A0%E3%81%95%E3%81%84%E3%80%82%20%E3%81%9D%E3%81%86%E3%81%97%E3%81%AA%E3%81%84%E3%81%A8%E3%80%81%E3%82%AC%E3%83%99%E3%83%BC%E3%82%B8%20%E3%82%B3%E3%83%AC%E3%82%AF%E3%82%BF%E3%83%BC%E3%81%8C%20SpeechSynthesizer%20%E3%82%AA%E3%83%96%E3%82%B8%E3%82%A7%E3%82%AF%E3%83%88%E3%81%AE%20Finalize%20%E3%83%A1%E3%82%BD%E3%83%83%E3%83%89%E3%82%92%E5%91%BC%E3%81%B3%E5%87%BA%E3%81%99%E3%81%BE%E3%81%A7%E3%80%81%E4%BD%BF%E7%94%A8%E3%81%95%E3%82%8C%E3%81%A6%E3%81%84%E3%82%8B%E3%83%AA%E3%82%BD%E3%83%BC%E3%82%B9%E3%81%AF%E8%A7%A3%E6%94%BE%E3%81%95%E3%82%8C%E3%81%BE%E3%81%9B%E3%82%93%E3%80%82>
        //SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        //synthesizer.SelectVoice(this.SelectedVoice.VoiceInfo.Name);
        //synthesizer.SpeakAsync(this.TargetText);

        // 3. SpeechSynthesizer.Speak() を Task で実行するのが現実的な気がする。
        Task.Run(() =>
        {
            using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
            {
                synthesizer.SelectVoice(this.SelectedVoice.VoiceInfo.Name);
                synthesizer.Speak(this.TargetText);
            }
        });
    }
}
