#include "lockServo.h"
#include <Servo.h>
#include <Arduino.h>

#define SERVOPIN            14         // Pin which is connected to the DHT sensor.

Servo latchServo;
int pos = 0;

void initServo(void)
{
    Serial.println("------------------------------------");
    Serial.println("Init Servo");
    Serial.println("------------------------------------");
    latchServo.attach(SERVOPIN); // Servo Attached at pin 14 on Adafruit Huzzah
    latchServo.write(pos);
}

void latch(bool status)
{
    if(status) {
      Serial.println("Latching lock");
      if(pos == 90){
        Serial.println("Latch already closed");  
      } else {
        Serial.println("Latch closing");
        pos = 90;
        latchServo.write(pos);
      }
    } else {
      Serial.println("Unlatching lock");
      if(pos == 0) {
        Serial.println("Latch already opened");  
      } else {
        Serial.println("Latch opening");
        pos = 0;
        latchServo.write(pos);
      }
    }
}
