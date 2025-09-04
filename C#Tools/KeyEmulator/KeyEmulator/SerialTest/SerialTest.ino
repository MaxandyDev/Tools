// Code für Normaly Open Taster


const int buttonPinA = 2;
const int buttonPinS = 3;
const int buttonPinD = 4;

const char A = 'A';
const char S = 'S';
const char D = 'D';

bool lastStateA = HIGH;
bool lastStateS = HIGH;
bool lastStateD = HIGH;

void setup() {
  Serial.begin(9600);

  pinMode(buttonPinA, INPUT_PULLUP);
  pinMode(buttonPinS, INPUT_PULLUP);
  pinMode(buttonPinD, INPUT_PULLUP);
}

void loop() {
  // Aktuellen Zustand lesen
  bool currentA = digitalRead(buttonPinA);
  bool currentS = digitalRead(buttonPinS);
  bool currentD = digitalRead(buttonPinD);

  if (lastStateA == HIGH && currentA == LOW) {
    Serial.println(A);
  }
  if (lastStateS == HIGH && currentS == LOW) {
    Serial.println(S);
  }
  if (lastStateD == HIGH && currentD == LOW) {
    Serial.println(D);
  }

  lastStateA = currentA;
  lastStateS = currentS;
  lastStateD = currentD;
}



/*
Code für Normaly Closed Taster

const int buttonPinA = 2;
const int buttonPinS = 3;
const int buttonPinD = 4;

const char A = 'A';
const char S = 'S';
const char D = 'D';

bool lastStateA = HIGH;  // NC-Taster: ungdrückt = LOW, gedrückt = HIGH (nach Pull-Up)
bool lastStateS = HIGH;
bool lastStateD = HIGH;

void setup() {
  Serial.begin(9600);

  // Eingänge mit Pullup
  pinMode(buttonPinA, INPUT_PULLUP);
  pinMode(buttonPinS, INPUT_PULLUP);
  pinMode(buttonPinD, INPUT_PULLUP);
}

void loop() {
  // aktuellen Zustand lesen
  bool currentA = digitalRead(buttonPinA);
  bool currentS = digitalRead(buttonPinS);
  bool currentD = digitalRead(buttonPinD);

  // RISING-Flanke erkennen: LOW -> HIGH
  if (lastStateA == LOW && currentA == HIGH) {
    Serial.println(A);
  }
  if (lastStateS == LOW && currentS == HIGH) {
    Serial.println(S);
  }
  if (lastStateD == LOW && currentD == HIGH) {
    Serial.println(D);
  }

  // Zustand merken
  lastStateA = currentA;
  lastStateS = currentS;
  lastStateD = currentD;
}

*/