import os
import sys
from PyQt5.QtWidgets import (
    QApplication, QWidget, QVBoxLayout, QHBoxLayout, QLabel, QPushButton, 
    QFileDialog, QMessageBox, QComboBox, QSpinBox, QLineEdit
)
from PyQt5.QtCore import Qt
from PIL import Image


class ImageConverterApp(QWidget):
    def __init__(self):
        super().__init__()
        self.init_ui()

    def init_ui(self):
        """Initialisiert die Benutzeroberfläche."""
        self.setWindowTitle("Bildkonverter")
        self.setGeometry(100, 100, 500, 250)

        # Layout
        layout = QVBoxLayout()

        # Eingabeordner
        input_layout = QHBoxLayout()
        self.input_label = QLabel("Eingabeordner: Nicht ausgewählt", self)
        input_button = QPushButton("Ordner auswählen", self)
        input_button.clicked.connect(self.select_input_folder)
        input_layout.addWidget(self.input_label)
        input_layout.addWidget(input_button)
        layout.addLayout(input_layout)

        # Ausgabeordner
        output_layout = QHBoxLayout()
        self.output_label = QLabel("Ausgabeordner: Nicht ausgewählt", self)
        output_button = QPushButton("Ordner auswählen", self)
        output_button.clicked.connect(self.select_output_folder)
        output_layout.addWidget(self.output_label)
        output_layout.addWidget(output_button)
        layout.addLayout(output_layout)

        # Zielformat
        format_layout = QHBoxLayout()
        format_label = QLabel("Zielformat:", self)
        self.format_combo = QComboBox(self)
        self.format_combo.addItems(["JPEG", "JPG", "PNG", "WEBP", "ICO"])
        self.format_combo.currentTextChanged.connect(self.toggle_options)
        format_layout.addWidget(format_label)
        format_layout.addWidget(self.format_combo)
        layout.addLayout(format_layout)

        # ICO Optionen (Container-Widget)
        self.ico_container = QWidget()
        self.ico_options_layout = QHBoxLayout(self.ico_container)
        ico_size_label = QLabel("ICO Größen (px, durch Komma getrennt):", self)
        self.ico_sizes_input = QLineEdit("16,32,48,64,128,256", self)
        self.ico_options_layout.addWidget(ico_size_label)
        self.ico_options_layout.addWidget(self.ico_sizes_input)
        layout.addWidget(self.ico_container)
        self.ico_container.hide()

        # Qualitätseinstellung (Container-Widget)
        self.quality_container = QWidget()
        self.quality_layout = QHBoxLayout(self.quality_container)
        quality_label = QLabel("Qualität (1-100):", self)
        self.quality_spinbox = QSpinBox(self)
        self.quality_spinbox.setRange(1, 100)
        self.quality_spinbox.setValue(80)
        self.quality_layout.addWidget(quality_label)
        self.quality_layout.addWidget(self.quality_spinbox)
        layout.addWidget(self.quality_container)
        self.quality_container.hide()

        # Startbutton
        start_button = QPushButton("Konvertierung starten", self)
        start_button.clicked.connect(self.start_conversion)
        layout.addWidget(start_button)

        # Hauptlayout setzen
        self.setLayout(layout)

    def toggle_options(self, format):
        """Zeigt/versteckt Optionen basierend auf ausgewähltem Format."""
        self.ico_container.setVisible(format == "ICO")
        self.quality_container.setVisible(format in ["WEBP", "JPEG", "JPG"])

    def select_input_folder(self):
        """Öffnet einen Dialog zur Auswahl des Eingabeordners."""
        folder = QFileDialog.getExistingDirectory(self, "Eingabeordner auswählen")
        if folder:
            self.input_label.setText(f"Eingabeordner: {folder}")

    def select_output_folder(self):
        """Öffnet einen Dialog zur Auswahl des Ausgabeordners."""
        folder = QFileDialog.getExistingDirectory(self, "Ausgabeordner auswählen")
        if folder:
            self.output_label.setText(f"Ausgabeordner: {folder}")

    def start_conversion(self):
        """Startet die Konvertierung."""
        input_folder = self.input_label.text().replace("Eingabeordner: ", "")
        output_folder = self.output_label.text().replace("Ausgabeordner: ", "")
        target_format = self.format_combo.currentText().upper()

        if not input_folder or not output_folder:
            QMessageBox.warning(self, "Warnung", "Bitte wähle sowohl den Eingabe- als auch den Ausgabeordner aus.")
            return

        self.convert_images(input_folder, output_folder, target_format)

    def convert_images(self, input_folder, output_folder, target_format):
        """
        Konvertiert alle unterstützten Bilddateien in einem Ordner in das Zielformat.

        :param input_folder: Pfad zum Ordner mit den Bilddateien
        :param output_folder: Pfad zum Ordner, in dem die konvertierten Dateien gespeichert werden
        :param target_format: Zielformat (z. B. "JPEG", "PNG", "WEBP", "ICO")
        """
        try:
            # Überprüfe, ob der Ausgabeordner existiert, andernfalls erstelle ihn
            if not os.path.exists(output_folder):
                os.makedirs(output_folder)

            # Unterstützte Bildformate
            supported_formats = [".webp", ".png", ".jpg", ".jpeg", ".bmp", ".tiff", ".gif"]

            # Durchlaufe alle Dateien im Eingabeordner
            for filename in os.listdir(input_folder):
                # Überprüfe, ob die Datei ein unterstütztes Format hat
                if any(filename.lower().endswith(ext) for ext in supported_formats):
                    # Vollständiger Pfad zur Bilddatei
                    input_path = os.path.join(input_folder, filename)

                    # Öffne das Bild
                    with Image.open(input_path) as img:
                        # Erstelle den Ausgabedateinamen mit der richtigen Endung
                        output_filename = os.path.splitext(filename)[0] + f".{target_format.lower()}"
                        output_path = os.path.join(output_folder, output_filename)

                        # Spezialbehandlung für verschiedene Formate
                        if target_format == "WEBP":
                            img.save(output_path, "WEBP", quality=self.quality_spinbox.value())
                        elif target_format in ["JPEG", "JPG"]:
                            img.convert("RGB").save(output_path, "JPEG", quality=self.quality_spinbox.value())
                        elif target_format == "ICO":
                            # ICO-Dateien benötigen mehrere Größen
                            sizes = [int(size.strip()) for size in self.ico_sizes_input.text().split(",")]
                            img.save(output_path, sizes=[(size, size) for size in sizes])
                        else:
                            # Für PNG und andere Formate
                            img.save(output_path, target_format)
                        
                        print(f"Konvertiert: {filename} -> {output_filename}")

            QMessageBox.information(self, "Erfolg", "Konvertierung abgeschlossen!")
        except Exception as e:
            QMessageBox.critical(self, "Fehler", f"Ein Fehler ist aufgetreten: {e}")


if __name__ == "__main__":
    app = QApplication(sys.argv)
    window = ImageConverterApp()
    window.show()
    sys.exit(app.exec_())