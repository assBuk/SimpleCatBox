using SimpleCatBox.Properties;
using System;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Resources;

class Program
{


    [STAThread]
    static void Main(string[] args)
    {

        bool startMinimized = args.Contains("/StartMinimized");

        if (startMinimized)
        {

            // Запускаем ваше приложение в свёрнутом режиме
            Application.Run(new MainForm());
            return;
        }

        var url = "https://catbox.moe/user/api.php";
        var filePath = args[0]; // Путь к файлу, который нужно загрузить
        var userHash = "1234567zxczxczxc"; // Ваш userhash

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "curl",
                Arguments = $"-X POST {url} -H \"Content-Type: multipart/form-data\" -F \"reqtype=fileupload\" -F \"userhash={userHash}\" -F \"fileToUpload=@{filePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };



        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        // Поиск ссылки в ответе
        var match = Regex.Match(output, @"https://files\.catbox\.moe/[a-zA-Z0-9_-]+(\.[a-zA-Z0-9]+)");
        var link = match.Success ? match.Value : null;

        if (!string.IsNullOrEmpty(link))
        {
            Clipboard.SetText(link);
            //PlaySoundAsync(Resources.f6);
            MessageBox.Show($"Ссылка скопирована в буфер обмена\n{link}", "Загружено.", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            //PlaySoundAsync(Resources.f6);
            MessageBox.Show("Ссылка не найдена.\nЧто-то пошло не так.", "Ошибка загрузки.", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void PlaySoundAsync(byte[] sound)
    {
        Task.Run(() =>
        {
            using (MemoryStream fileOut = new MemoryStream(sound))
            using (GZipStream gz = new GZipStream(fileOut, CompressionMode.Decompress))
                new SoundPlayer(gz).Play();
        });
    }
}
public class MainForm : Form
{
    public MainForm()
    {
        this.WindowState = FormWindowState.Minimized;
    }
}
