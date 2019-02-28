/*!
 *
 * Project: Pi Control
 *
 * MIT License
 *
 * Copyright (C) 2017 : KUNBUS GmbH, Heerweg 15C, 73370 Denkendorf, Germany
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 *
 * \file piControlIf.c
 *
 * \brief PI Control Interface
 *
 *
 */

#ifndef PICONTROLIF_H_
#define PICONTROLIF_H_

/******************************************************************************/
/********************************  Includes  **********************************/
/******************************************************************************/

#include <stdint.h>
#include <piControl.h>


/******************************************************************************/
/*********************************  Types  ************************************/
/******************************************************************************/

extern int PiControlHandle_g;


/******************************************************************************/
/*******************************  Prototypes  *********************************/
/******************************************************************************/
#ifdef __cplusplus
extern "C" {
#endif

void __attribute__((__cdecl__)) piControlOpen(void);
void __attribute__((__cdecl__)) piControlClose(void);
int32_t __attribute__((__cdecl__)) piControlReset(void);
int32_t __attribute__((__cdecl__)) piControlRead(uint32_t Offset, uint32_t Length, uint8_t *pData);
int32_t __attribute__((__cdecl__)) piControlWrite(uint32_t Offset, uint32_t Length, uint8_t *pData);
int32_t __attribute__((__cdecl__)) piControlGetDeviceInfo(SDeviceInfo *pDev);
int32_t __attribute__((__cdecl__)) piControlGetDeviceInfoList(SDeviceInfo *pDev);
int32_t __attribute__((__cdecl__)) piControlGetBitValue(SPIValue *pSpiValue);
int32_t __attribute__((__cdecl__)) piControlSetBitValue(SPIValue *pSpiValue);
int32_t __attribute__((__cdecl__)) piControlGetVariableInfo(SPIVariable *pSpiVariable);
int32_t __attribute__((__cdecl__)) piControlFindVariable(const char *name);
int32_t __attribute__((__cdecl__)) piControlResetCounter(int32_t address, int32_t bitfield);
int32_t __attribute__((__cdecl__)) piControlWaitForEvent(void);
int32_t __attribute__((__cdecl__)) piControlUpdateFirmware(uint32_t addr_p);
/*
#ifdef KUNBUS_TEST
int piControlIntMsg(int msg, unsigned char *data, int size);
int piControlSetSerial(int addr, int serial);
#endif
*/

void __attribute__((__cdecl__)) piControlClose(void);

#ifdef __cplusplus
}
#endif

#endif /* PICONTROLIF_H_ */
