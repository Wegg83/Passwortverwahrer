using System;
using System.IO;

public class Logger
{
    public LogLevel AktlogLevel;
    public enum LogLevel { Debug = 9, Info = 2, Warning = 1, Error = 0 };
    private const string _dateiName = "Tresorlogger.txt", _dateiNameKopie = "TresorLogger_old.txt";
    private string _logPfad, _logPfadKopie;
    private static readonly object _locker = new object();
    long _maxSize = 5000000;
	public Logger(LogLevel neuerlevel = LogLevel.Error)
	{
        AktlogLevel = neuerlevel;
        _logPfad = $@"{Logik.Pw.Logik.Properties.Settings.Default.PfadZielOrdner}{_dateiName}";
        _logPfadKopie = $@"{Logik.Pw.Logik.Properties.Settings.Default.PfadZielOrdner}{_dateiNameKopie}";
        if (!File.Exists(_logPfad))
            KreiereLogFile(_logPfad);
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
            using(StreamWriter writer = new StreamWriter(new FileStream(_logPfad, FileMode.Append)))
            {
                writer.WriteLine($"{DateTime.Now}: {level} -> {message}");
                writer.Close();
            }
        }
    }

    private void KreiereLogFile(string Pfad)
    {
        string ordner = Path.GetDirectoryName(Pfad);
        if (!Directory.Exists(ordner))
        {
            if (Directory.Exists(Environment.CurrentDirectory))
            {
                _logPfad = $@"{Environment.CurrentDirectory}{_dateiName}";
                _logPfadKopie = $@"{Environment.CurrentDirectory}{_dateiNameKopie}";
            }
            else
            {
                Environment.Exit(0);
            }
        }

        var machzu = File.Create(_logPfad);
        machzu.Close();
        SchreibeEintrag("NeuesLogFile erstellt", LogLevel.Info);
        return;
    }
}
