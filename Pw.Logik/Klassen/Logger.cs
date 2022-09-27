using System;
using System.IO;

public class Logger
{
    public LogLevel AktlogLevel;
    public enum LogLevel { Debug = 9, Info = 0, Warning = 1, Error = 2 };
    private const string _dateiName = "Tresorlogger.txt", _dateiNameKopie = "TresorLogger_old.txt";
    private string _logPfad, _logPfadKopie;
    private object _locker = new object();
    long _maxSize = 1020;
	public Logger(LogLevel neuerlevel = LogLevel.Error)
	{
        AktlogLevel = neuerlevel;
        _logPfad = $@"{Logik.Pw.Logik.Properties.Settings.Default.PfadZielOrdner}\{_dateiName}";
        _logPfadKopie = $@"{Logik.Pw.Logik.Properties.Settings.Default.PfadZielOrdner}\{_dateiNameKopie}";
        if (!File.Exists(_logPfad))
            File.Create(_logPfad);
	}

    public void SchreibeEintrag(string message, LogLevel level)
    {
        if (message == null || message.Length == 0)
            return;
        if (level <= AktlogLevel)
        {
            if (FileMaximumErreicht())
                KopiereLogFile();
            SchreibeFile(message, level);
        }
    }

    private bool FileMaximumErreicht()
    {
        long aktsize;
        if ((aktsize = new FileInfo(_logPfad).Length) > _maxSize)
            return true;
        else
            return false;
    }

    private void KopiereLogFile()
    {
        File.Copy(_logPfad, _logPfadKopie, true);
    }

    private void SchreibeFile(string message, LogLevel level)
    {
        lock (_locker)
        {
            FileStream mystream = new FileStream(_logPfad, FileMode.Append);

            using(StreamWriter writer = new StreamWriter(mystream))
            {
                writer.WriteLine($"{DateTime.Now}: {level} -> {message}");
            }
        }
    }

}
