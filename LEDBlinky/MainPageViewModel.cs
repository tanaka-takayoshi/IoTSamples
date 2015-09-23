using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace LEDBlinky
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private static readonly PropertyChangedEventArgs GpioStatusPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(GpioStatus));
        private static readonly PropertyChangedEventArgs LedColorPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(LedColor));

        private const int LED_PIN = 5;

        private GpioPin pin;
        private GpioPinValue pinValue;
        private DispatcherTimer timer;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

        private string gpioStatus = "";
        
        public string GpioStatus
        {
            get { return gpioStatus; }
            set
            {
                if (gpioStatus == value) { return; }
                gpioStatus = value;
                PropertyChanged?.Invoke(this, GpioStatusPropertyChangedEventArgs);
            }
        }

        private SolidColorBrush ledColor;
        public SolidColorBrush LedColor
        {
            get { return ledColor; }
            set
            {
                if (ledColor == value) { return; }
                ledColor = value;
                PropertyChanged?.Invoke(this, LedColorPropertyChangedEventArgs);
            }
        }

        public MainPageViewModel()
        {
            timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(500)};
            timer.Tick += Timer_Tick;
            InitGPIO();
            if (pin != null)
            {
                //timer.Start();
            }
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                pin = null;
                GpioStatus = "There is no GPIO controller on this device.";
                return;
            }

            pin = gpio.OpenPin(LED_PIN);
            pinValue = GpioPinValue.High;
            pin.Write(pinValue);
            pin.SetDriveMode(GpioPinDriveMode.Output);

            GpioStatus = "GPIO pin initialized correctly!";

        }

        public void SwitchTimer()
        {
            
        }

        private void Timer_Tick(object sender, object e)
        {
            if (pinValue == GpioPinValue.High)
            {
                pinValue = GpioPinValue.Low;
                pin.Write(pinValue);
                LedColor = redBrush;
            }
            else
            {
                pinValue = GpioPinValue.High;
                pin.Write(pinValue);
                LedColor = grayBrush;
            }
        }

        public void TurnOnLed()
        {
            if (pinValue == GpioPinValue.High)
            {
                pinValue = GpioPinValue.Low;
                pin.Write(pinValue);
                LedColor = redBrush;
            }
        }

        public void TurnOffLed()
        {
            if (pinValue == GpioPinValue.Low)
            {
                pinValue = GpioPinValue.High;
                pin.Write(pinValue);
                LedColor = grayBrush;
            }
        }

        public void StartTimer()
        {
            timer.Start();
        }

        public void StopTimer()
        {
            TurnOffLed();
            timer.Stop();
        }
    }
}
