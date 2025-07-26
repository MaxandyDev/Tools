import os
import sys
from fontTools.ttLib import TTFont
from PyQt5.QtWidgets import (
    QApplication, QWidget, QVBoxLayout, QHBoxLayout, QLabel, QPushButton, QFileDialog, QMessageBox
)

class FontConverterApp(QWidget):
    def __init__(self):
        super().__init__()
        self.initUI()

    def initUI(self):
        self.setWindowTitle("Font Konverter")
        self.setGeometry(100, 100, 400, 200)

        # Layout
        layout = QVBoxLayout()

        # Eingabeordner
        self.input_folder_label = QLabel("Kein Ordner ausgewählt", self)
        input_button = QPushButton("Eingabeordner auswählen", self)
        input_button.clicked.connect(self.select_input_folder)

        input_layout = QHBoxLayout()
        input_layout.addWidget(self.input_folder_label)
        input_layout.addWidget(input_button)
        layout.addLayout(input_layout)

        # Ausgabeordner
        self.output_folder_label = QLabel("Kein Ordner ausgewählt", self)
        output_button = QPushButton("Ausgabeordner auswählen", self)
        output_button.clicked.connect(self.select_output_folder)

        output_layout = QHBoxLayout()
        output_layout.addWidget(self.output_folder_label)
        output_layout.addWidget(output_button)
        layout.addLayout(output_layout)

        # Starte Konvertierung
        convert_button = QPushButton("Konvertierung starten", self)
        convert_button.clicked.connect(self.start_conversion)
        layout.addWidget(convert_button)

        self.setLayout(layout)

    def select_input_folder(self):
        folder = QFileDialog.getExistingDirectory(self, "Eingabeordner auswählen")
        if folder:
            self.input_folder_label.setText(folder)

    def select_output_folder(self):
        folder = QFileDialog.getExistingDirectory(self, "Ausgabeordner auswählen")
        if folder:
            self.output_folder_label.setText(folder)

    def start_conversion(self):
        input_folder = self.input_folder_label.text()
        output_folder = self.output_folder_label.text()

        if not input_folder or not output_folder:
            QMessageBox.critical(self, "Fehler", "Bitte wähle sowohl den Eingabe- als auch den Ausgabeordner.")
            return

        if not os.path.exists(output_folder):
            os.makedirs(output_folder)

        success_count = 0
        error_count = 0

        for filename in os.listdir(input_folder):
            if filename.endswith(".ttf") or filename.endswith(".otf"):
                input_path = os.path.join(input_folder, filename)
                if self.convert_font(input_path, output_folder):
                    success_count += 1
                else:
                    error_count += 1

        QMessageBox.information(
            self,
            "Fertig",
            f"Konvertierung abgeschlossen!\nErfolgreich: {success_count}\nFehlgeschlagen: {error_count}",
        )

    def convert_font(self, input_path, output_folder):
        try:
            font = TTFont(input_path)
            font_name = os.path.splitext(os.path.basename(input_path))[0]

            # Konvertiere zu WOFF
            woff_path = os.path.join(output_folder, f"{font_name}.woff")
            font.flavor = "woff"
            font.save(woff_path)

            # Konvertiere zu WOFF2
            woff2_path = os.path.join(output_folder, f"{font_name}.woff2")
            font.flavor = "woff2"
            font.save(woff2_path)

            return True
        except Exception as e:
            print(f"Fehler bei der Konvertierung: {e}")
            return False

if __name__ == "__main__":
    app = QApplication(sys.argv)
    window = FontConverterApp()
    window.show()
    sys.exit(app.exec_())