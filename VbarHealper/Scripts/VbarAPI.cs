using System;
using System.Runtime.InteropServices;

// DllBit:64x

namespace Vbar
{
    public class VbarAPI
    {
        private IntPtr dev = IntPtr.Zero;

        // 開啟信道
        [DllImport("vbar.dll", EntryPoint = "vbar_channel_open", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr vbar_channel_open(int type, long parm);

        // 發送數據
        [DllImport("vbar.dll", EntryPoint = "vbar_channel_send", CallingConvention = CallingConvention.Cdecl)]
        private static extern int vbar_channel_send(IntPtr dev, byte[] data, int length);

        // 接收數據
        [DllImport("vbar.dll", EntryPoint = "vbar_channel_recv", CallingConvention = CallingConvention.Cdecl)]
        private static extern int vbar_channel_recv(IntPtr dev, byte[] buffer, int size, int milliseconds);

        // 關閉信道
        [DllImport("vbar.dll", EntryPoint = "vbar_channel_close", CallingConvention = CallingConvention.Cdecl)]
        private static extern void vbar_channel_close(IntPtr dev);


        // 連接設備
        public bool OpenDevice()
        {
            dev = vbar_channel_open(1, 1);
            if (dev == IntPtr.Zero)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // 斷開設備
        public void CloseDevice()
        {
            if (dev != null)
            {
                vbar_channel_close(dev);
                dev = IntPtr.Zero;
            }
        }

        private byte[] iSetByte_ctl = new byte[64];

        // 掃碼開關
        public void ControlScan(bool cswitch)
        {
            if (dev != null)
            {
                if (cswitch)
                {
                    iSetByte_ctl[0] = 0x55;
                    iSetByte_ctl[1] = 0xAA;
                    iSetByte_ctl[2] = 0x05;
                    iSetByte_ctl[3] = 0x01;
                    iSetByte_ctl[4] = 0x00;
                    iSetByte_ctl[5] = 0x00;
                    iSetByte_ctl[6] = 0xfb;
                }
                else
                {
                    iSetByte_ctl[0] = 0x55;
                    iSetByte_ctl[1] = 0xAA;
                    iSetByte_ctl[2] = 0x05;
                    iSetByte_ctl[3] = 0x01;
                    iSetByte_ctl[4] = 0x00;
                    iSetByte_ctl[5] = 0x01;
                    iSetByte_ctl[6] = 0xfa;
                }
                vbar_channel_send(dev, iSetByte_ctl, 64);
            }

        }

        // 背光控制
        byte[] iSetByte = new byte[64];
        public void Backlight(bool bswitch)
        {
            if (dev != null)
            {
                if (bswitch)
                {
                    iSetByte[0] = 0x55;
                    iSetByte[1] = 0xAA;
                    iSetByte[2] = 0x24;
                    iSetByte[3] = 0x01;
                    iSetByte[4] = 0x00;
                    iSetByte[5] = 0x01;
                    iSetByte[6] = 0xDB;
                }
                else
                {
                    iSetByte[0] = 0x55;
                    iSetByte[1] = 0xAA;
                    iSetByte[2] = 0x24;
                    iSetByte[3] = 0x01;
                    iSetByte[4] = 0x00;
                    iSetByte[5] = 0x00;
                    iSetByte[6] = 0xDA;
                }
                vbar_channel_send(dev, iSetByte, 64);
            }

        }

        // 解碼設置
        public bool GetResultStr(out byte[] result_buffer, out int result_size)
        {
            byte[] c_result = new byte[1024];
            if (dev != null)
            {
                byte[] bufferrecv = new byte[1024];
                vbar_channel_recv(dev, bufferrecv, 1024, 200);
                if (bufferrecv[0] == 85 && bufferrecv[1] == 170 && bufferrecv[3] == 0)
                {
                    Console.WriteLine(bufferrecv[4]);
                    Console.WriteLine(bufferrecv[5]);
                    int datalen = bufferrecv[4] + (bufferrecv[5] << 8);
                    Console.WriteLine(datalen);
                    byte[] readBuffers = new byte[datalen];
                    for (int s1 = 0; s1 < datalen; s1++)
                    {
                        readBuffers[s1] = bufferrecv[6 + s1];
                    }
                    result_buffer = readBuffers;
                    result_size = datalen;
                    return true;
                }
                else
                {
                    result_buffer = null;
                    result_size = 0;
                    return false;
                }
            }
            else
            {
                result_buffer = null;
                result_size = 0;
                return false;
            }
        }
    }
}
