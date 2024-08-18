using System.Reflection.Emit;
using System.Reflection;
using SharpDX;
using SharpDX.XInput;

namespace XboxControllerToolkit
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var controller = new Controller(UserIndex.One);
            while (true)
            {
                Console.SetBufferSize(160, 100);
                Console.Clear();
                var (connectLeft, connectTop) = Console.GetCursorPosition();
                while (!controller.IsConnected)
                {
                    Console.SetCursorPosition(0, connectTop);
                    Console.Write($"[{DateTime.Now:HH:mm:ss.fff}] Xbox Controller [No.{controller.UserIndex}] Not Connected.");
                    Thread.Sleep(100);
                }

                Console.SetCursorPosition(0, connectTop);
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Xbox Controller [No.{controller.UserIndex}] Connected.".PadRight(80));

                var battery = controller.GetBatteryInformation(BatteryDeviceType.Gamepad);
                Console.WriteLine($"""
                    Battery Information:
                        Type: {battery.BatteryType}
                        Level: {battery.BatteryLevel}
                    """);
                if (controller.GetCapabilities(DeviceQueryType.Any, out var capabilities))
                {
                    Console.WriteLine($"""
                        Capabilities:
                            Type: {capabilities.Type}
                            Gamepad:
                                Buttons: {capabilities.Gamepad.Buttons.ToString().Replace(" ", string.Empty)}
                                LeftThumbX: {capabilities.Gamepad.LeftThumbX}
                                LeftThumbY: {capabilities.Gamepad.LeftThumbY}
                                LeftTrigger: {capabilities.Gamepad.LeftTrigger}
                                RightThumbX: {capabilities.Gamepad.RightThumbX}
                                RightThumbY: {capabilities.Gamepad.RightThumbY}
                                RightTrigger: {capabilities.Gamepad.RightTrigger}
                            Vibration:
                                LeftMotorSpeed: {capabilities.Vibration.LeftMotorSpeed}
                                RightMotorSpeed: {capabilities.Vibration.RightMotorSpeed}
                            SubType: {capabilities.SubType}
                            Flags: {capabilities.Flags}
                        """);
                }
                var result = controller.GetKeystroke(DeviceQueryType.Any, out var keystroke);
                Console.WriteLine($"Get Keystroke: [{result.Code:X}] {result.Success}/{result.Failure}");
                var resultDescriptor = ResultDescriptor.Find(result);
                Console.WriteLine($"""
                    Keystroke Result Description:
                        [{resultDescriptor.Code:X}] [{resultDescriptor.Module}] {resultDescriptor.NativeApiCode}/{resultDescriptor.ApiCode}: {resultDescriptor.Description}
                    """);

                var (stateLeft, stateTop) = Console.GetCursorPosition();
                while (true)
                {
                    try
                    {
                        var state = controller.GetState();
                        Console.SetCursorPosition(0, stateTop - 1);
                        Console.Write($"""
                            Real-time State: [{DateTime.Now:HH:mm:ss.fff}] [SN={state.PacketNumber,10}]
                                {"Buttons",12} = {state.Gamepad.Buttons.ToString().Replace(" ", string.Empty),-80}
                                {"LeftThumbX",12} = {state.Gamepad.LeftThumbX,-10}
                                {"LeftThumbY",12} = {state.Gamepad.LeftThumbY,-10}
                                {"RightThumbX",12} = {state.Gamepad.RightThumbX,-10}
                                {"RightThumbY",12} = {state.Gamepad.RightThumbY,-10}
                                {"LeftTrigger",12} = {state.Gamepad.LeftTrigger,-5}
                                {"RightTrigger",12} = {state.Gamepad.RightTrigger,-5}
                            """);
                        Thread.Sleep(1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Failed to get Xbox Controller State:\n\t{ex.Message}");
                        break;
                    }
                }
                Console.Read();
            }
        }
    }
}
