#ifndef __LOCKSERVO_H
#define __LOCKSERVO_H


#ifdef __cplusplus
extern "C" {
#endif

void initServo(void);
void latch(bool status);

#ifdef __cplusplus
}
#endif


#endif//__LOCKSERVO_H
