using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    /// <summary>
    /// 入力
    /// </summary>
    public static class Input
    {
        static Dictionary<Inputs, int> inputState;
        static Dictionary<Inputs, InputMapping> inputMappings = new Dictionary<Inputs, InputMapping>();

        public static void InitInput()
        {
            inputState = new Dictionary<Inputs, int>();
            foreach (Inputs item in Enum.GetValues(typeof(Inputs)))
            {
                inputState[item] = 0;
            }

            inputMappings[Inputs.Up] = new InputMapping()
            {
                IsAxis = true,
                AxisNumber = 1,
                AxisThreshold = 0.5f,
                AxisType = InputMapping.AxisInputType.Up,
                Key = asd.Keys.Up,
            };

            inputMappings[Inputs.Down] = new InputMapping()
            {
                IsAxis = true,
                AxisNumber = 1,
                AxisThreshold = -0.5f,
                AxisType = InputMapping.AxisInputType.Down,
                Key = asd.Keys.Down,
            };

            inputMappings[Inputs.Left] = new InputMapping()
            {
                IsAxis = true,
                AxisNumber = 0,
                AxisThreshold = -0.5f,
                AxisType = InputMapping.AxisInputType.Down,
                Key = asd.Keys.Left,
            };

            inputMappings[Inputs.Right] = new InputMapping()
            {
                IsAxis = true,
                AxisNumber = 0,
                AxisThreshold = 0.5f,
                AxisType = InputMapping.AxisInputType.Up,
                Key = asd.Keys.Right,
            };

            inputMappings[Inputs.A] = new InputMapping()
            {
                IsAxis = false,
                ButtonNumber = 0,
                Key = asd.Keys.Z,
            };

            inputMappings[Inputs.B] = new InputMapping()
            {
                IsAxis = false,
                ButtonNumber = 1,
                Key = asd.Keys.LeftShift,
            };

            inputMappings[Inputs.L] = new InputMapping()
            {
                IsAxis = false,
                ButtonNumber = 5,
                Key = asd.Keys.LeftControl,
            };

            inputMappings[Inputs.R] = new InputMapping()
            {
                IsAxis = false,
                ButtonNumber = 6,
                Key = asd.Keys.RightControl,
            };

            inputMappings[Inputs.Esc] = new InputMapping()
            {
                IsAxis = false,
                ButtonNumber = 8,
                Key = asd.Keys.Escape,
            };
        }

        public static void UpdateInput()
        {
            if (inputState == null) InitInput();
            if (asd.Engine.JoystickContainer != null)
            {
                foreach (Inputs item in Enum.GetValues(typeof(Inputs)))
                {
                    switch (InputButton(item))
                    {
                        case asd.ButtonState.Push:
                            inputState[item] = 1;
                            break;
                        case asd.ButtonState.Release:
                            inputState[item] = -1;
                            break;
                        case asd.ButtonState.Hold:
                            inputState[item]++;
                            break;
                        case asd.ButtonState.Free:
                            inputState[item] = 0;
                            break;
                    }
                }
            }
            else
            {
                foreach (Inputs item in Enum.GetValues(typeof(Inputs)))
                {
                    switch (GetButtonState(item))
                    {
                        case asd.ButtonState.Push:
                            inputState[item] = 1;
                            break;
                        case asd.ButtonState.Release:
                            inputState[item] = -1;
                            break;
                        case asd.ButtonState.Hold:
                            inputState[item]++;
                            break;
                        case asd.ButtonState.Free:
                            inputState[item] = 0;
                            break;
                    }
                }
            }
        }

        public static int GetInputState(Inputs inputs)
        {
            return inputState[inputs];
        }

        public static Func<Inputs, asd.ButtonState> GetButtonState { get; set; } = DefaultFunc;

        static asd.ButtonState DefaultFunc(Inputs inputs)
        {
            if (!inputMappings.ContainsKey(inputs)) return asd.ButtonState.Free;
            return asd.Engine.Keyboard.GetKeyState(inputMappings[inputs].Key);
        }

        static asd.ButtonState InputButton(Inputs inputs)
        {
            if (!inputMappings.ContainsKey(inputs)) return asd.ButtonState.Free;
            if (asd.Engine.Keyboard.GetKeyState(inputMappings[inputs].Key) != asd.ButtonState.Free) return asd.Engine.Keyboard.GetKeyState(inputMappings[inputs].Key);
            if (!asd.Engine.JoystickContainer.GetIsPresentAt(0)) return asd.ButtonState.Free;
            if (inputMappings[inputs].IsAxis)
            {
                if (inputMappings[inputs].AxisType == InputMapping.AxisInputType.Down)
                {
                    if (asd.Engine.JoystickContainer.GetJoystickAt(0).GetAxisState(inputMappings[inputs].AxisNumber) <= inputMappings[inputs].AxisThreshold)
                    {
                        if (GetInputState(inputs) > 0) return asd.ButtonState.Hold;
                        if (GetInputState(inputs) < 1) return asd.ButtonState.Push;
                    }
                    else
                    {
                        if (GetInputState(inputs) == 0) return asd.ButtonState.Free;
                        if (GetInputState(inputs) > 0) return asd.ButtonState.Release;
                    }
                }
                else if (inputMappings[inputs].AxisType == InputMapping.AxisInputType.Up)
                {
                    if (asd.Engine.JoystickContainer.GetJoystickAt(0).GetAxisState(inputMappings[inputs].AxisNumber) >= inputMappings[inputs].AxisThreshold)
                    {
                        if (GetInputState(inputs) > 0) return asd.ButtonState.Hold;
                        if (GetInputState(inputs) < 1) return asd.ButtonState.Push;
                    }
                    else
                    {
                        if (GetInputState(inputs) == 0) return asd.ButtonState.Free;
                        if (GetInputState(inputs) > 0) return asd.ButtonState.Release;
                    }
                }
            }
            return asd.Engine.JoystickContainer.GetJoystickAt(0).GetButtonState(inputMappings[inputs].ButtonNumber);
        }

        public static void SetConfig(KeyConfigIO keyConfig)
        {
            foreach (var item in keyConfig.InputMappings)
            {
                if (keyConfig.ControllerName == "KeyBoard") inputMappings[item.Key] = InputMapping.MergeMapping(keyConfig.InputMappings[item.Key], inputMappings[item.Key]);
                else inputMappings[item.Key] = InputMapping.MergeMapping(inputMappings[item.Key], keyConfig.InputMappings[item.Key]);
            }
        }
    }

    /// <summary>
    /// 入力リスト
    /// </summary>
    public enum Inputs
    {
        Left = 0,
        Right = 1,
        Up = 2,
        Down = 3,
        A = 4,
        B = 5,
        L = 6,
        R = 7,
        Esc = 8,
    }
}
