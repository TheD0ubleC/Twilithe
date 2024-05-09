using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HandyControl.Controls;
using TencentCloud.Common.Profile;
using TencentCloud.Common;
using TencentCloud.Tmt.V20180321.Models;
using TencentCloud.Tmt.V20180321;
using MessageBox = HandyControl.Controls.MessageBox;

namespace Twilithe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        static async Task<string> TranslationAsync(string sourceText, string source, string target)
        {
            var secretId = "YOURID";
            var secretKey = "YOURKEY";

            Credential cred = new Credential
            {
                SecretId = secretId,
                SecretKey = secretKey
            };
            ClientProfile clientProfile = new ClientProfile();
            HttpProfile httpProfile = new HttpProfile();
            httpProfile.Endpoint = "tmt.ap-shanghai.tencentcloudapi.com";
            clientProfile.HttpProfile = httpProfile;

            TmtClient client = new TmtClient(cred, "ap-shanghai", clientProfile);
            TextTranslateRequest req = new TextTranslateRequest
            {
                SourceText = sourceText,
                Source = source,
                Target = target,
                ProjectId = 0
            };

            try
            {
                TextTranslateResponse resp = await client.TextTranslate(req);
                return resp.TargetText;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private async void TranslationButton_Click(object sender, RoutedEventArgs e)
        {
            TranslationButton.IsEnabled = false;
            TranslationButton.Content = "翻译中...";
            var translator = new TwilitheTranslator();
            if (Source_English.IsChecked == true && Target_Twilithe.IsChecked == true)
            {
                OutputTextbox.Text = translator.TranslateToTwilithe(InputTextbox.Text.ToLower());
            }
            else if (Source_Chinese.IsChecked == true && Target_Twilithe.IsChecked == true)
            {
                string c2eResult = await TranslationAsync(InputTextbox.Text, "zh", "en");

                OutputTextbox.Text = translator.TranslateToTwilithe(c2eResult.ToLower());
            }
            else if (Source_Twilithe.IsChecked == true && Target_English.IsChecked == true)
            {

                OutputTextbox.Text = translator.TranslateToEnglish(InputTextbox.Text.ToLower());
            }
            else if (Source_Chinese.IsChecked == true && Target_Twilithe.IsChecked == true)
            {
                string c2eResult = await TranslationAsync(InputTextbox.Text, "zh", "en");

                OutputTextbox.Text = translator.TranslateToTwilithe(c2eResult.ToLower());
            }
            else if (Source_Chinese.IsChecked == true && Target_English.IsChecked == true)
            {
                string e2cResult = await TranslationAsync(InputTextbox.Text, "zh", "en");
                OutputTextbox.Text = e2cResult;
            }
            else if (Source_English.IsChecked == true && Target_Chinese.IsChecked == true)
            {
                string e2cResult = await TranslationAsync(InputTextbox.Text, "zh", "en");
                OutputTextbox.Text = e2cResult;
            }
            else if (Source_Twilithe.IsChecked == true && Target_Chinese.IsChecked == true)
            {
                string temp;
                temp = translator.TranslateToEnglish(InputTextbox.Text.ToLower());
                string e2cResult = await TranslationAsync(temp, "en", "zh");
                OutputTextbox.Text = e2cResult;
            }
            else { OutputTextbox.Text = InputTextbox.Text; }
            TranslationButton.IsEnabled = true;
            TranslationButton.Content = "翻译";
        }
    }

    class TwilitheTranslator
    {
        private Dictionary<string, string> letterMap = new Dictionary<string, string>()
    {
        {"you", "ior"}, /*{"i", "ri"},*/ {"he", "tur"}, {"she", "sir"}, // 你 我 他 她
        {"it", "nes"}, {"walk", "plato"}, {"see", "minso"}, {"speak", "ralto"}, // 它 走 看 说
        {"find", "sanko"}, {"use", "lunso"}, {"create", "tarko"}, {"protect", "murno"}, // 找 使用 创建 保护
        {"love", "kinal"}, {"learn", "laiso"}, {"travel", "torno"}, {"help", "sulo"}, // 爱 学习 旅行 帮助
        {"understand", "kramo"}, {"fail", "malto"}, {"laugh", "lilso"}, {"cry", "krenso"}, // 理解 失败 笑 哭
        {"jump", "rakto"}, {"tree", "tarno"}, {"water", "nulsa"}, {"light", "lorin"}, // 跳 树 水 光
        {"friend", "pulra"}, {"home", "ronka"}, {"strength", "krint"}, {"magic", "mrsan"}, // 朋友 家 力量 魔法
        {"secret", "rusil"}, {"star", "sorin"}, {"mountain", "klito"}, {"flower", "kresno"}, // 秘密 星星 山 花
        {"rain", "miro"}, {"music", "plura"}, {"sun", "solo"}, {"dream", "nemro"}, // 雨 音乐 太阳 梦
        {"river", "dulsa"}, {"beautiful", "luna"}, {"powerful", "krin"}, {"old", "nalt"}, // 河流 美丽 强大 旧的
        {"new", "nilt"}, {"happy", "liso"}, {"sad", "murn"}, {"calm", "sano"}, // 新的 快乐 悲伤 平静
        {"angry", "rask"}, {"up", "topa"}, {"down", "nopa"}, {"left", "lepa"}, // 愤怒 上 下 左
        {"right", "repa"}, {"inside", "insa"}, {"outside", "outsa"}, {"one", "uno"}, // 右 内 外 一
        {"two", "duo"}, {"three", "trio"}, {"many", "muno"}, {"few", "puno"}, // 二 三 多 少
        {"all", "alla"}, {"now", "nonsa"}, {"past", "pasta"}, {"future", "futura"}, // 全部 现在 过去 未来
        {"day", "dina"}, {"night", "nocta"}, {"forever", "eversa"}, // 日 夜 永远
        {"twilithe","lorinoka" },//暮言
        {"question", "ralna"}, {"answer", "kansa"}, {"hope", "mino"}, {"fear", "tuno"}, // 问题 答案 希望 恐惧
        {"peace", "lino"}, {"conflict", "tano"}, {"wisdom", "sarno"}, {"foolish", "lukno"}, // 和平 冲突 智慧 愚蠢
        {"cold", "nino"}, {"warm", "muno"}, {"lightweight", "lito"}, {"heavy", "kanto"}, // 冷 温暖 轻 重
        {"fast", "rino"}, {"slow", "luno"}, {"early", "timo"}, {"late", "nomo"}, // 快 慢 早 晚
        {"young", "yuno"}, {"bright", "lirno"}, {"dark", "nirno"}, // 年轻 老 明亮 黑暗
        {"hard", "karno"}, {"soft", "suno"}, {"sharp", "tino"}, {"dull", "duno"}, // 硬 软 尖 钝
        {"rich", "runo"}, {"poor", "puno"}, {"strong", "struno"}, {"weak", "wekno"}, // 富 穷 强 弱
        {"high", "hino"}, {"low", "lowno"}, {"near", "nino"}, {"far", "farno"}, // 高 低 近 远

        {"wind", "sura"}, {"stone", "kurna"}, {"forest", "lurno"}, {"mount", "tarna"}, // 风 石 头 森林 山
        {"stream", "nura"}, {"cloud", "runa"}, {"starlight", "sorina"}, {"moonlight", "lurina"}, // 小溪 云 星光 月光
        {"thunder", "torna"}, {"lightning", "lina"}, {"ice", "nina"}, {"snow", "snura"}, // 雷 闪电 冰 雪
        {"fog", "funa"}, {"raindrop", "miroa"}, {"sunshine", "solra"}, {"dew", "dera"}, // 雾 雨滴 阳光 露水
        {"leaf", "lira"}, {"branch", "brana"}, {"root", "rota"}, {"bud", "buda"}, // 叶 枝 根 芽
        {"bloom", "bloma"}, {"petal", "petra"}, {"seed", "sera"}, {"soil", "sola"}, // 开花 花瓣 种子 土壤
        {"a", "a"}, {"b", "pa"}, {"c", "km"}, {"d", "ta"},
        {"e", "e"}, {"f", "pu"}, {"g", "kn"}, {"h", "lm"},
        {"i", "ri"}, {"j", "ia"}, {"k", "k"}, {"l", "l"},
        {"m", "m"}, {"n", " m"}, {"o", "o"}, {"p", "p"},
        {"q", "ku"}, {"r", "r"}, {"s", "s"}, {"t", "t"},
        {"u", "u"}, {"v", "um"}, {"w", "ue"}, {"x", "ks"},
        {"y", "im"}, {"z", "sa"},
        {"ch", "ra"}, {"sh", "sme"}, {"th", "sm"},
    };

        private Dictionary<string, string> reverseLetterMap = new Dictionary<string, string>();

        public TwilitheTranslator()
        {
            // 构建反向映射，用于暮言到英文的转换
            foreach (var item in letterMap)
            {
                if (!reverseLetterMap.ContainsKey(item.Value))
                {
                    reverseLetterMap.Add(item.Value, item.Key);
                }
            }
        }

        public string TranslateToTwilithe(string sentence)
        {
            return TranslateSentence(sentence.ToLower(), letterMap);
        }

        public string TranslateToEnglish(string sentence)
        {
            return TranslateSentence(sentence, reverseLetterMap);
        }

        private string TranslateSentence(string sentence, Dictionary<string, string> map)
        {
            var words = Regex.Split(sentence, @"(\s+|[^a-zA-Z]+)").Where(s => s != string.Empty);
            var translatedWords = words.Select(word => TranslateWord(word, map));
            return string.Join("", translatedWords);
        }

        private string TranslateWord(string word, Dictionary<string, string> map)
        {
            var result = new StringBuilder();
            int startIndex = 0;
            while (startIndex < word.Length)
            {
                string segmentFound = null;
                // 从当前位置开始，尝试匹配最长的字母组合
                for (int len = word.Length - startIndex; len > 0; len--)
                {
                    string segment = word.Substring(startIndex, len);
                    if (map.ContainsKey(segment))
                    {
                        segmentFound = segment;
                        result.Append(map[segment]);
                        startIndex += len;
                        break;
                    }
                }
                if (segmentFound == null)
                {
                    result.Append(word[startIndex]);
                    startIndex++;
                }
            }
            return result.ToString();
        }

    }
}
