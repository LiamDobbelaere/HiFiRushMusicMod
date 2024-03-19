using NAudio.Wave;

public class MP3Player
{
    private readonly IWavePlayer waveOutDevice;
    private readonly AudioFileReader audioFileReader;

    public MP3Player(string fileName)
    {
        waveOutDevice = new WaveOutEvent(); // or WaveOut(), depending on your needs
        audioFileReader = new AudioFileReader(fileName);
        waveOutDevice.Init(audioFileReader);
    }

    public void Play()
    {
        waveOutDevice.Play();
    }

    public void Stop()
    {
        waveOutDevice.Stop();
    }

    public void Dispose()
    {
        waveOutDevice.Dispose();
        audioFileReader.Dispose();
    }
}