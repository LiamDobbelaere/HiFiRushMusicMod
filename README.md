# Readme

Note: this is still a heavy WIP, but I wanted to hopefully already provide something semi-usable.

1. Install [Python 3.12.0](https://www.python.org/ftp/python/3.12.0/python-3.12.0-amd64.exe)
2. Install [VLC x64](https://get.videolan.org/vlc/3.0.21/win64/vlc-3.0.21-win64.exe)
3. Run `python -m venv hfrmm` inside of the HiFiRushMusicMod directory
3. Run `./hfrmm/Scripts/activate`
4. Inside this virtual environment: Execute `pip install -r requirements.txt`
5. Still inside the environment: Run `python main.py`
6. A window should appear with the game view, this mod relies on the **rhythm assist** feature being enabled in the game 
7. Everything is currently calibrated to 1920x1080

# Sound files
1. The app expects .ogg files in the format `title.bpm.rank`, for a song called `test`, the files would be:
- `test.135.s.ogg`
    - Plays when the user's rank is S
- `test.135.lower.ogg`
    - Plays when the user's rank is not S
2. Multiple tracks of the same BPM aren't supported yet, program expects only one of each BPM