import cv2
import os

# Ordner, in dem das Skript liegt
base_dir = os.path.dirname(os.path.abspath(__file__))
output_folder = os.path.join(base_dir, "output")

# Output-Ordner anlegen
os.makedirs(output_folder, exist_ok=True)

# Erstes Video im Ordner finden
video_file = None
for file in os.listdir(base_dir):
    if file.lower().endswith((".mp4", ".avi", ".mov", ".mkv")):
        video_file = os.path.join(base_dir, file)
        break

if not video_file:
    raise FileNotFoundError("Kein Video im Skriptordner gefunden.")

# Video öffnen
cap = cv2.VideoCapture(video_file)
if not cap.isOpened():
    raise RuntimeError(f"Video '{video_file}' konnte nicht geöffnet werden.")

# Frames extrahieren
count = 0
while True:
    ret, frame = cap.read()
    if not ret:
        break
    cv2.imwrite(os.path.join(output_folder, f"frame_{count:05d}.png"), frame)
    count += 1

cap.release()
print(f"✅ Fertig! {count} Frames gespeichert in '{output_folder}'")
