#include <stdlib.h>
#include <stdio.h>
#include <stdint.h>
#include <pgmspace.h>
#include <Arduino.h>
#include <time.h>
#include "dht22.h"
#include "adafruit_http.h"
#include "lockServo.h"

/* This sample uses the _LL APIs of iothub_client for example purposes.
That does not mean that HTTP only works with the _LL APIs.
Simply changing the using the convenience layer (functions not having _LL)
and removing calls to _DoWork will yield the same results. */

#include "AzureIoTHub.h"
#include "sdk/schemaserializer.h"

static const char DeviceId[] = "adafruitFeather1";
static const char connectionString[] = "HostName=dns.azure-devices.net;DeviceId=adafruitFeather1;SharedAccessKey=Wq0WlSMWOY8p8C2/VFC9mneOGoJVikWixbzE9t0J7pc=";

static int redLedPin = 12;
static int redLedState = LOW;
static int greenLedPin = 13;
static int greenLedState = LOW;

int fsrReading;
// Define the Model
BEGIN_NAMESPACE(AdafruitController);

DECLARE_MODEL(DnsAdafruit,
WITH_DATA(ascii_char_ptr, DeviceId),
WITH_DATA(int, EventTime),
WITH_DATA(int, MTemperature),
WITH_DATA(int, Humidity),
WITH_DATA(int, Pressure),
WITH_ACTION(OpenLock),
WITH_ACTION(CloseLock)
);

END_NAMESPACE(AdafruitController);

DEFINE_ENUM_STRINGS(IOTHUB_CLIENT_CONFIRMATION_RESULT, IOTHUB_CLIENT_CONFIRMATION_RESULT_VALUES)

EXECUTE_COMMAND_RESULT CloseLock(DnsAdafruit* device)
{
    (void)device;
    LogInfo("Toggling Latch Closed.\r\n");
    redLedState = HIGH;
    greenLedState = LOW;
    digitalWrite(redLedPin, redLedState);
    digitalWrite(greenLedPin, greenLedState);
    latch(true);
    return EXECUTE_COMMAND_SUCCESS;
}

EXECUTE_COMMAND_RESULT OpenLock(DnsAdafruit* device)
{
    (void)device;
    LogInfo("Toggling Latch Open.\r\n");
    redLedState = LOW;
    greenLedState = HIGH;
    digitalWrite(greenLedPin, greenLedState);
    digitalWrite(redLedPin, redLedState);
    latch(false);
    return EXECUTE_COMMAND_SUCCESS;
}

void sendCallback(IOTHUB_CLIENT_CONFIRMATION_RESULT result, void* userContextCallback)
{
    int messageTrackingId = (intptr_t)userContextCallback;

    LogInfo("Message Id: %d Received.\r\n", messageTrackingId);

    LogInfo("Result Call Back Called! Result is: %s \r\n", ENUM_TO_STRING(IOTHUB_CLIENT_CONFIRMATION_RESULT, result));
}

static void sendMessage(IOTHUB_CLIENT_LL_HANDLE iotHubClientHandle, const unsigned char* buffer, size_t size)
{
    static unsigned int messageTrackingId;
    IOTHUB_MESSAGE_HANDLE messageHandle = IoTHubMessage_CreateFromByteArray(buffer, size);
    if (messageHandle == NULL)
    {
        LogInfo("unable to create a new IoTHubMessage\r\n");
    }
    else
    {
        if (IoTHubClient_LL_SendEventAsync(iotHubClientHandle, messageHandle, sendCallback, (void*)(uintptr_t)messageTrackingId) != IOTHUB_CLIENT_OK)
        {
            LogInfo("failed to hand over the message to IoTHubClient");
        }
        else
        {
            LogInfo("IoTHubClient accepted the message for delivery\r\n");
        }
        IoTHubMessage_Destroy(messageHandle);
    }
    free((void*)buffer);
    messageTrackingId++;
}

/*this function "links" IoTHub to the serialization library*/
static IOTHUBMESSAGE_DISPOSITION_RESULT IoTHubMessage(IOTHUB_MESSAGE_HANDLE message, void* userContextCallback)
{
    LogInfo("Command Received\r\n");
    IOTHUBMESSAGE_DISPOSITION_RESULT result;
    const unsigned char* buffer;
    size_t size;
    if (IoTHubMessage_GetByteArray(message, &buffer, &size) != IOTHUB_MESSAGE_OK)
    {
        LogInfo("unable to IoTHubMessage_GetByteArray\r\n");
        result = EXECUTE_COMMAND_ERROR;
    }
    else
    {
        /*buffer is not zero terminated*/
        char* temp = malloc(size + 1);
        if (temp == NULL)
        {
            LogInfo("failed to malloc\r\n");
            result = EXECUTE_COMMAND_ERROR;
        }
        else
        {
            memcpy(temp, buffer, size);
            temp[size] = '\0';
            EXECUTE_COMMAND_RESULT executeCommandResult = EXECUTE_COMMAND(userContextCallback, temp);
            result =
                (executeCommandResult == EXECUTE_COMMAND_ERROR) ? IOTHUBMESSAGE_ABANDONED :
                (executeCommandResult == EXECUTE_COMMAND_SUCCESS) ? IOTHUBMESSAGE_ACCEPTED :
                IOTHUBMESSAGE_REJECTED;
            free(temp);
        }
    }
    return result;
}

void http_run(void)
{
    initDht();
    initServo();
    digitalWrite(redLedPin, redLedState);
    pinMode(redLedPin, OUTPUT);
    
    digitalWrite(greenLedPin, greenLedState);
    pinMode(greenLedPin, OUTPUT);

    if (serializer_init(NULL) != SERIALIZER_OK)
    {
        LogInfo("Failed on serializer_init\r\n");
    }
    else
    {
        IOTHUB_CLIENT_LL_HANDLE iotHubClientHandle = IoTHubClient_LL_CreateFromConnectionString(connectionString, HTTP_Protocol);
        srand((unsigned int)time(NULL));
        int avgWindSpeed = 10.0;

        if (iotHubClientHandle == NULL)
        {
            LogInfo("Failed on IoTHubClient_LL_Create\r\n");
        }
        else
        {
            unsigned int minimumPollingTime = 9; /*because it can poll "after 9 seconds" polls will happen effectively at ~10 seconds*/
            if (IoTHubClient_LL_SetOption(iotHubClientHandle, "MinimumPollingTime", &minimumPollingTime) != IOTHUB_CLIENT_OK)
            {
                LogInfo("failure to set option \"MinimumPollingTime\"\r\n");
            }

#ifdef MBED_BUILD_TIMESTAMP
            // For mbed add the certificate information
            if (IoTHubClient_LL_SetOption(iotHubClientHandle, "TrustedCerts", certificates) != IOTHUB_CLIENT_OK)
            {
                LogInfo("failure to set option \"TrustedCerts\"\r\n");
            }
#endif // MBED_BUILD_TIMESTAMP

            DnsAdafruit* myDevice = CREATE_MODEL_INSTANCE(AdafruitController, DnsAdafruit);
            if (myDevice == NULL)
            {
                LogInfo("Failed on CREATE_MODEL_INSTANCE\r\n");
            }
            else
            {
                if (IoTHubClient_LL_SetMessageCallback(iotHubClientHandle, IoTHubMessage, myDevice) != IOTHUB_CLIENT_OK)
                {
                    LogInfo("unable to IoTHubClient_SetMessageCallback\r\n");
                }
                else
                {
                    
                    /* wait for commands */
                    long Prev_time_ms = millis();
                    char buff[11];
                    int timeNow = 0;

                    while (1)
                    {
                        long Curr_time_ms = millis();
                        if (Curr_time_ms >= Prev_time_ms + 5000)
                        {
                            Prev_time_ms = Curr_time_ms;
                            
                            timeNow = (int)time(NULL);
                            //sprintf(buff, "%d", timeNow);

                            float Temp_c__f, Humi_pct__f;
                            getNextSample(&Temp_c__f, &Humi_pct__f);
                            fsrReading = analogRead(15);
                            
                            myDevice->DeviceId = DeviceId;
                            myDevice->EventTime = timeNow;
                            myDevice->MTemperature = (int)Temp_c__f;
                            myDevice->Humidity = (int)Humi_pct__f;
                            myDevice->Pressure = (int)fsrReading;

                            LogInfo("Result: %s | %d | %d | %d\r\n", myDevice->DeviceId, myDevice->EventTime, myDevice->MTemperature, myDevice->Humidity);
                            LogInfo("Pressure : %d\n", fsrReading);
                            unsigned char* destination;
                            size_t destinationSize;
                            
                            if (SERIALIZE(&destination, &destinationSize, myDevice->DeviceId, myDevice->EventTime, myDevice->MTemperature, myDevice->Humidity, myDevice->Pressure) != IOT_AGENT_OK)
                            {
                                LogInfo("Failed to serialize\r\n");
                            }
                            else
                            {
                                IOTHUB_MESSAGE_HANDLE messageHandle = IoTHubMessage_CreateFromByteArray(destination, destinationSize);
                                if (messageHandle == NULL)
                                {
                                    LogInfo("unable to create a new IoTHubMessage\r\n");
                                }
                                else
                                {
                                    if (IoTHubClient_LL_SendEventAsync(iotHubClientHandle, messageHandle, sendCallback, (void*)1) != IOTHUB_CLIENT_OK)
                                    {
                                        LogInfo("failed to hand over the message to IoTHubClient\r\n");
                                    }
                                    else
                                    {
                                        LogInfo("IoTHubClient accepted the message for delivery\r\n");
                                    }
    
                                    IoTHubMessage_Destroy(messageHandle);
                                }
                                free(destination);
                            }
                            
                        }
                        
                        IoTHubClient_LL_DoWork(iotHubClientHandle);
                        ThreadAPI_Sleep(100);
                    }
                }

                DESTROY_MODEL_INSTANCE(myDevice);
            }
            IoTHubClient_LL_Destroy(iotHubClientHandle);
        }
        serializer_deinit();
    }
}


