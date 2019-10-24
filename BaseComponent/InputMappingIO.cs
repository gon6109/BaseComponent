using BaseComponent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    [Serializable]
    public class KeyConfigIO
    {
        public string ControllerName;
        public Dictionary<Inputs, InputMapping> InputMappings;

        public KeyConfigIO()
        {
            InputMappings = new Dictionary<Inputs, InputMapping>();
        }

        /// <summary>
        /// キーコンフィグを保存
        /// </summary>
        /// <param name="path">ファイル</param>
        public void SaveConfig(string path)
        {
            using (FileStream mapfile = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(mapfile, this);
            }
        }

        /// <summary>
        /// キーコンフィグをロードする
        /// </summary>
        /// <param name="path">ファイル</param>
        static public KeyConfigIO LoadConfig(string path)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            KeyConfigIO map = (KeyConfigIO)serializer.Deserialize(IO.GetStream(path));
            return map;
        }
    }

    [Serializable]
    public class InputMapping
    {
        public bool IsAxis;
        public int ButtonNumber;
        public int AxisNumber;
        public float AxisThreshold;
        public AxisInputType AxisType;
        public asd.Keys Key;

        [Serializable]
        public enum AxisInputType
        {
            Up,
            Down
        }

        public bool Compare(InputMapping to, bool isKey)
        {
            if (isKey)
                return Key == to.Key;
            else
            {
                if (IsAxis != to.IsAxis)
                    return false;

                if (IsAxis)
                    return AxisNumber == to.AxisNumber && AxisType == to.AxisType && AxisThreshold == to.AxisThreshold;
                else
                    return ButtonNumber == to.ButtonNumber;
            }
        }

        public static InputMapping MergeMapping(InputMapping keyboard, InputMapping joystick)
        {
            return new InputMapping()
            {
                IsAxis = joystick.IsAxis,
                ButtonNumber = joystick.ButtonNumber,
                AxisNumber = joystick.AxisNumber,
                AxisThreshold = joystick.AxisThreshold,
                AxisType = joystick.AxisType,
                Key = keyboard.Key,
            };
        }
    }
}