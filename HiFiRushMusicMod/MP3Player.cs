using NAudio.Wave;

public class MP3Player
{
    private string fileName;
    private IWavePlayer waveOutDevice;
    private AudioFileReader audioFileReader;

    public MP3Player(string fileName)
    {
        this.fileName = fileName;
        waveOutDevice = new WaveOutEvent(); // or WaveOut(), depending on your needs
        audioFileReader = new AudioFileReader(fileName);
        waveOutDevice.Init(audioFileReader);

        waveOutDevice.PlaybackStopped += WaveOutDevice_PlaybackStopped;
    }

    private void WaveOutDevice_PlaybackStopped(object? sender, StoppedEventArgs e)
    {
        audioFileReader = new AudioFileReader(this.fileName);
        waveOutDevice.Init(audioFileReader);

        waveOutDevice.Play();
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