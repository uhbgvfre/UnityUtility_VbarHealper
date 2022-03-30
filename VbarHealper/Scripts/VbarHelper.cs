using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using UnityEngine;

namespace Vbar
{
    public class VbarHelper : MonoBehaviour
    {
        public int infoScanedTimes;
        public string lastInfo;
        public Action<string> onInfoScanned;

        private VbarAPI m_VbarAPI = new VbarAPI();
        private Queue<string> infoBuffer = new Queue<string>(k_BufferSize);
        private Task m_ScanTask;
        private CancellationTokenSource m_ScanTaskHandle = new CancellationTokenSource();

        private const int k_BufferSize = 8;

        private void OnEnable() => Open();
        private void OnDisable() => Close();

        public void Open()
        {
            bool state = m_VbarAPI.OpenDevice();
            print("Open device -> " + (state ? "Succ" : "Fail"));

            m_ScanTaskHandle = new CancellationTokenSource();
            m_ScanTask = Task.Run(() =>
            {
                while (true)
                {
                    if (m_ScanTaskHandle.IsCancellationRequested) break;
                    CheckScanResult();
                }
            }, m_ScanTaskHandle.Token);
        }

        public void Close()
        {
            m_VbarAPI.CloseDevice();
            m_ScanTaskHandle.Cancel();
            print("Close device");
        }

        public void SetLight(bool isLit)
        {
            m_VbarAPI.Backlight(isLit);
        }

        public void SetScanActive(bool isActive)
        {
            m_VbarAPI.ControlScan(isActive);
            print("Set scan active -> " + isActive.ToString());
        }

        private void CheckScanResult()
        {
            if (!m_VbarAPI.GetResultStr(out byte[] data, out int size)) return;
            string info = Encoding.UTF8.GetString(data, 0, size);

            if (string.IsNullOrWhiteSpace(info)) return;

            if (infoBuffer.Count >= k_BufferSize)
            {
                infoBuffer.Dequeue();
            }

            infoBuffer.Enqueue(info);
            lastInfo = info;
            onInfoScanned?.Invoke(info);
            infoScanedTimes++;
            print("ScanResult: " + info);
        }
    }
}