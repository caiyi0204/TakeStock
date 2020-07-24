using ClouReaderAPI;
using ClouReaderAPI.ClouInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeStock
{
    public class ReaderComponent
    {
        public string ConnID { set; get; }
        /// <summary>
        /// 读卡类型
        /// </summary>
        private eReadType readType;
        /// <summary>
        /// 天线设置
        /// </summary>
        private eAntennaNo antNo;
        /// <summary>
        /// 天线数量
        /// </summary>
        public Int32 antNUM = 0;
        /// <summary>
        /// 单次还是循环
        /// 0 单次，1循环
        /// </summary>
        private Int32 singleOrWhile = 1;
        /// <summary>
        /// 天线的Tag
        /// </summary>
        private List<int> AntennaTagList = new List<int> { 1,2,4,8,16,32,64,128 };
        /// <summary>
        /// 正在读取标签
        /// </summary>
        public bool IsStartRead = false;

        public bool Usbconnect() {
            var listUsbDevicePath = CLReader.GetUsbHidDeviceList();

            bool isConnect = CLReader.CreateUsbConn(listUsbDevicePath[0], (IntPtr)1000, new AsynchronousMessage());
            this.ConnID = listUsbDevicePath[0];

            return isConnect;
        }

        /// <summary>
        /// 初始化天线
        /// </summary>
        /// <param name="AntennaCount">天线的数量</param>
        /// <returns></returns>
        public int GeteAntennaNo(int AntennaCount) {
            for (int i=0 ; i<AntennaCount ; i++) {
                antNUM += AntennaTagList[i];
                if (i == 0)
                {
                    antNo = (eAntennaNo)AntennaTagList[i];
                }
                else {
                    antNo = antNo | (eAntennaNo)AntennaTagList[i];
                }
            }
            return antNUM;
        }

        /// <summary>
        /// 开始读卡
        /// </summary>
        public void StartReadEpc(bool IsTid = false) {
            this.readType = (eReadType)singleOrWhile;
            //var st = CLReader._Tag6C.GetEPC(ConnID, this.antNo, readType);
            antNo = antNo | (eAntennaNo)15;
            var st2 = -1;
            if (IsTid)
            {
                CLReader._Config.Stop(ConnID);
                st2 = CLReader._Tag6C.GetEPC_TID(ConnID, antNo, readType);
            }
            else {
                ClouReaderAPI.CLReader.RFID_OPTION.StopReader(ConnID);
                st2 = CLReader._Tag6C.GetEPC(ConnID, antNo, readType);
            }

            CLReader.DIC_CONNECT[ConnID].ProcessCount = 0;
            IsStartRead = true;
        }

        /// <summary>
        /// 停止读卡
        /// </summary>
        public void StopReadEpc() {
            CLReader._Tag6C.Stop(ConnID);
        }

        /// <summary>
        /// 关闭当前连接
        /// </summary>
        /// <returns></returns>
        public bool CloseNowConnect() {
            bool res = true;
            if (!String.IsNullOrEmpty(ConnID))
            {
                try
                {
                    if (this.IsStartRead)
                    {
                        CLReader._Config.Stop(ConnID);
                        this.IsStartRead = false;
                    }
                    ClouReaderAPI.CLReader.CloseConn(ConnID);
                    this.ConnID = "";
                }
                catch {
                    res = false;
                }
            }
            return res;
        }

    }
}
