#ifndef __DHT22_H
#define __DHT22_H


#ifdef __cplusplus
extern "C" {
#endif

void initDht(void);
void getNextSample(float* Temperature, float* Humidity);

#ifdef __cplusplus
}
#endif


#endif//__DHT22_H

