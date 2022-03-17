using System.Text;
using UnityEngine;

namespace Vbar
{
    public class VbarHealper : MonoBehaviour
    {
        private VbarAPI m_VbarAPI = new VbarAPI();

        private void OnEnable() => Open();
        private void OnDisable() => Close();

        public void Open()
        {
            bool state = m_VbarAPI.OpenDevice();
            print("Open device -> " + (state ? "Succ" : "Fail"));
        }

        public void Close()
        {
            m_VbarAPI.CloseDevice();
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

        private void Update()
        {
            TryScanCode();
        }

        private void TryScanCode()
        {
            if (!m_VbarAPI.GetResultStr(out byte[] data, out int size)) return;
            string msg = Encoding.UTF8.GetString(data, 0, size);

            if (string.IsNullOrWhiteSpace(msg)) return;
            print("ScanResult: " + msg);
        }
    }
}
