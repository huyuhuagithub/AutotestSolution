using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractEquipment
{
    public abstract class CANEquipmentAbstract
    {
        public bool IsOpen { get; set; }
        public abstract bool initializeCAN(uint DeviceType, uint DeviceIndex, uint CANIndex, byte baudratio);
        public abstract string Read(uint command, uint deviceType, uint deviceIndex, uint cANIndex);
        public abstract bool Write(string command, uint deviceType, uint deviceIndex, uint cANIndex, uint frameid);
        public abstract string Query(string command, uint filterID, uint deviceType, uint deviceIndex, uint cANIndex, uint frameid);
        public abstract bool CancelCAN(uint DeviceType, uint DeviceIndex, uint CANIndex);
    }
}
