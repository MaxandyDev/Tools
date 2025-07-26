import keyboard
import time
from threading import Thread, Event

class AutoClicker:
    def __init__(self):
        self.running = Event()
        self.max_cps = 5  # Maximale Klicks pro Sekunde
        self.click_thread = None
        self.click_count = 0
        self.max_clicks = 2511                       # Maximal 11 Klicks, dann stoppt es

    def start_clicking(self):
        self.click_count = 0  # Zurücksetzen bei Neustart
        while self.running.is_set() and self.click_count < self.max_clicks:
            start_time = time.time()
            
            # Sende Leertasten-Klick
            keyboard.press(' ')
            keyboard.release(' ')
            self.click_count += 1
            
            # Berechne die benötigte Wartezeit für 200 Klicks/Sekunde
            elapsed = time.time() - start_time
            min_delay = 1.0 / self.max_cps
            if elapsed < min_delay:
                time.sleep(min_delay - elapsed)
        
        # Automatisch stoppen, wenn max_clicks erreicht
        if self.click_count >= self.max_clicks:
            self.running.clear()
            print(f"Automatisch gestoppt nach {self.max_clicks} Klicks.")

    def toggle(self):
        if self.running.is_set():
            self.running.clear()
            if self.click_thread is not None:
                self.click_thread.join()
            print("Autoklicker gestoppt")
        else:
            self.running.set()
            self.click_thread = Thread(target=self.start_clicking)
            self.click_thread.start()
            print(f"Autoklicker gestartet (max {self.max_clicks} Klicks)")

def main():
    autoklicker = AutoClicker()
    
    print("Drücke die Leertaste zum Starten/Stoppen des Autoklickers...")
    print("Drücke ESC zum Beenden des Programms.")
    
    keyboard.add_hotkey('space', autoklicker.toggle)
    
    # Warte auf ESC zum Beenden
    keyboard.wait('esc')
    autoklicker.running.clear()
    if autoklicker.click_thread is not None:
        autoklicker.click_thread.join()

if __name__ == "__main__":
    main()