import numpy as np
import cv2
import time
import math
import os
import pygame
from mss import mss

class CrossfadePlayer:
    def __init__(self):
        """Initialize the player and pygame mixer."""
        pygame.mixer.init()
        self.current_track = None
        self.current_channel = None

    def play_track(self, file, loop=False, start_pos=0):
        """Play an audio track, optionally starting at a specific position."""
        sound = pygame.mixer.Sound(file)
        channel = sound.play(-1 if loop else 0)  # Loop if required
        if start_pos > 0:
            channel.set_volume(0)  # Start muted if starting at a specific position
            pygame.time.delay(int(start_pos * 1000))  # Seek to the position
        self.current_track = sound
        self.current_channel = channel
        return sound, channel

    def get_position(self):
        """Get the current playback position in seconds."""
        if self.current_channel and self.current_channel.get_busy():
            return pygame.mixer.music.get_pos() / 1000  # Position in seconds
        return 0

    def crossfade_tracks(self, new_track_file, fade_duration=3, copy_position=False):
        """Crossfade to a new track with an option to copy playback position."""
        position = 0
        if copy_position and self.current_channel and self.current_channel.get_busy():
            position = self.get_position()  # Get the current position of the active track

        # Load the new track
        new_track = pygame.mixer.Sound(new_track_file)
        new_channel = new_track.play(-1)  # Loop the new track
        if position > 0:
            new_channel.set_volume(0)  # Start muted if copying position
            pygame.time.delay(int(position * 1000))  # Skip to the position

        # Perform the crossfade
        steps = 20  # Number of steps for the fade
        for i in range(steps + 1):
            volume_a = max(0, 1 - i / steps)
            volume_b = min(1, i / steps)
            if self.current_channel:
                self.current_channel.set_volume(volume_a)
            new_channel.set_volume(volume_b)
            time.sleep(fade_duration / steps)

        # Stop the current track after the crossfade
        if self.current_track:
            self.current_track.stop()

        # Update the current track and channel
        self.current_track = new_track
        self.current_channel = new_channel

    def stop(self):
        """Stop playback of the current track."""
        if self.current_track:
            self.current_track.stop()
        self.current_track = None
        self.current_channel = None

class FastCapture:
    def __init__(self, bounding_box):
        self.bounding_box = bounding_box
        self.sct = mss()

    def capture(self):
        sct_img = self.sct.grab(self.bounding_box)
        return np.array(sct_img)

    def get_pixel(self, x, y):
        sct_img = self.sct.grab(self.bounding_box)
        return (np.array(sct_img), sct_img.pixel(x, y))

class RollingAverage:
    def __init__(self, size):
        self.size = size
        self.values = []

    def add(self, value):
        self.values.append(value)
        if len(self.values) > self.size:
            self.values.pop(0)

    def get(self):
        if len(self.values) == 0:
            return 0
        
        return sum(self.values) / len(self.values)

def lerp (a, b, t):
    return a + (b - a) * t

def get_rank_from_color(color):
    # use euclidean distance to determine rank
    # Math.Sqrt(Math.Pow(sRankColor.R - color.R, 2) + Math.Pow(sRankColor.G - color.G, 2) + Math.Pow(sRankColor.B - color.B, 2));
    sRankDistance = math.sqrt((sRankColor[0] - color[0]) ** 2 + (sRankColor[1] - color[1]) ** 2 + (sRankColor[2] - color[2]) ** 2)
    aRankDistance = math.sqrt((aRankColor[0] - color[0]) ** 2 + (aRankColor[1] - color[1]) ** 2 + (aRankColor[2] - color[2]) ** 2)
    tolerance = 10.0

    if sRankDistance < tolerance:
        return 'S'
    elif aRankDistance < tolerance:
        return 'A'
    else:
        return 'LOWER'

def find_matching_track(bpm, rank):
    for track in mp3_files:
        if abs(track['bpm'] - bpm) < 8 and (ignore_rank_for_mp3_selection or track['rank'] == rank.lower()):
            return track
    return

# s rank color from hex #EF1375
sRankColor = (0xEF, 0x13, 0x75)
aRankColor = (0xE4, 0x30, 0x0B)

current_track = None

#bpmCatCapture = FastCapture({'top': 945, 'left': 930, 'width': 64, 'height': 64 })
# BPM rush mode:
bpmCatCapture = FastCapture({'top': 820, 'left': 930, 'width': 64, 'height': 64 })

rankCapture = FastCapture({'top': 185, 'left': 1710, 'width': 1, 'height': 1 })

bpmCatOffset = (32, 8)

bpmRollingAverage = RollingAverage(15)

cat_r_last = 0
beat_dir_last = -1

time_last_beat = time.time()
time_last_color_change = time.time()

beat_indication_color = 0

cv2.namedWindow('output', cv2.WINDOW_NORMAL)    # Create window with freedom of dimensions
cv2.resizeWindow('output', 256, 256)

crossfade_player = CrossfadePlayer()

# get all mp3 files in mp3 folder
mp3_files = []
for file in os.listdir('mp3'):
    if file.endswith('.mp3'):
        # split on . and take the first part
        (name, bpm, rank, _) = file.split('.')

        print(f'Found {name} with bpm {bpm} and rank {rank.lower()}')
        mp3_files.append({'name': name, 'bpm': int(bpm), 'rank': rank.lower(), 'file': file })

print(mp3_files)

ignore_rank_for_mp3_selection = False
debugging = False

beat_log_index = 0
matching_track_stability = 0
last_matching_track = ''

while True:
    (sct_img, pixel) = bpmCatCapture.get_pixel(*bpmCatOffset)
    cat_r = pixel[0]
    (_, rank_color) = rankCapture.get_pixel(0, 0)
    rank = get_rank_from_color(rank_color)
    
    # if last change was too fast, ignore this change
    color_changed_too_fast = (time.time() - time_last_color_change) < 0.1

    green_and_blue_are_zero = pixel[1] == 0 and pixel[2] == 0

    if (not color_changed_too_fast) and (cat_r != cat_r_last) and green_and_blue_are_zero:
        time_last_color_change = time.time()
        beat_dir = -1
        
        if cat_r > cat_r_last:
            beat_dir = 1
        else:
            beat_dir = 0

        if beat_dir != -1 and beat_dir != beat_dir_last:
            beat_dir_last = beat_dir

            if beat_dir == 1:
                beat_indication_color = 255

                bpm = 60 / (time.time() - time_last_beat)
                bpmRollingAverage.add(bpm)

                avg_bpm = bpmRollingAverage.get()

                matching_track = find_matching_track(avg_bpm, rank)

                if matching_track != None and matching_track == last_matching_track:
                    matching_track_stability += 1
                else:
                    matching_track_stability = 0
                    last_matching_track = ''

                with open('beat.log', 'a') as f:
                    f.write(f'{beat_log_index} {pixel[0]} {pixel[1]} {pixel[2]} {avg_bpm} {rank} {matching_track} {matching_track_stability}\n')
                    beat_log_index += 1

                if matching_track != None and (current_track == None or current_track['file'] != matching_track['file']) and matching_track_stability > 3:
                    should_copy_pos = current_track != None and current_track['name'] == matching_track['name']
                    current_track = matching_track

                    crossfade_player.crossfade_tracks(f'mp3/{current_track["file"]}', 3, should_copy_pos)

                    #copied_pos = mixer.music.get_pos()

                    #mixer.music.load(f'mp3/{current_track['file']}')

                last_matching_track = matching_track
                time_last_beat = time.time()
    cat_r_last = cat_r                

    beat_indication_color = lerp(beat_indication_color, 0, 0.1)

    if debugging:
        # edit the image so the pixel at 32, 8 is green
        sct_img[bpmCatOffset[1]][bpmCatOffset[0]] = [0, 255, 0, 255]

        for y in range(8):
            for x in range(64):
                sct_img[bpmCatOffset[1] + 1 + y][x] = [beat_indication_color, 0, 0, 255]

        cv2.putText(sct_img, str(bpmRollingAverage.get()), (10, 35), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (255, 255, 255), 1, cv2.LINE_AA)
        cv2.putText(sct_img, rank, (10, 50), cv2.FONT_HERSHEY_SIMPLEX, 0.5, rank_color[::-1], 1, cv2.LINE_AA)
    cv2.imshow('output', np.array(sct_img))

    if (cv2.waitKey(1) & 0xFF) == ord('q'):
        cv2.destroyAllWindows()
        break
